using System;
using System.Buffers;
using Common;

namespace MetricsPush;

public class RentedBufferWriter : IBufferWriter<byte>, IDisposable
{
    private Common.RentedBuffer _buffer;
    private int _written;

    public RentedBufferWriter(int initialCapacity = 256)
    {
        _buffer = new Common.RentedBuffer();
        _buffer.Rent(initialCapacity);
        _written = 0;
    }

    public void Advance(int count)
    {
        if (count < 0)
            throw new ArgumentException(null, nameof(count));
        
        if (_written > _buffer.Length - count)
            throw new InvalidOperationException("Cannot advance past the end of the buffer.");

        _written += count;
    }

    public Memory<byte> GetMemory(int sizeHint = 0)
    {
        CheckAndResizeBuffer(sizeHint);
        return _buffer.Data.AsMemory(_written);
    }

    public Span<byte> GetSpan(int sizeHint = 0)
    {
        CheckAndResizeBuffer(sizeHint);
        return _buffer.Data.AsSpan(_written);
    }

    public ReadOnlySpan<byte> WrittenSpan => _buffer.Data.AsSpan(0, _written);
    
    public Common.RentedBuffer DetachBuffer()
    {
         // We return the current buffer and reset internal state so we don't dispose it.
         // However, the caller expects the buffer to contain only the valid data?
         // Common.RentedBuffer contains the full array.
         // This is tricky because Common.RentedBuffer doesn't store 'UsedLength'.
         // So we probably should just return the struct as is, and the caller must know the length?
         // Or we can create a new struct that wraps it?
         
         // Actually, for this usage, we probably want to return the RentedBuffer to the caller
         // so they can use it and then dispose it.
         // But RentedBuffer.Length is the *capacity*.
         
         var buffer = _buffer;
         _buffer = default; // Clear it so we don't dispose it
         return buffer;
    }

    public void Dispose()
    {
        _buffer.Dispose();
        _written = 0;
    }

    private void CheckAndResizeBuffer(int sizeHint)
    {
        if (sizeHint == 0)
        {
            sizeHint = 256;
        }

        int availableSpace = _buffer.Length - _written;
        if (sizeHint > availableSpace)
        {
            int currentLength = _buffer.Length;
            int growBy = Math.Max(sizeHint, currentLength);
            int newSize = currentLength + growBy;

            var newBuffer = new Common.RentedBuffer();
            newBuffer.Rent(newSize);

            if (_buffer.Data != null)
            {
                Array.Copy(_buffer.Data, newBuffer.Data!, _written);
                _buffer.Dispose();
            }

            _buffer = newBuffer;
        }
    }
    
    public int WrittenCount => _written;
}
