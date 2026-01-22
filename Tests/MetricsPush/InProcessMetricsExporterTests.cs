using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MetricsPush;
using Xunit;

namespace Tests.MetricsPush;

public class InProcessMetricsExporterTests
{
    [Fact]
    public void GetSnapshot_CopiesStoredData()
    {
        var exporter = new InProcessMetricsExporter(new Dictionary<string, string> { { "k", "v" } });
        const string payloadText = "test_counter{k=\"v\"} 1\n";
        byte[] payload = Encoding.UTF8.GetBytes(payloadText);
        SetExporterState(exporter, payload);

        using var snapshot = exporter.GetSnapshot(out int length);
        Assert.Equal(payload.Length, length);
        Assert.NotNull(snapshot.Data);
        string text = Encoding.UTF8.GetString(snapshot.Data!, 0, length);
        Assert.Equal(payloadText, text);
    }

    [Fact]
    public void GetSnapshot_ReturnsEmpty_IfNoData()
    {
        var exporter = new InProcessMetricsExporter(null!);
        using var snapshot = exporter.GetSnapshot(out int length);
        Assert.Equal(0, length);
        Assert.Null(snapshot.Data);
    }

    private static void SetExporterState(InProcessMetricsExporter exporter, byte[] payload)
    {
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
}
