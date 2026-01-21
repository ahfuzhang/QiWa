using System.Diagnostics.Metrics;

namespace MetricsPush;

internal static class MetricsPushTelemetry
{
    public static readonly Meter Meter = new("MetricsPush");
    public static readonly Counter<long> PushCount = Meter.CreateCounter<long>("metrics_push_requests_total");
    public static readonly Histogram<long> PayloadBytes = Meter.CreateHistogram<long>("metrics_push_payload_bytes");
    public static readonly Histogram<long> PayloadUncompressedBytes = Meter.CreateHistogram<long>("metrics_push_payload_uncompressed_bytes");
}
