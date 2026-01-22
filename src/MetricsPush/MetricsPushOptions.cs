using System;
using System.Collections.Generic;

namespace MetricsPush;

public class MetricsPushOptions
{
    /// <summary>
    /// Push interval in seconds.
    /// </summary>
    public int PushIntervalSeconds { get; set; }

    /// <summary>
    /// Push target address.
    /// </summary>
    public string PushAddr { get; set; } = string.Empty;

    /// <summary>
    /// Extra labels to add to all metrics.
    /// </summary>
    public Dictionary<string, string> PublicTags { get; set; } = new();

    public MetricsPushOptions(int pushIntervalSeconds, string pushAddr, Dictionary<string, string> publicTags)
    {
        PushIntervalSeconds = pushIntervalSeconds;
        PushAddr = pushAddr;
        PublicTags = publicTags ?? new Dictionary<string, string>();
    }
}
