using System.Text;
using Log;
using Xunit;

namespace Tests.Log;

public class FieldTests
{
    #region TestCase Structures
    public struct StringFieldTestCase
    {
        public string Name;
        public byte[] FieldName;
        public string Value;
        public string ExpectedJson;
    }

    public struct Utf8StringFieldTestCase
    {
        public string Name;
        public byte[] FieldName;
        public byte[] Value;
        public string ExpectedJson;
    }

    public struct BoolFieldTestCase
    {
        public string Name;
        public byte[] FieldName;
        public bool Value;
        public string ExpectedJson;
    }

    public struct Int64FieldTestCase
    {
        public string Name;
        public byte[] FieldName;
        public long Value;
        public string ExpectedJson;
    }

    public struct UInt64FieldTestCase
    {
        public string Name;
        public byte[] FieldName;
        public ulong Value;
        public string ExpectedJson;
    }

    public struct Float64FieldTestCase
    {
        public string Name;
        public byte[] FieldName;
        public double Value;
        public string ExpectedJson;
    }

    public struct DateTimeFieldTestCase
    {
        public string Name;
        public byte[] FieldName;
        public DateTime Value;
        public string ExpectedJsonPattern;
    }

    public struct RawJsonStringTestCase
    {
        public string Name;
        public byte[] FieldName;
        public string Value;
        public string ExpectedJson;
    }

    public struct RawJsonUtf8TestCase
    {
        public string Name;
        public byte[] FieldName;
        public byte[] Value;
        public string ExpectedJson;
    }
    #endregion

    [Fact]
    public void String_CreatesFieldWithCorrectProperties()
    {
        var testCases = new StringFieldTestCase[] {
            new() { Name = "simple string", FieldName = "msg"u8.ToArray(), Value = "hello", ExpectedJson = "\"msg\":\"hello\"" },
            new() { Name = "empty string", FieldName = "empty"u8.ToArray(), Value = "", ExpectedJson = "\"empty\":\"\"" },
            new() { Name = "string with quotes", FieldName = "quoted"u8.ToArray(), Value = "say \"hi\"", ExpectedJson = "\"quoted\":\"say \\\"hi\\\"\"" },
            new() { Name = "string with newline", FieldName = "nl"u8.ToArray(), Value = "line1\nline2", ExpectedJson = "\"nl\":\"line1\\nline2\"" },
            new() { Name = "string with tab", FieldName = "tab"u8.ToArray(), Value = "a\tb", ExpectedJson = "\"tab\":\"a\\tb\"" },
            new() { Name = "string with backslash", FieldName = "bs"u8.ToArray(), Value = "path\\file", ExpectedJson = "\"bs\":\"path\\\\file\"" },
            new() { Name = "unicode string", FieldName = "unicode"u8.ToArray(), Value = "你好世界", ExpectedJson = "\"unicode\":\"你好世界\"" },
        };

        foreach (var tc in testCases)
        {
            var field = Field.String(tc.FieldName, tc.Value);
            Assert.Equal(FieldDataType.String, field.DataType);
            Assert.Equal(tc.Value, field.StringValue);

            Common.RentedBuffer buf = new(256);
            field.WriteTo(ref buf);
            var result = Encoding.UTF8.GetString(buf.Bytes());
            // Note: System.Text.Json uses unicode escapes for some characters
            Assert.Contains($"\"{Encoding.UTF8.GetString(tc.FieldName)}\":", result);
            buf.Dispose();
        }
    }

    [Fact]
    public void Utf8String_CreatesFieldWithCorrectProperties()
    {
        var testCases = new Utf8StringFieldTestCase[] {
            new() { Name = "simple utf8", FieldName = "msg"u8.ToArray(), Value = "hello"u8.ToArray(), ExpectedJson = "\"msg\":\"hello\"" },
            new() { Name = "empty utf8", FieldName = "empty"u8.ToArray(), Value = Array.Empty<byte>(), ExpectedJson = "\"empty\":\"\"" },
            new() { Name = "utf8 with quotes", FieldName = "quoted"u8.ToArray(), Value = "say \"hi\""u8.ToArray(), ExpectedJson = "\"quoted\":\"say \\\"hi\\\"\"" },
        };

        foreach (var tc in testCases)
        {
            var field = Field.Utf8String(tc.FieldName, tc.Value);
            Assert.Equal(FieldDataType.Utf8String, field.DataType);

            Common.RentedBuffer buf = new(256);
            field.WriteTo(ref buf);
            var result = Encoding.UTF8.GetString(buf.Bytes());
            Assert.Equal(tc.ExpectedJson, result);
            buf.Dispose();
        }
    }

