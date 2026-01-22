using System.Collections.Generic;
using System.Text;
using MetricsPush;
using OpenTelemetry;
using System.Diagnostics.Metrics;
using OpenTelemetry.Metrics;
using Xunit;

namespace Tests.MetricsPush;

public class InProcessMetricsExporterTests
{
    [Fact]
    public void Export_StoresData()
    {
        // Arrange
        var exporter = new InProcessMetricsExporter(new Dictionary<string, string> { { "k", "v" } });
        
        // Cannot easily construct Batch<Metric> manually as constructors are internal/protected or hard to mock.
        // However, we can use OTel SDK to emit metrics and have them exported to our exporter.
        
        using var meterProvider = Sdk.CreateMeterProviderBuilder()
            .AddMeter("TestMeter")
            .AddReader(new BaseExportingMetricReader(exporter))
            .Build();

        var meter = new Meter("TestMeter");
        var counter = meter.CreateCounter<long>("test_counter");
        counter.Add(1);

        // Act
        meterProvider.ForceFlush();

        // Assert
        using var snapshot = exporter.GetSnapshot(out int length);
        Assert.True(length > 0);
        string text = Encoding.UTF8.GetString(snapshot.Data, 0, length);
        Assert.Contains("test_counter", text);
        Assert.Contains("k=\"v\"", text);
    }

    [Fact]
    public void GetSnapshot_ReturnsEmpty_IfNoData()
    {
        var exporter = new InProcessMetricsExporter(null!);
        using var snapshot = exporter.GetSnapshot(out int length);
        Assert.Equal(0, length);
    }
}
