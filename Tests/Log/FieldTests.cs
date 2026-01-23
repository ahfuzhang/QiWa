using System;
using System.Globalization;
using System.Text;
using Xunit;
using Log;

public class LogFieldFactoryTests {
    [Fact]
    public void String_SetsNameTypeAndValue() {
        var field = Field.String("msg"u8, "hello");

        Assert.Equal(FieldDataType.String, field.DataType);
        Assert.Equal("msg", Encoding.UTF8.GetString(field.Name));
        Assert.Equal("hello", field.StringValue);
    }

    [Fact]
    public void Utf8String_SetsNameTypeAndValue() {
        var field = Field.Utf8String("msg"u8, "hello"u8);

        Assert.Equal(FieldDataType.Utf8String, field.DataType);
        Assert.Equal("msg", Encoding.UTF8.GetString(field.Name));
        Assert.Equal("hello", Encoding.UTF8.GetString(field.Utf8StringValue));
    }

    [Fact]
    public void Bool_SetsNameTypeAndValue() {
        var field = Field.Bool("flag"u8, true);

        Assert.Equal(FieldDataType.Bool, field.DataType);
        Assert.Equal("flag", Encoding.UTF8.GetString(field.Name));
        Assert.True(field.PrimitiveValue.BoolValue);
    }

    [Fact]
    public void Int64_SetsNameTypeAndValue() {
        var field = Field.Int64("count"u8, -42);

        Assert.Equal(FieldDataType.Int64, field.DataType);
        Assert.Equal("count", Encoding.UTF8.GetString(field.Name));
        Assert.Equal(-42, field.PrimitiveValue.Int64Value);
    }

    [Fact]
    public void UInt64_SetsNameTypeAndValue() {
        var field = Field.UInt64("count"u8, 42);

        Assert.Equal(FieldDataType.Uint64, field.DataType);
        Assert.Equal("count", Encoding.UTF8.GetString(field.Name));
        Assert.Equal((ulong)42, field.PrimitiveValue.Uint64Value);
    }

    [Fact]
    public void Float64_SetsNameTypeAndValue() {
        var field = Field.Float64("ratio"u8, 12.5);

        Assert.Equal(FieldDataType.Float64, field.DataType);
        Assert.Equal("ratio", Encoding.UTF8.GetString(field.Name));
        Assert.Equal(12.5, field.PrimitiveValue.Float64Value);
    }

    [Fact]
    public void UtcDateTime_SetsNameTypeAndValue() {
        var dtm = new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc);
        var field = Field.UtcDateTime("time"u8, dtm);

        Assert.Equal(FieldDataType.DateTime, field.DataType);
        Assert.Equal("time", Encoding.UTF8.GetString(field.Name));
        Assert.Equal(dtm, field.PrimitiveValue.DateTimeValue);
    }

    [Fact]
    public void RawJsonString_SetsNameTypeAndValue() {
        var field = Field.RawJson("payload"u8, "{\"a\":1}");

        Assert.Equal(FieldDataType.RawJsonString, field.DataType);
        Assert.Equal("payload", Encoding.UTF8.GetString(field.Name));
        Assert.Equal("{\"a\":1}", field.StringValue);
    }

    [Fact]
    public void RawJsonUtf8String_SetsNameTypeAndValue() {
        var field = Field.RawJson("payload"u8, "{\"a\":1}"u8);

        Assert.Equal(FieldDataType.RawJsonUtf8String, field.DataType);
        Assert.Equal("payload", Encoding.UTF8.GetString(field.Name));
        Assert.Equal("{\"a\":1}", Encoding.UTF8.GetString(field.Utf8StringValue));
    }
}

public class LogFieldWriteToTests {
    [Fact]
    public void WriteTo_String_EscapesAndQuotes() {
        var field = Field.String("msg"u8, "a\"b\n");

        var text = WriteToString(ref field);

        Assert.Equal("\"msg\":\"a\\u0022b\\n\"", text);
    }

    [Fact]
    public void WriteTo_Utf8String_EscapesAndQuotes() {
        var field = Field.Utf8String("msg"u8, "a\tb\n\"\\c"u8);

        var text = WriteToString(ref field);

        Assert.Equal("\"msg\":\"a\\tb\\n\\\"\\\\c\"", text);
    }

    [Fact]
    public void WriteTo_Bool_WritesLiteral() {
        var field = Field.Bool("flag"u8, true);

        var text = WriteToString(ref field);

        Assert.Equal("\"flag\":true", text);
    }

    [Fact]
    public void WriteTo_Int64_WritesLiteral() {
        var field = Field.Int64("count"u8, -42);

        var text = WriteToString(ref field);

        Assert.Equal("\"count\":-42", text);
    }

    [Fact]
    public void WriteTo_UInt64_WritesLiteral() {
        var field = Field.UInt64("count"u8, 42);

        var text = WriteToString(ref field);

        Assert.Equal("\"count\":42", text);
    }

    [Fact]
    public void WriteTo_Float64_WritesLiteral() {
        var value = 123.5;
        var field = Field.Float64("ratio"u8, value);

        var text = WriteToString(ref field);

        var expected = "\"ratio\":" + value.ToString(CultureInfo.InvariantCulture);
        Assert.Equal(expected, text);
    }

    [Fact]
    public void WriteTo_DateTime_WritesUtcFormat() {
        var dtm = new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc);
        var field = Field.UtcDateTime("time"u8, dtm);

        var text = WriteToString(ref field);

        Assert.Equal("\"time\":2024-01-02T03:04:05.0000000Z", text);
    }

    [Fact]
    public void WriteTo_RawJsonString_WritesRaw() {
        var field = Field.RawJson("payload"u8, "{\"a\":1}");

        var text = WriteToString(ref field);

        Assert.Equal("\"payload\":{\"a\":1}", text);
    }

    [Fact]
    public void WriteTo_RawJsonUtf8String_WritesRaw() {
        var field = Field.RawJson("payload"u8, "{\"a\":1}"u8);

        var text = WriteToString(ref field);

        Assert.Equal("\"payload\":{\"a\":1}", text);
    }

    [Fact]
    public void WriteTo_UnsupportedType_Throws() {
        var ex = Assert.Throws<Exception>(WriteUnsupportedType);
        Assert.Equal("not support type", ex.Message);
    }

    private static string WriteToString(ref Field field) {
        Common.RentedBuffer buffer = default;
        buffer.Rent(128);
        try {
            field.WriteTo(ref buffer);
            return Encoding.UTF8.GetString(buffer.Bytes());
        } finally {
            buffer.Dispose();
        }
    }

    private static void WriteUnsupportedType() {
        Field field = new Field {
            Name = "bad"u8,
            DataType = (FieldDataType)123
        };
        Common.RentedBuffer buffer = default;
        buffer.Rent(16);
        try {
            field.WriteTo(ref buffer);
        } finally {
            buffer.Dispose();
        }
    }
}
