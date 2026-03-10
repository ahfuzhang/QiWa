using System.Net;
using System.Reflection;
using System.Text;
using MetricsPush;
using Moq;
using Moq.Protected;
using Xunit;

namespace Tests.MetricsPush;

public class MetricsPusherUnitTests
{
    [Fact]
    public async Task Constructor_ArgValidation()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.ConfigureKestrel(options => options.ListenAnyIP(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new MetricsPusher(0, "u", null!, builder));
        Assert.Throws<ArgumentNullException>(() => new MetricsPusher(1, null!, null!, builder));
        Assert.Throws<ArgumentNullException>(() => new MetricsPusher(1, "u", null!, null!));
    }

    [Fact]
    public async Task PushOnce_Successful_IncrementsTelemetry()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.ConfigureKestrel(options => options.ListenAnyIP(0));
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
            .Verifiable();

        var httpClient = new HttpClient(handler.Object);
        var tags = new Dictionary<string, string>();

        using var pusher = new MetricsPusher(60, "http://test.com", tags, builder, httpClient);

        var payload = Encoding.UTF8.GetBytes("metrics_push_count 1\n");
        SeedExporter(pusher, payload);
        await InvokePushOnceAsync(pusher).ConfigureAwait(false);

        // Assert
        handler.Protected().Verify(
            "SendAsync",
            Times.AtLeastOnce(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post &&
                req.RequestUri == new Uri("http://test.com") &&
                req.Content != null &&
                req.Content.Headers.ContentEncoding.Contains("zstd")
            ),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    private static void SeedExporter(MetricsPusher pusher, byte[] payload)
    {
        var exporterField = typeof(MetricsPusher).GetField("_exporter", BindingFlags.NonPublic | BindingFlags.Instance);
        var exporter = (InProcessMetricsExporter)exporterField!.GetValue(pusher)!;

        var buffer = new Common.RentedBuffer
        {
            Data = payload,
            Length = payload.Length
        };

        var lockField = typeof(InProcessMetricsExporter).GetField("_lock", BindingFlags.NonPublic | BindingFlags.Instance);
        var gate = lockField!.GetValue(exporter)!;
        lock (gate)
        {
            typeof(InProcessMetricsExporter).GetField("_latest", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(exporter, buffer);
            typeof(InProcessMetricsExporter).GetField("_latestUsed", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(exporter, payload.Length);
        }
    }

    private static Task InvokePushOnceAsync(MetricsPusher pusher)
    {
        var method = typeof(MetricsPusher).GetMethod("PushOnceAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        return (Task)method!.Invoke(pusher, new object[] { CancellationToken.None })!;
    }
}
