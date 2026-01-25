using System;
using System.Buffers;
using Common;

namespace MetricsPush;

// todo: 这个类不应该存在。应该在 RentedBuffer 的基础上扩展
public class RentedBufferWriter : IBufferWriter<byte>, IDisposable {
    private Common.RentedBuffer _buffer;
    private int _written;

    public RentedBufferWriter(int initialCapacity = 256) {
        _buffer = new Common.RentedBuffer(initialCapacity);
        _written = 0;
    }

    public void Advance(int count) {
        if (count < 0)
            throw new ArgumentException(null, nameof(count));

        int capacity = _buffer.Data?.Length ?? 0;
        if (_written > capacity - count)
            throw new InvalidOperationException("Cannot advance past the end of the buffer.");

        _written += count;
        _buffer.Length = _written;
    }

    public Memory<byte> GetMemory(int sizeHint = 0) {
        CheckAndResizeBuffer(sizeHint);
        return _buffer.Data.AsMemory(_written);
    }

    public Span<byte> GetSpan(int sizeHint = 0) {
        CheckAndResizeBuffer(sizeHint);
        return _buffer.Data.AsSpan(_written);
    }

    public ReadOnlySpan<byte> WrittenSpan => _buffer.Data == null
        ? ReadOnlySpan<byte>.Empty
        : _buffer.Data.AsSpan(0, _written);

    public Common.RentedBuffer DetachBuffer() {
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
        buffer.Length = _written;
        _buffer = default;
        _written = 0;
        return buffer;
    }

    public void Dispose() {
        _buffer.Dispose();
        _buffer = default;
        _written = 0;
    }

    private void CheckAndResizeBuffer(int sizeHint) {
        if (sizeHint == 0) {
            sizeHint = 256;
        }

        int capacity = _buffer.Data?.Length ?? 0;
        int availableSpace = capacity - _written;
        if (sizeHint > availableSpace) {
            int growBy = Math.Max(sizeHint, capacity);
            int newSize = capacity + growBy;

            var newBuffer = new Common.RentedBuffer();
            newBuffer.Rent(newSize);

            if (_buffer.Data != null) {
                if (_written > 0) {
                    Array.Copy(_buffer.Data, newBuffer.Data!, _written);
                }
                _buffer.Dispose();
            }

            newBuffer.Length = _written;
            _buffer = newBuffer;
        }
    }

    public int WrittenCount => _written;
}
