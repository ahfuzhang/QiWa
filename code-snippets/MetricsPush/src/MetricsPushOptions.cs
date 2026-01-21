namespace MetricsPush;

internal sealed record MetricsPushOptions(
    int HttpPort,
    int PushIntervalSeconds,
    Uri PushAddress,
    IReadOnlyDictionary<string, string> ExtraLabels);
