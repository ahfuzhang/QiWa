
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Common {
        public struct RentedBuffer : IDisposable {
        public byte[]? Data;
        public System.Int32 Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RentedBuffer(System.Int32 length){
            Rent(length);
        }

        //private const string BufferNotRentedMessage = "Buffer is not rented.";
        private const string Utf8FormatterFailedMessage = "Utf8Formatter.TryFormat failed.";

        /// <summary>
        /// 从内存池借用内存
        /// </summary>
        /// <param name="length">需要的内存大小</param>
        /// <exception>out of memory</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Rent(System.Int32 length) {
            Data = ArrayPool<byte>.Shared.Rent(length);
            Length = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() {
            if (Data != null) {
                ArrayPool<byte>.Shared.Return(Data);
                Data = null;
                Length = 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> Bytes() {
            if (Data == null || Length == 0) {
                return Span<byte>.Empty;
            }
            return Data.AsSpan(0, Length);
        }

        //const int CodeOfNotRentYet = 255;
        const int CodeOfFormatFail = 254;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Error Extend() {
            /*
            prompt:
            * 目标: 提供一个函数，对存储空间翻倍
            * 过程:
              - 根据 Data 的容量，rent 容量翻倍的一块内存
              - 根据 Length, 把 Data 中的数据复制过去
              - 交换 Data
              -  return 旧的内存块
            */
            // if (Data == null) {
            //     return new Error{Code=CodeOfNotRentYet, Message=BufferNotRentedMessage};
            // }
            byte[] newData = ArrayPool<byte>.Shared.Rent(Data!.Length*2);
            if (Length > 0) {
                Array.Copy(Data, newData, Length);
            }
            ArrayPool<byte>.Shared.Return(Data);
            Data = newData;
            return default(Error);
        }

        /// <summary>
        /// 注意：调用此函数一定会在堆上创建 new string[]{}
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Error Append(params string[] arr) {
            // if (Data == null) {
            //     return new Error{Code=CodeOfNotRentYet, Message=BufferNotRentedMessage};
            // }

            foreach (var s in arr) {
                if (string.IsNullOrEmpty(s)) {
                    continue;
                }

                int byteCount = System.Text.Encoding.UTF8.GetByteCount(s);
                while (Length + byteCount > Data!.Length) {
                    Extend();
                }

                int bytesWritten = System.Text.Encoding.UTF8.GetBytes(s, 0, s.Length, Data, Length);
                Length += bytesWritten;
            }
            return default(Error);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]  
        public Error Append(string s){
            // if (Data == null) {
            //     return new Error{Code=CodeOfNotRentYet, Message=BufferNotRentedMessage};
            // }
            if (string.IsNullOrEmpty(s)) {
                return default(Error);
            }
            int byteCount = System.Text.Encoding.UTF8.GetByteCount(s);
            while (Length + byteCount > Data!.Length) {
                Extend();
            }
            int bytesWritten = System.Text.Encoding.UTF8.GetBytes(s, 0, s.Length, Data, Length);
            Length += bytesWritten;
            return default(Error);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Error Append(byte c) {
            // if (Data == null) {
            //     return new Error{Code=CodeOfNotRentYet, Message=BufferNotRentedMessage};
            // }

            while (Length + 1 > Data!.Length) {
                Extend();
            }

            Data[Length] = c;
            Length++;
            return default(Error);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Error Append(ReadOnlySpan<byte> s) {
            // if (Data == null) {
            //     return new Error{Code=CodeOfNotRentYet, Message=BufferNotRentedMessage};
            // }
            if (s.IsEmpty) {
                return default(Error);
            }
            while (Length + s.Length > Data!.Length) {
                Extend();
            }

            s.CopyTo(Data.AsSpan(Length));
            Length += s.Length;
            return default(Error);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Error Append(long value) {
            // if (Data == null) {
            //     return new Error{Code=CodeOfNotRentYet, Message=BufferNotRentedMessage};
            // }
            const int maxByteCount = 20;
            while (Data!.Length - Length < maxByteCount) {
                Extend();
            }

            if (!System.Buffers.Text.Utf8Formatter.TryFormat(value, Data.AsSpan(Length), out int bytesWritten)) {
                return new Error{Code=CodeOfFormatFail, Message=Utf8FormatterFailedMessage};
            }
            Length += bytesWritten;
            return default(Error);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Error Append(System.UInt64 value) {
            // if (Data == null) {
            //     return new Error{Code=CodeOfNotRentYet, Message=BufferNotRentedMessage};
            // }
            const int maxByteCount = 20;
            while (Data!.Length - Length < maxByteCount) {
                Extend();
            }

            if (!System.Buffers.Text.Utf8Formatter.TryFormat(value, Data.AsSpan(Length), out int bytesWritten)) {
                return new Error{Code=CodeOfFormatFail, Message=Utf8FormatterFailedMessage};
            }
            Length += bytesWritten;
            return default(Error);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Error Append(bool value) {
            // if (Data == null) {
            //     return new Error{Code=CodeOfNotRentYet, Message=BufferNotRentedMessage};
            // }
            const int maxByteCount = 5;
            while (Data!.Length - Length < maxByteCount) {
                Extend();
            }

            if (!System.Buffers.Text.Utf8Formatter.TryFormat(value, Data.AsSpan(Length), out int bytesWritten)) {
                return new Error{Code=CodeOfFormatFail, Message=Utf8FormatterFailedMessage};
            }
            Length += bytesWritten;
            return default(Error);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Error AppendUtcDatetime(DateTime dtm) {
            if (dtm.Kind == DateTimeKind.Local) {
                dtm = dtm.ToUniversalTime();
            } else if (dtm.Kind == DateTimeKind.Unspecified) {
                dtm = DateTime.SpecifyKind(dtm, DateTimeKind.Utc);
            }

            const int bytesNeeded = 28;
            while (Data!.Length - Length < bytesNeeded) {
                Extend();
            }

            var dst = Data.AsSpan(Length, bytesNeeded);

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

            Length += bytesNeeded;
            return default(Error);

            static void Write2(int value, Span<byte> destination, int offset) {
                destination[offset] = (byte)('0' + (value / 10));
                destination[offset + 1] = (byte)('0' + (value % 10));
            }

            static void Write4(int value, Span<byte> destination, int offset) {
                destination[offset + 3] = (byte)('0' + (value % 10));
                value /= 10;
                destination[offset + 2] = (byte)('0' + (value % 10));
                value /= 10;
                destination[offset + 1] = (byte)('0' + (value % 10));
                value /= 10;
                destination[offset] = (byte)('0' + (value % 10));
            }

            static void Write7(int value, Span<byte> destination, int offset) {
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
    }
}
