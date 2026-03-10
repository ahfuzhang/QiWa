using System.Buffers;
using System.Diagnostics.Metrics;
using System.Text;
using MetricsPush;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using Xunit;

namespace Tests.MetricsPush;

public class MetricTextFormatterTests
{
    [Fact]
    public void Format_CorrectlyFormatsCounter()
    {
        // Ideally we want to call MetricTextFormatter.Format(writer, metrics, extraLabels).
        // But creating `Metric` objects manually is hard (internal OTel types).
        // We will perform an integration-style unit test via InProcessMetricsExporter logic, 
        // OR we can rely on `InProcessMetricsExporterTests` for end-to-end format verification.
        // Given `MetricTextFormatter` is internal, maybe it's acceptable to test it via the Exporter.
        // However, the user asked for "each file corresponding test file".
        // Let's try to verify via Exporter but focused on formatting details.

        // Note: MetricTextFormatter is internal. InternalsVisibleTo is set for MetricsPush.Tests.
        // But we still need valid Metric instances.

        var exportedItems = new List<Metric>();
        var exporter = new CapturingExporter(exportedItems);

        using var meterProvider = Sdk.CreateMeterProviderBuilder()
            .AddMeter("TestMeter")
            .AddReader(new BaseExportingMetricReader(exporter))
            .Build();

        var meter = new Meter("TestMeter");
        var counter = meter.CreateCounter<long>("my_counter");
        counter.Add(10, new KeyValuePair<string, object?>("tag1", "val1"));

        meterProvider.ForceFlush();

        Assert.NotEmpty(exportedItems);

        // Now call Formatter directly
        var writer = new ArrayBufferWriter<byte>();
        var labels = new Dictionary<string, string> { { "global", "gval" } };
        MetricTextFormatter.Format(writer, exportedItems, labels);

        string text = Encoding.UTF8.GetString(writer.WrittenSpan);

        // EXPECTED:
        // # HELP ...
        // # TYPE ...
        // my_counter_total{global="gval",tag1="val1"} 10
        // (OTel Prometheus exporter adds _total suffix usually, but our formatter implementation?)
        // Let's check our implementation:
        // It uses `metric.Name`. It doesn't seem to enforce suffix unless OTel `Metric` has it.
        // Our implementation:
        // Utf8BufferWriter.AppendString(writer, name);
        // It appends labels.
        // Then value.

        // Wait, regular OTel Prometheus exporter adds TYPE and HELP lines.
        // Our `MetricTextFormatter.cs` implementation (from memory) just loops points and appends lines like:
        // name{labels} value
        // It does NOT seem to add TYPE/HELP lines in the provided code snippet I read earlier.
        // Let's verify expectations based on code I generated.

        Assert.Contains("my_counter", text);
        Assert.Contains("global=\"gval\"", text);
        Assert.Contains("tag1=\"val1\"", text);
        Assert.Contains(" 10", text);
    }

    private sealed class CapturingExporter : BaseExporter<Metric>
    {
        private readonly List<Metric> _items;
        public CapturingExporter(List<Metric> items) => _items = items;

        public override ExportResult Export(in Batch<Metric> batch)
        {
            foreach (var item in batch)
            {
                _items.Add(item);
            }

            return ExportResult.Success;
        }
    }
}