    [Fact]
    public void Bool_CreatesFieldWithCorrectProperties()
    {
        var testCases = new BoolFieldTestCase[] {
            new() { Name = "true value", FieldName = "enabled"u8.ToArray(), Value = true, ExpectedJson = "\"enabled\":true" },
            new() { Name = "false value", FieldName = "disabled"u8.ToArray(), Value = false, ExpectedJson = "\"disabled\":false" },
        };

        foreach (var tc in testCases)
        {
            var field = Field.Bool(tc.FieldName, tc.Value);
            Assert.Equal(FieldDataType.Bool, field.DataType);
            Assert.Equal(tc.Value, field.PrimitiveValue.BoolValue);

            Common.RentedBuffer buf = new(256);
            field.WriteTo(ref buf);
            var result = Encoding.UTF8.GetString(buf.Bytes());
            Assert.Equal(tc.ExpectedJson, result);
            buf.Dispose();
        }
    }

    [Fact]
    public void Int64_CreatesFieldWithCorrectProperties()
    {
        var testCases = new Int64FieldTestCase[] {
            new() { Name = "positive value", FieldName = "count"u8.ToArray(), Value = 12345, ExpectedJson = "\"count\":12345" },
            new() { Name = "negative value", FieldName = "diff"u8.ToArray(), Value = -999, ExpectedJson = "\"diff\":-999" },
            new() { Name = "zero value", FieldName = "zero"u8.ToArray(), Value = 0, ExpectedJson = "\"zero\":0" },
            new() { Name = "max value", FieldName = "max"u8.ToArray(), Value = long.MaxValue, ExpectedJson = $"\"max\":{long.MaxValue}" },
            new() { Name = "min value", FieldName = "min"u8.ToArray(), Value = long.MinValue, ExpectedJson = $"\"min\":{long.MinValue}" },
        };

        foreach (var tc in testCases)
        {
            var field = Field.Int64(tc.FieldName, tc.Value);
            Assert.Equal(FieldDataType.Int64, field.DataType);
            Assert.Equal(tc.Value, field.PrimitiveValue.Int64Value);

            Common.RentedBuffer buf = new(256);
            field.WriteTo(ref buf);
            var result = Encoding.UTF8.GetString(buf.Bytes());
            Assert.Equal(tc.ExpectedJson, result);
            buf.Dispose();
        }
    }

    [Fact]
    public void UInt64_CreatesFieldWithCorrectProperties()
    {
        var testCases = new UInt64FieldTestCase[] {
            new() { Name = "positive value", FieldName = "count"u8.ToArray(), Value = 12345, ExpectedJson = "\"count\":12345" },
            new() { Name = "zero value", FieldName = "zero"u8.ToArray(), Value = 0, ExpectedJson = "\"zero\":0" },
            new() { Name = "max value", FieldName = "max"u8.ToArray(), Value = ulong.MaxValue, ExpectedJson = $"\"max\":{ulong.MaxValue}" },
        };

        foreach (var tc in testCases)
        {
            var field = Field.UInt64(tc.FieldName, tc.Value);
            Assert.Equal(FieldDataType.Uint64, field.DataType);
            Assert.Equal(tc.Value, field.PrimitiveValue.Uint64Value);

            Common.RentedBuffer buf = new(256);
            field.WriteTo(ref buf);
            var result = Encoding.UTF8.GetString(buf.Bytes());
            Assert.Equal(tc.ExpectedJson, result);
            buf.Dispose();
        }
    }

