using System;
using MetricsPush;
using Xunit;

namespace Tests.MetricsPush;

public class RentedBufferWriterTests {
    [Fact]
    public void Constructor_InitializesCorrectly() {
        using var writer = new RentedBufferWriter(1024);
        Assert.Equal(0, writer.WrittenCount);
        Assert.True(writer.WrittenSpan.IsEmpty);
    }

    [Fact]
    public void Advance_ValidCount_UpdatesWrittenCount() {
        using var writer = new RentedBufferWriter(100);
        writer.GetSpan(10);
        writer.Advance(10);
        Assert.Equal(10, writer.WrittenCount);
        Assert.Equal(10, writer.WrittenSpan.Length);
    }

    [Fact]
    public void Advance_NegativeCount_ThrowsArgumentException() {
        using var writer = new RentedBufferWriter();
        Assert.Throws<ArgumentException>(() => writer.Advance(-1));
    }

    [Fact]
    public void Advance_TooMuch_ThrowsInvalidOperationException() {
        using var writer = new RentedBufferWriter(10);
        Assert.Throws<InvalidOperationException>(() => writer.Advance(10000));
    }

    [Fact]
    public void GetSpan_ResizesBufferIfNeeded() {
        using var writer = new RentedBufferWriter(10);

        // Request more than available
        var span = writer.GetSpan(1000);
        Assert.True(span.Length >= 1000);

        writer.Advance(1000);
        Assert.Equal(1000, writer.WrittenCount);
    }

    [Fact]
    public void GetMemory_ResizesBufferIfNeeded() {
        using var writer = new RentedBufferWriter(10);
        var memory = writer.GetMemory(1000);
        Assert.True(memory.Length >= 1000);
        writer.Advance(1000);
        Assert.Equal(1000, writer.WrittenCount);
    }

    [Fact]
    public void DetachBuffer_ReturnsBufferAndResetsWriter() {
        // 1. Write some data
        using var writer = new RentedBufferWriter();
        writer.GetSpan(1)[0] = 42;
        writer.Advance(1);

        // 2. Detach
        var buffer = writer.DetachBuffer();

        // 3. Verify buffer
        Assert.NotNull(buffer.Data);
        Assert.Equal(42, buffer.Data[0]);
        Assert.Equal(1, buffer.Length);

        // 4. Verify writer reset
        Assert.Equal(0, writer.WrittenCount);
        var span = writer.GetSpan(10);
        writer.Advance(10);
        Assert.Equal(10, writer.WrittenCount);

        // Dispose buffer returned
        buffer.Dispose();
    }
}
