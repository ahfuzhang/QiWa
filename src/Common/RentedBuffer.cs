
using System.Buffers;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Common;

/// <summary>
/// 提供一个自主管理的内存分配组件。高性能，绕过 GC，但是忘记释放会导致内存泄露。
/// 相当于重新实现了 golang 的 []byte
/// </summary>
public struct RentedBuffer : IDisposable
{
    public byte[]? Data;
    public System.Int32 Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RentedBuffer(System.Int32 length)
    {
        Rent(length);
    }

    private const string Utf8FormatterFailedMessage = "Utf8Formatter.TryFormat failed.";

    /// <summary>
    /// 从内存池借用内存
    /// </summary>
    /// <param name="length">需要的内存大小</param>
    /// <exception>out of memory</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Rent(System.Int32 length)
    {
        // todo: rent 的总数，释放的总数，应该加上上报。便于跟踪性能
        Data = ArrayPool<byte>.Shared.Rent(length);
        Length = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if (Data != null)
        {
            ArrayPool<byte>.Shared.Return(Data);
            Data = null;
        }
        Length = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> Bytes()
    {
        if (Data == null || Length == 0)
        {
            return [];
        }
        return Data.AsSpan(0, Length);
    }

    const int CodeOfFormatFail = 254;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Extend(int needed)
    {
        if (Length + needed <= Data!.Length)
        {
            return;
        }
        byte[] newData = ArrayPool<byte>.Shared.Rent(Data!.Length * 2 + needed);
        Array.Copy(Data, newData, Length);
        ArrayPool<byte>.Shared.Return(Data);
        Data = newData;
    }

    /// <summary>
    /// 注意：调用此函数一定会在堆上创建 new string[]{}. AppendMulti 与 Append(string s) 区分开来。
    /// </summary>
    /// <param name="arr"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Error AppendMulti(params string[] arr)
    {
        foreach (var s in arr)
        {
            if (string.IsNullOrEmpty(s))
            {
                continue;
            }
            int byteCount = System.Text.Encoding.UTF8.GetByteCount(s);
            Extend(byteCount);
            int bytesWritten = System.Text.Encoding.UTF8.GetBytes(s, 0, s.Length, Data!, Length);
            Length += bytesWritten;
        }
        return default(Error);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Error Append(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return default(Error);
        }
        int byteCount = System.Text.Encoding.UTF8.GetByteCount(s);
        Extend(byteCount);
        int bytesWritten = System.Text.Encoding.UTF8.GetBytes(s, 0, s.Length, Data!, Length);
        Length += bytesWritten;
        return default(Error);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Error Append(byte c)
    {
        Extend(1);
        Data![Length] = c;
        Length++;
        return default(Error);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Error Append(ReadOnlySpan<byte> s)
    {
        if (s.IsEmpty)
        {
            return default(Error);
        }
        Extend(s.Length);
        s.CopyTo(Data.AsSpan(Length));
        Length += s.Length;
        return default(Error);
    }

    const int maxIntegerLength = 20;
    const int maxBoolLength = 5;
    const int maxUtcDatetimeLength = 28;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Error Append(long value)
    {
        Extend(maxIntegerLength);
        if (!System.Buffers.Text.Utf8Formatter.TryFormat(value, Data.AsSpan(Length), out int bytesWritten))
        {
            return new Error { Code = CodeOfFormatFail, Message = Utf8FormatterFailedMessage };
        }
        Length += bytesWritten;
        return default(Error);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Error Append(System.UInt64 value)
    {
        Extend(maxIntegerLength);
        if (!System.Buffers.Text.Utf8Formatter.TryFormat(value, Data.AsSpan(Length), out int bytesWritten))
        {
            return new Error { Code = CodeOfFormatFail, Message = Utf8FormatterFailedMessage };
        }
        Length += bytesWritten;
        return default(Error);
    }

    const int maxFloat64Length = 64;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Error Append(double value)
    {
        Extend(maxFloat64Length);
        if (!Utf8Formatter.TryFormat(value, Data.AsSpan(Length), out int bytesWritten))
        {
            return new Error { Code = CodeOfFormatFail, Message = Utf8FormatterFailedMessage };
        }
        Length += bytesWritten;
        return default(Error);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Error Append(bool value)
    {
        Extend(maxBoolLength);
        if (value)
        {
            "true"u8.CopyTo(Data.AsSpan(Length));
            Length += 4;
        }
        else
        {
            "false"u8.CopyTo(Data.AsSpan(Length));
            Length += 5;
        }
        return default(Error);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Error AppendUtcDatetime(DateTime dtm)
    {
        if (dtm.Kind == DateTimeKind.Local)
        {
            dtm = dtm.ToUniversalTime();
        }
        else if (dtm.Kind == DateTimeKind.Unspecified)
        {
            dtm = DateTime.SpecifyKind(dtm, DateTimeKind.Utc);
        }
        Extend(maxUtcDatetimeLength);
        var dst = Data.AsSpan(Length, maxUtcDatetimeLength);

        int year = dtm.Year;
        int month = dtm.Month;
        int day = dtm.Day;
        int hour = dtm.Hour;
        int minute = dtm.Minute;
        int second = dtm.Second;
        int fraction = (int)(dtm.Ticks % TimeSpan.TicksPerSecond);

        Write4(year, dst, 0);
        dst[4] = (byte)'-';
        Write2(month, dst, 5);
        dst[7] = (byte)'-';
        Write2(day, dst, 8);
        dst[10] = (byte)'T';
        Write2(hour, dst, 11);
        dst[13] = (byte)':';
        Write2(minute, dst, 14);
        dst[16] = (byte)':';
        Write2(second, dst, 17);
        dst[19] = (byte)'.';
        Write7(fraction, dst, 20);
        dst[27] = (byte)'Z';

        Length += maxUtcDatetimeLength;
        return default(Error);

        static void Write2(int value, Span<byte> destination, int offset)
        {
            destination[offset] = (byte)('0' + (value / 10));
            destination[offset + 1] = (byte)('0' + (value % 10));
        }

        static void Write4(int value, Span<byte> destination, int offset)
        {
            destination[offset + 3] = (byte)('0' + (value % 10));
            value /= 10;
            destination[offset + 2] = (byte)('0' + (value % 10));
            value /= 10;
            destination[offset + 1] = (byte)('0' + (value % 10));
            value /= 10;
            destination[offset] = (byte)('0' + (value % 10));
        }

        static void Write7(int value, Span<byte> destination, int offset)
        {
            destination[offset + 6] = (byte)('0' + (value % 10));
            value /= 10;
            destination[offset + 5] = (byte)('0' + (value % 10));
            value /= 10;
            destination[offset + 4] = (byte)('0' + (value % 10));
            value /= 10;
            destination[offset + 3] = (byte)('0' + (value % 10));
            value /= 10;
            destination[offset + 2] = (byte)('0' + (value % 10));
            value /= 10;
            destination[offset + 1] = (byte)('0' + (value % 10));
            value /= 10;
            destination[offset] = (byte)('0' + (value % 10));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly RentedBuffer Clone()
    {
        RentedBuffer cloned = new(Data!.Length);
        Array.Copy(Data, cloned.Data!, Length);
        cloned.Length = Length;
        return cloned;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendAsJsonEscapedString(string s)
    {
        JsonEncodedText encoded = JsonEncodedText.Encode(s);
        Extend(encoded.EncodedUtf8Bytes.Length);
        Append(encoded.EncodedUtf8Bytes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendAsJsonEscapedString(ReadOnlySpan<byte> s)
    {
        int needed = s.Length * 2;
        Extend(needed);
        foreach (var b in s)
        {
            switch (b)
            {
                case (byte)'\t':
                    Data![Length] = (byte)'\\';
                    Data[Length + 1] = (byte)'t';
                    Length += 2;
                    break;
                case (byte)'\n':
                    Data![Length] = (byte)'\\';
                    Data[Length + 1] = (byte)'n';
                    Length += 2;
                    break;
                case (byte)'\\':
                    Data![Length] = (byte)'\\';
                    Data[Length + 1] = (byte)'\\';
                    Length += 2;
                    break;
                case (byte)'"':
                    Data![Length] = (byte)'\\';
                    Data[Length + 1] = (byte)'"';
                    Length += 2;
                    break;
                default:
                    Data![Length] = b;
                    Length++;
                    break;
            }
        }
    }
}
