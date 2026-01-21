using System.Buffers;
using System.Text;
using Xunit;

namespace MetricsPush.Tests;

public class Utf8LabelWriterTests
{
    [Theory]
    [InlineData("plain", "plain")]
    [InlineData("a\"b", "a\\\"b")]
    [InlineData("a\\b", "a\\\\b")]
    [InlineData("a\nb", "a\\nb")]
    [InlineData("a\rb", "a\\rb")]
    [InlineData("a\tb", "a\\tb")]
    public void AppendEscaped_WritesExpected(string input, string expected)
    {
        var writer = new ArrayBufferWriter<byte>();
        Utf8LabelWriter.AppendEscaped(writer, input);
        string output = Encoding.UTF8.GetString(writer.WrittenSpan);
        Assert.Equal(expected, output);
    }
}
