using System;
using System.Buffers;
using System.Text;
using MetricsPush;
using Xunit;

namespace Tests.MetricsPush;

public class Utf8BufferWriterTests
{
    [Fact]
    public void AppendString_ValidString_WritesUtf8Bytes()
    {
        // Arrange
        var writer = new ArrayBufferWriter<byte>();
        string input = "Hello, World!";

        // Act
        Utf8BufferWriter.AppendString(writer, input);

        // Assert
        Assert.Equal(input, Encoding.UTF8.GetString(writer.WrittenSpan));
    }

    [Fact]
    public void AppendString_NullOrEmpty_WritesNothing()
    {
        // Arrange
        var writer = new ArrayBufferWriter<byte>();

        // Act
        Utf8BufferWriter.AppendString(writer, null);
        Utf8BufferWriter.AppendString(writer, string.Empty);

        // Assert
        Assert.Equal(0, writer.WrittenCount);
    }

    [Fact]
    public void AppendString_UnicodeBuffer_WritesCorrectUtf8()
    {
        // Arrange
        var writer = new ArrayBufferWriter<byte>();
        string input = "你好，世界！🌟";

        // Act
        Utf8BufferWriter.AppendString(writer, input);

        // Assert
        Assert.Equal(input, Encoding.UTF8.GetString(writer.WrittenSpan));
    }

    [Fact]
    public void AppendBytes_ValidBytes_WritesBytes()
    {
         // Arrange
        var writer = new ArrayBufferWriter<byte>();
        byte[] input = new byte[] { 1, 2, 3, 4, 5 };

        // Act
        Utf8BufferWriter.AppendBytes(writer, input);

        // Assert
        Assert.Equal(input, writer.WrittenSpan.ToArray());
    }

    [Fact]
    public void AppendBytes_EmptyBytes_WritesNothing()
    {
        // Arrange
        var writer = new ArrayBufferWriter<byte>();

        // Act
        Utf8BufferWriter.AppendBytes(writer, Span<byte>.Empty);

        // Assert
        Assert.Equal(0, writer.WrittenCount);
    }

    [Fact]
    public void AppendByte_ValidByte_WritesByte()
    {
         // Arrange
        var writer = new ArrayBufferWriter<byte>();

        // Act
        Utf8BufferWriter.AppendByte(writer, 65); // 'A'

        // Assert
        Assert.Equal(1, writer.WrittenCount);
        Assert.Equal(65, writer.WrittenSpan[0]);
    }
}
