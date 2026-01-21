using Xunit;

namespace MetricsPush.Tests;

public class ExtraLabelsParserTests
{
    public static IEnumerable<object?[]> ParseCases()
    {
        yield return new object?[] { null, new Dictionary<string, string>() };
        yield return new object?[] { string.Empty, new Dictionary<string, string>() };
        yield return new object?[]
        {
            "a=b&c=d&e=f",
            new Dictionary<string, string>
            {
                ["a"] = "b",
                ["c"] = "d",
                ["e"] = "f"
            }
        };
        yield return new object?[]
        {
            "a=b&c=&=d&x",
            new Dictionary<string, string>
            {
                ["a"] = "b",
                ["c"] = string.Empty,
                ["x"] = string.Empty
            }
        };
    }

    [Theory]
    [MemberData(nameof(ParseCases))]
    public void Parse_ReturnsExpected(string? raw, Dictionary<string, string> expected)
    {
        Dictionary<string, string> actual = ExtraLabelsParser.Parse(raw);
        Assert.Equal(expected.Count, actual.Count);
        foreach (var kvp in expected)
        {
            Assert.True(actual.ContainsKey(kvp.Key));
            Assert.Equal(kvp.Value, actual[kvp.Key]);
        }
    }
}
