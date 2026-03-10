using MetricsPush;
using Xunit;

namespace Tests.MetricsPush;

public class MetricsPushOptionsTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var tags = new Dictionary<string, string> { { "k", "v" } };
        var options = new MetricsPushOptions(10, "http://uri", tags);

        Assert.Equal(10, options.PushIntervalSeconds);
        Assert.Equal("http://uri", options.PushAddr);
        Assert.Same(tags, options.PublicTags);
    }

    [Fact]
    public void Constructor_NullTags_InitializesEmpty()
    {
        var options = new MetricsPushOptions(10, "http://uri", null!);
        Assert.NotNull(options.PublicTags);
        Assert.Empty(options.PublicTags);
    }
}
