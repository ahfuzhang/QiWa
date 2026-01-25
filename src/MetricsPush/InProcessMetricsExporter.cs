using System;
using System.Collections.Generic;
using Common;
using OpenTelemetry;
using OpenTelemetry.Metrics;

namespace MetricsPush;

public sealed class InProcessMetricsExporter : BaseExporter<Metric> {
    private readonly object _lock = new();
    private readonly IReadOnlyDictionary<string, string> _extraLabels;
    private Common.RentedBuffer _latest;
    private int _latestUsed;

    public InProcessMetricsExporter(IReadOnlyDictionary<string, string> extraLabels) {
        _extraLabels = extraLabels ?? new Dictionary<string, string>();
    }

    public override ExportResult Export(in Batch<Metric> batch) {
        // 类型 Batch 来自 OpenTelemetry
        var items = new List<Metric>();
        foreach (var item in batch) {
            items.Add(item);
        }

        using var writer = new RentedBufferWriter(initialCapacity: 4096);
        MetricTextFormatter.Format(writer, items, _extraLabels);

        var written = writer.WrittenCount;
        var newBuffer = writer.DetachBuffer();

        lock (_lock) {
            _latest.Dispose();
            _latest = newBuffer;
            _latestUsed = written;
        }

        return ExportResult.Success;
    }

    public Common.RentedBuffer GetSnapshot(out int usedLength) {
        lock (_lock) {
            usedLength = 0;
            if (_latest.Data == null || _latestUsed == 0) {
                //Console.WriteLine("_latest.Data == null || _latestUsed == 0");
                return new Common.RentedBuffer();
            }

            var snapshot = new Common.RentedBuffer(_latestUsed);

            Array.Copy(_latest.Data, snapshot.Data!, _latestUsed);
            snapshot.Length = _latestUsed;
            usedLength = _latestUsed;

            return snapshot;
        }
    }
}
