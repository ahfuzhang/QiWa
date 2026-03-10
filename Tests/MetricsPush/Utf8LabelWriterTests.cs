using System.Buffers;
using System.Text;
using MetricsPush;
using Xunit;

namespace Tests.MetricsPush;

public class Utf8LabelWriterTests
{
    [Theory]
    [InlineData("simple", "simple")]
    [InlineData("escape\\slash", "escape\\\\slash")]
    [InlineData("escape\"quote", "escape\\\"quote")]
    [InlineData("new\nline", "new\\nline")]
    [InlineData("carriage\rreturn", "carriage\\rreturn")]
    [InlineData("tab\tchar", "tab\\tchar")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void AppendEscaped_ValidInput_WritesEscapedUtf8(string? input, string expected)
    {
        // Arrange
        var writer = new ArrayBufferWriter<byte>();

        // Act
        Utf8LabelWriter.AppendEscaped(writer, input!);

        // Assert
        string result = Encoding.UTF8.GetString(writer.WrittenSpan);
        // Note: The writer appends raw bytes. 
        // If input is null/empty, it writes nothing.
        // If expected is empty, result should be empty.
        Assert.Equal(expected, result);
    }

    [Fact]
    public void AppendEscaped_ComplexString_EscapesAll()
    {
        // Arrange
        var writer = new ArrayBufferWriter<byte>();
        string input = "Line1\nLine2\t\"Quote\"\\Backslash";
        string expected = "Line1\\nLine2\\t\\\"Quote\\\"\\\\Backslash";

        // Act
        Utf8LabelWriter.AppendEscaped(writer, input);

        // Assert
        Assert.Equal(expected, Encoding.UTF8.GetString(writer.WrittenSpan));
    }
}
