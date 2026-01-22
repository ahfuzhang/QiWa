using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
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

        using var pusher = new MetricsPusher(1, "http://test.com", tags, builder, httpClient);
        
        // We need to trigger data generation.
        // But InProcessMetricsExporter is hidden inside.
        // And we cannot access `_exporter` directly.
        // However, MetricsPusher registers OTel.
        using var app = builder.Build();
        await app.StartAsync(); // Start to ensure OTel reader runs
        
        // We need some metric update.
        // MetricsPushTelemetry is static, so we can use it.
        MetricsPushTelemetry.PushCount.Add(1);

        // Wait for loop
        await Task.Delay(2500);

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
}
