using System;
using System.Buffers;
using System.Text;

namespace MetricsPush;

// todo: 这个类不应该存在，应该在 RentedBuffer 的基础上扩展
internal static class Utf8BufferWriter {
    public static void AppendString(IBufferWriter<byte> writer, string value) {
        if (string.IsNullOrEmpty(value)) {
            return;
        }

        int byteCount = Encoding.UTF8.GetByteCount(value);
        Span<byte> span = writer.GetSpan(byteCount);
        Encoding.UTF8.GetBytes(value.AsSpan(), span);
        writer.Advance(byteCount);
    }

    public static void AppendBytes(IBufferWriter<byte> writer, ReadOnlySpan<byte> bytes) {
        if (bytes.IsEmpty) {
            return;
        }

        Span<byte> span = writer.GetSpan(bytes.Length);
        bytes.CopyTo(span);
        writer.Advance(bytes.Length);
    }

    public static void AppendByte(IBufferWriter<byte> writer, byte value) {
        Span<byte> span = writer.GetSpan(1);
        span[0] = value;
        writer.Advance(1);
    }
}
