using OpenTelemetry;
using OpenTelemetry.Metrics;

namespace MetricsPush;

public sealed class InProcessMetricsExporter : BaseExporter<Metric>
{
    private readonly object _lock = new();
    private readonly IReadOnlyDictionary<string, string> _extraLabels;
    private byte[] _latest = Array.Empty<byte>();

    public InProcessMetricsExporter(IReadOnlyDictionary<string, string> extraLabels)
    {
        _extraLabels = extraLabels ?? new Dictionary<string, string>();
    }

    public override ExportResult Export(in Batch<Metric> batch)
    {
        var items = new List<Metric>();
        foreach (var item in batch)
        {
            items.Add(item);
        }
        byte[] payload = MetricTextFormatter.Format(items, _extraLabels);

        lock (_lock)
        {
            _latest = payload;
        }
        return ExportResult.Success;
    }

    public byte[] GetSnapshot()
    {
        lock (_lock)
        {
            return _latest;
        }
    }
}
