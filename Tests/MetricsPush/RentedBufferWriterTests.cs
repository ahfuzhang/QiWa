using System;
using MetricsPush;
using Xunit;

namespace Tests.MetricsPush;

public class RentedBufferWriterTests
{
    [Fact]
    public void Constructor_InitializesCorrectly()
    {
        using var writer = new RentedBufferWriter(1024);
        Assert.Equal(0, writer.WrittenCount);
        Assert.True(writer.WrittenSpan.IsEmpty);
    }

    [Fact]
    public void Advance_ValidCount_IncreasesWrittenCount()
    {
        using var writer = new RentedBufferWriter(100);
        writer.GetSpan(10); // Check valid
        writer.Advance(10);
        
        Assert.Equal(10, writer.WrittenCount);
    }

    [Fact]
    public void Advance_NegativeCount_ThrowsArgumentException()
    {
        using var writer = new RentedBufferWriter();
        Assert.Throws<ArgumentException>(() => writer.Advance(-1));
    }

    [Fact]
    public void Advance_TooMuch_ThrowsInvalidOperationException()
    {
        using var writer = new RentedBufferWriter(10);
        // We verify that if we didn't ask for a span, we can't advance past capacity.
        // Actually RentedBufferWriter checks against _buffer.Length.
        // If we initialized with 10, _buffer.Length >= 10.
        // If we advance 100, it should fail.
        Assert.Throws<InvalidOperationException>(() => writer.Advance(10000)); 
    }

    [Fact]
    public void GetSpan_ResizesBufferIfNeeded()
    {
        using var writer = new RentedBufferWriter(10);
        writer.Advance(5);
        
        // Request more than available
        var span = writer.GetSpan(1000);
        Assert.True(span.Length >= 1000);
        
        writer.Advance(1000);
        Assert.Equal(1005, writer.WrittenCount);
    }

    [Fact]
    public void GetMemory_ResizesBufferIfNeeded()
    {
        using var writer = new RentedBufferWriter(10);
        var memory = writer.GetMemory(1000);
        Assert.True(memory.Length >= 1000);
    }

    [Fact]
    public void DetachBuffer_ReturnsBufferAndResetsWriter()
    {
        // 1. Write some data
        using var writer = new RentedBufferWriter();
        writer.GetSpan(1)[0] = 42;
        writer.Advance(1);
        
        // 2. Detach
        var buffer = writer.DetachBuffer();
        
        // 3. Verify buffer
        Assert.NotNull(buffer.Data);
        Assert.Equal(42, buffer.Data[0]);
        
        // 4. Verify writer reset (buffer is default)
        // Accessing methods might fail or alloc new buffer?
        // Constructor initializes _buffer. Detach sets _buffer = default.
        // Calling GetSpan should alloc new buffer?
        // Let's check logic:
        // CheckAndResizeBuffer: if _buffer.Data == null? 
        // _buffer is struct. _buffer.Length will be 0.
        // availableSpace = 0 - 0 = 0.
        // sizeHint > 0 -> will rent new buffer.
        
        var span = writer.GetSpan(10);
        Assert.False(span.IsEmpty); // It should recover
        
        // Dispose buffer returned
        buffer.Dispose();
    }
}
