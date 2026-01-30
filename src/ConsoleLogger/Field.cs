namespace ConsoleLogger
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public enum FieldDataType
    {
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
    public readonly ref struct FieldValue
    {
        [FieldOffset(0)]
        public readonly bool BoolValue;
        [FieldOffset(0)]
        public readonly long Int64Value;
        [FieldOffset(0)]
        public readonly ulong Uint64Value;
        [FieldOffset(0)]
        public readonly double Float64Value;
        [FieldOffset(0)]
        public readonly DateTime DateTimeValue;

        public FieldValue(bool v)
        {
            this.BoolValue = v;
        }

        public FieldValue(long v)
        {
            this.Int64Value = v;
        }

        public FieldValue(ulong v)
        {
            this.Uint64Value = v;
        }

        public FieldValue(double v)
        {
            this.Float64Value = v;
        }

        public FieldValue(DateTime v)
        {
            this.DateTimeValue = v;
        }
    }

    public readonly ref struct Field
    {
        public readonly ReadOnlySpan<byte> Name;
        public readonly FieldDataType DataType;

        // For String, SpanByte, RawJsonString, RawJsonSpanByte
        public readonly string StringValue;
        public readonly ReadOnlySpan<byte> Utf8StringValue;

        // For primitive types
        public readonly FieldValue PrimitiveValue;

        public static Field String(ReadOnlySpan<byte> name, string value)
        {
            return new Field(name, FieldDataType.String, value);
        }

        private Field(ReadOnlySpan<byte> name, FieldDataType t, string s)
        {
            this.Name = name;
            this.DataType = t;
            this.StringValue = s;
        }

        public static Field Utf8String(ReadOnlySpan<byte> name, ReadOnlySpan<byte> value)
        {
            return new Field(name, FieldDataType.Utf8String, value);
        }

        private Field(ReadOnlySpan<byte> name, FieldDataType t, ReadOnlySpan<byte> value)
        {
            this.Name = name;
            this.DataType = t;
            this.Utf8StringValue = value;
            this.StringValue = "";
        }

        public static Field Bool(ReadOnlySpan<byte> name, bool value)
        {
            return new Field(name, FieldDataType.Bool, new FieldValue(value));
        }

        private Field(ReadOnlySpan<byte> name, FieldDataType t, FieldValue value)
        {
            this.Name = name;
            this.DataType = t;
            this.PrimitiveValue = value;
            this.StringValue = "";
        }

        public static Field Int64(ReadOnlySpan<byte> name, long value)
        {
            return new Field(name, FieldDataType.Int64, new FieldValue(value));
        }

        public static Field UInt64(ReadOnlySpan<byte> name, ulong value)
        {
            return new Field(name, FieldDataType.Uint64, new FieldValue(value));
        }

        public static Field Float64(ReadOnlySpan<byte> name, double value)
        {
            return new Field(name, FieldDataType.Float64, new FieldValue(value));
        }

        public static Field UtcDateTime(ReadOnlySpan<byte> name, DateTime value)
        {
            return new Field(name, FieldDataType.DateTime, new FieldValue(value));
        }

        public static Field RawJson(ReadOnlySpan<byte> name, string s)
        {
            return new Field(name, FieldDataType.RawJsonString, s);
        }

        public static Field RawJson(ReadOnlySpan<byte> name, ReadOnlySpan<byte> value)
        {
            return new Field(name, FieldDataType.RawJsonUtf8String, value);
        }

        /// <summary>
        /// json 序列化到 buffer 中.
        /// </summary>
        /// <param name="rent"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteTo(ref Common.RentedBuffer rent)
        {
            rent.Append((byte)'"');
            rent.Append(this.Name);
            rent.Append("\":");
            switch (this.DataType)
            {
                case FieldDataType.String:
                    rent.Append((byte)'"');
                    // todo: 如果内容为空，应该跳过这个字段
                    rent.AppendAsJsonEscapedString(this.StringValue);
                    rent.Append((byte)'"');
                    break;
                case FieldDataType.Utf8String:
                    rent.Append((byte)'"');
                    rent.AppendAsJsonEscapedString(this.Utf8StringValue);
                    rent.Append((byte)'"');
                    break;
                case FieldDataType.RawJsonString:
                    rent.Append(this.StringValue);
                    break;
                case FieldDataType.RawJsonUtf8String:
                    rent.Append(this.Utf8StringValue);
                    break;
                case FieldDataType.Bool:
                    rent.Append(this.PrimitiveValue.BoolValue);
                    break;
                case FieldDataType.Int64:
                    rent.Append(this.PrimitiveValue.Int64Value);
                    break;
                case FieldDataType.Uint64:
                    rent.Append(this.PrimitiveValue.Uint64Value);
                    break;
                case FieldDataType.DateTime:
                    rent.AppendUtcDatetime(this.PrimitiveValue.DateTimeValue);
                    break;
                case FieldDataType.Float64:
                    rent.Append(this.PrimitiveValue.Float64Value);
                    break;
                default:
                    throw new Exception("not support type");
            }
        }
    }
}
