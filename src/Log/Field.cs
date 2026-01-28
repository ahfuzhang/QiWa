using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Log {
    public enum FieldDataType {
        String,
        Utf8String,
        Bool,
        Int64,
        Uint64,
        Float64,
        DateTime,
        RawJsonString,
        RawJsonUtf8String,
    }

    [StructLayout(LayoutKind.Explicit)]
    public ref struct FieldValue {
        [FieldOffset(0)]
        public bool BoolValue;
        [FieldOffset(0)]
        public long Int64Value;
        [FieldOffset(0)]
        public ulong Uint64Value;
        [FieldOffset(0)]
        public double Float64Value;
        [FieldOffset(0)]
        public DateTime DateTimeValue;
    }

    public ref struct Field {
        public ReadOnlySpan<byte> Name;
        public FieldDataType DataType;

        // For String, SpanByte, RawJsonString, RawJsonSpanByte
        public string StringValue;
        public ReadOnlySpan<byte> Utf8StringValue;

        // For primitive types
        public FieldValue PrimitiveValue;

        public static Field String(ReadOnlySpan<byte> name, string value) {
            return new Field {
                Name = name,
                DataType = FieldDataType.String,
                StringValue = value
            };
        }

        public static Field Utf8String(ReadOnlySpan<byte> name, ReadOnlySpan<byte> value) {
            return new Field {
                Name = name,
                DataType = FieldDataType.Utf8String,
                Utf8StringValue = value
            };
        }

        public static Field Bool(ReadOnlySpan<byte> name, bool value) {
            return new Field {
                Name = name,
                DataType = FieldDataType.Bool,
                PrimitiveValue = new FieldValue { BoolValue = value }
            };
        }

        public static Field Int64(ReadOnlySpan<byte> name, long value) {
            return new Field {
                Name = name,
                DataType = FieldDataType.Int64,
                PrimitiveValue = new FieldValue { Int64Value = value }
            };
        }

        public static Field UInt64(ReadOnlySpan<byte> name, ulong value) {
            return new Field {
                Name = name,
                DataType = FieldDataType.Uint64,
                PrimitiveValue = new FieldValue { Uint64Value = value }
            };
        }

        public static Field Float64(ReadOnlySpan<byte> name, double value) {
            return new Field {
                Name = name,
                DataType = FieldDataType.Float64,
                PrimitiveValue = new FieldValue { Float64Value = value }
            };
        }

        public static Field UtcDateTime(ReadOnlySpan<byte> name, DateTime value) {
            return new Field {
                Name = name,
                DataType = FieldDataType.DateTime,
                PrimitiveValue = new FieldValue { DateTimeValue = value }
            };
        }

        public static Field RawJson(ReadOnlySpan<byte> name, string s) {
            return new Field {
                Name = name,
                DataType = FieldDataType.RawJsonString,
                StringValue = s
            };
        }

        public static Field RawJson(ReadOnlySpan<byte> name, ReadOnlySpan<byte> value) {
            return new Field {
                Name = name,
                DataType = FieldDataType.RawJsonUtf8String,
                Utf8StringValue = value
            };
        }

        /// <summary>
        /// json 序列化到 buffer 中
        /// </summary>
        /// <param name="rent"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteTo(ref Common.RentedBuffer rent) {
            rent.Append((byte)'"');
            rent.Append(Name);
            rent.Append("\":");
            switch (DataType) {
                case FieldDataType.String:
                    rent.Append((byte)'"');
                    rent.AppendAsJsonEscapedString(StringValue);
                    rent.Append((byte)'"');
                    break;
                case FieldDataType.Utf8String:
                    rent.Append((byte)'"');
                    rent.AppendAsJsonEscapedString(Utf8StringValue);
                    rent.Append((byte)'"');
                    break;
                case FieldDataType.RawJsonString:
                    rent.Append(StringValue);
                    break;
                case FieldDataType.RawJsonUtf8String:
                    rent.Append(Utf8StringValue);
                    break;
                case FieldDataType.Bool:
                    rent.Append(PrimitiveValue.BoolValue);
                    break;
                case FieldDataType.Int64:
                    rent.Append(PrimitiveValue.Int64Value);
                    break;
                case FieldDataType.Uint64:
                    rent.Append(PrimitiveValue.Uint64Value);
                    break;
                case FieldDataType.DateTime:
                    rent.AppendUtcDatetime(PrimitiveValue.DateTimeValue);
                    break;
                case FieldDataType.Float64:
                    rent.Append(PrimitiveValue.Float64Value);
                    break;
                default:
                    throw new Exception("not support type");
            }
        }
    }
}