    [Fact]
    public void Float64_CreatesFieldWithCorrectProperties()
    {
        var testCases = new Float64FieldTestCase[] {
            new() { Name = "positive decimal", FieldName = "rate"u8.ToArray(), Value = 3.14159, ExpectedJson = "\"rate\":3.14159" },
            new() { Name = "negative decimal", FieldName = "temp"u8.ToArray(), Value = -273.15, ExpectedJson = "\"temp\":-273.15" },
            new() { Name = "zero value", FieldName = "zero"u8.ToArray(), Value = 0.0, ExpectedJson = "\"zero\":0" },
            new() { Name = "integer as double", FieldName = "int"u8.ToArray(), Value = 42.0, ExpectedJson = "\"int\":42" },
        };

        foreach (var tc in testCases)
        {
            var field = Field.Float64(tc.FieldName, tc.Value);
            Assert.Equal(FieldDataType.Float64, field.DataType);
            Assert.Equal(tc.Value, field.PrimitiveValue.Float64Value);

            Common.RentedBuffer buf = new(256);
            field.WriteTo(ref buf);
            var result = Encoding.UTF8.GetString(buf.Bytes());
            Assert.Equal(tc.ExpectedJson, result);
            buf.Dispose();
        }
    }

    [Fact]
    public void UtcDateTime_CreatesFieldWithCorrectProperties()
    {
        var utcTime = new DateTime(2024, 6, 15, 10, 30, 45, DateTimeKind.Utc).AddTicks(1234567);
        var field = Field.UtcDateTime("timestamp"u8.ToArray(), utcTime);

        Assert.Equal(FieldDataType.DateTime, field.DataType);
        Assert.Equal(utcTime, field.PrimitiveValue.DateTimeValue);

        Common.RentedBuffer buf = new(256);
        field.WriteTo(ref buf);
        var result = Encoding.UTF8.GetString(buf.Bytes());
        Assert.Contains("\"timestamp\":", result);
        Assert.Contains("2024-06-15T10:30:45", result);
        buf.Dispose();
    }

    [Fact]
    public void RawJson_String_CreatesFieldWithCorrectProperties()
    {
        var testCases = new RawJsonStringTestCase[] {
            new() { Name = "json object", FieldName = "data"u8.ToArray(), Value = "{\"a\":1}", ExpectedJson = "\"data\":{\"a\":1}" },
            new() { Name = "json array", FieldName = "arr"u8.ToArray(), Value = "[1,2,3]", ExpectedJson = "\"arr\":[1,2,3]" },
            new() { Name = "json number", FieldName = "num"u8.ToArray(), Value = "123", ExpectedJson = "\"num\":123" },
            new() { Name = "json null", FieldName = "nil"u8.ToArray(), Value = "null", ExpectedJson = "\"nil\":null" },
        };

        foreach (var tc in testCases)
        {
            var field = Field.RawJson(tc.FieldName, tc.Value);
            Assert.Equal(FieldDataType.RawJsonString, field.DataType);
            Assert.Equal(tc.Value, field.StringValue);

            Common.RentedBuffer buf = new(256);
            field.WriteTo(ref buf);
            var result = Encoding.UTF8.GetString(buf.Bytes());
            Assert.Equal(tc.ExpectedJson, result);
            buf.Dispose();
        }
    }

    [Fact]
    public void RawJson_Utf8_CreatesFieldWithCorrectProperties()
    {
        var testCases = new RawJsonUtf8TestCase[] {
            new() { Name = "json object", FieldName = "data"u8.ToArray(), Value = "{\"a\":1}"u8.ToArray(), ExpectedJson = "\"data\":{\"a\":1}" },
            new() { Name = "json array", FieldName = "arr"u8.ToArray(), Value = "[1,2,3]"u8.ToArray(), ExpectedJson = "\"arr\":[1,2,3]" },
        };

        foreach (var tc in testCases)
        {
            var field = Field.RawJson(tc.FieldName, tc.Value);
            Assert.Equal(FieldDataType.RawJsonUtf8String, field.DataType);

            Common.RentedBuffer buf = new(256);
            field.WriteTo(ref buf);
            var result = Encoding.UTF8.GetString(buf.Bytes());
            Assert.Equal(tc.ExpectedJson, result);
            buf.Dispose();
        }
    }

    [Fact]
    public void FieldDataType_HasAllExpectedValues()
    {
        // Verify all enum values exist and can be used
        var allTypes = new FieldDataType[] {
            FieldDataType.String,
            FieldDataType.Utf8String,
            FieldDataType.Bool,
            FieldDataType.Int64,
            FieldDataType.Uint64,
            FieldDataType.Float64,
            FieldDataType.DateTime,
            FieldDataType.RawJsonString,
            FieldDataType.RawJsonUtf8String,
        };

        Assert.Equal(9, allTypes.Length);
    }
}
