using MetricsPush;
using Xunit;

namespace Tests.MetricsPush;

public class MetricsPushTelemetryTests
{
    [Fact]
    public void Meter_IsInitialized()
    {
        Assert.NotNull(MetricsPushTelemetry.Meter);
        Assert.Equal("MetricsPush", MetricsPushTelemetry.Meter.Name);
    }

    [Fact]
    public void Counters_AreInitialized()
    {
        Assert.NotNull(MetricsPushTelemetry.PushCount);
        Assert.NotNull(MetricsPushTelemetry.PayloadBytes);
        Assert.NotNull(MetricsPushTelemetry.PayloadUncompressedBytes);
    }
}
