// <copyright file="FieldTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Tests.ConsoleLogger;

using System;
using System.Text;
using global::ConsoleLogger;
using Xunit;

/// <summary>
/// Unit tests for ConsoleLogger.Field and FieldValue using table-driven approach.
/// </summary>
public class FieldTests : TestBase
{
    #region Test Case Structures

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
        public string ExpectedContains;
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

    #region FieldValue Constructor Tests

    [Fact]
    public void FieldValue_BoolConstructor_SetsCorrectValue()
    {
        var testCases = new[]
        {
            new { Value = true, Expected = true },
            new { Value = false, Expected = false },
        };

        foreach (var tc in testCases)
        {
            var fieldValue = new FieldValue(tc.Value);
            Assert.Equal(tc.Expected, fieldValue.BoolValue);
        }
    }

    [Fact]
    public void FieldValue_Int64Constructor_SetsCorrectValue()
    {
        var testCases = new[]
        {
            new { Value = 0L, Expected = 0L },
            new { Value = 12345L, Expected = 12345L },
            new { Value = -99999L, Expected = -99999L },
            new { Value = long.MaxValue, Expected = long.MaxValue },
            new { Value = long.MinValue, Expected = long.MinValue },
        };

        foreach (var tc in testCases)
        {
            var fieldValue = new FieldValue(tc.Value);
            Assert.Equal(tc.Expected, fieldValue.Int64Value);
        }
    }

    [Fact]
    public void FieldValue_UInt64Constructor_SetsCorrectValue()
    {
        var testCases = new[]
        {
            new { Value = 0UL, Expected = 0UL },
            new { Value = 12345UL, Expected = 12345UL },
            new { Value = ulong.MaxValue, Expected = ulong.MaxValue },
        };

        foreach (var tc in testCases)
        {
            var fieldValue = new FieldValue(tc.Value);
            Assert.Equal(tc.Expected, fieldValue.Uint64Value);
        }
    }

    [Fact]
    public void FieldValue_Float64Constructor_SetsCorrectValue()
    {
        var testCases = new[]
        {
            new { Value = 0.0, Expected = 0.0 },
            new { Value = 3.14159, Expected = 3.14159 },
            new { Value = -273.15, Expected = -273.15 },
            new { Value = double.MaxValue, Expected = double.MaxValue },
            new { Value = double.MinValue, Expected = double.MinValue },
            new { Value = double.Epsilon, Expected = double.Epsilon },
        };

        foreach (var tc in testCases)
        {
            var fieldValue = new FieldValue(tc.Value);
            Assert.Equal(tc.Expected, fieldValue.Float64Value);
        }
    }

    [Fact]
    public void FieldValue_DateTimeConstructor_SetsCorrectValue()
    {
        var testCases = new[]
        {
            new { Value = DateTime.MinValue, Expected = DateTime.MinValue },
            new { Value = DateTime.MaxValue, Expected = DateTime.MaxValue },
            new { Value = new DateTime(2024, 6, 15, 10, 30, 45, DateTimeKind.Utc), Expected = new DateTime(2024, 6, 15, 10, 30, 45, DateTimeKind.Utc) },
            new { Value = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), Expected = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
        };

        foreach (var tc in testCases)
        {
            var fieldValue = new FieldValue(tc.Value);
            Assert.Equal(tc.Expected, fieldValue.DateTimeValue);
        }
    }

    #endregion

    #region Field.String Tests

    [Fact]
    public void String_CreatesFieldWithCorrectProperties()
    {
        var testCases = new StringFieldTestCase[]
        {
            new () { Name = "simple string", FieldName = "msg"u8.ToArray(), Value = "hello", ExpectedJson = "\"msg\":\"hello\"" },
            new () { Name = "empty string", FieldName = "empty"u8.ToArray(), Value = string.Empty, ExpectedJson = "\"empty\":\"\"" },
            new () { Name = "string with quotes", FieldName = "quoted"u8.ToArray(), Value = "say \"hi\"", ExpectedJson = "\"quoted\":\"say \\\"hi\\\"\"" },
            new () { Name = "string with newline", FieldName = "nl"u8.ToArray(), Value = "line1\nline2", ExpectedJson = "\"nl\":\"line1\\nline2\"" },
            new () { Name = "string with tab", FieldName = "tab"u8.ToArray(), Value = "a\tb", ExpectedJson = "\"tab\":\"a\\tb\"" },
            new () { Name = "string with backslash", FieldName = "bs"u8.ToArray(), Value = "path\\file", ExpectedJson = "\"bs\":\"path\\\\file\"" },
            new () { Name = "unicode string", FieldName = "unicode"u8.ToArray(), Value = "你好世界", ExpectedJson = "\"unicode\":\"你好世界\"" },
            new () { Name = "string with carriage return", FieldName = "cr"u8.ToArray(), Value = "line1\rline2", ExpectedJson = "\"cr\":\"line1\\rline2\"" },
            new () { Name = "string with mixed escapes", FieldName = "mix"u8.ToArray(), Value = "a\t\n\\\"b", ExpectedJson = "\"mix\":\"a\\t\\n\\\\\\\"b\"" },
            new () { Name = "long field name", FieldName = "very_long_field_name_with_many_characters"u8.ToArray(), Value = "value", ExpectedJson = "\"very_long_field_name_with_many_characters\":\"value\"" },
            new () { Name = "whitespace only", FieldName = "ws"u8.ToArray(), Value = "   ", ExpectedJson = "\"ws\":\"   \"" },
        };

        foreach (var tc in testCases)
        {
            var field = Field.String(tc.FieldName, tc.Value);

            // Verify field properties
            Assert.Equal(FieldDataType.String, field.DataType);
            Assert.Equal(tc.Value, field.StringValue);
            Assert.True(field.Name.SequenceEqual(tc.FieldName));

            // Verify WriteTo output
            Common.RentedBuffer buf = new(256);
            field.WriteTo(ref buf);
            var result = Encoding.UTF8.GetString(buf.Bytes());
            Assert.Contains($"\"{Encoding.UTF8.GetString(tc.FieldName)}\":", result);
            buf.Dispose();
        }
    }

    [Fact]
    public void String_WithNullValue_CreatesFieldWithNullStringValue()
    {
        var field = Field.String("key"u8, null!);
        Assert.Equal(FieldDataType.String, field.DataType);
        Assert.Null(field.StringValue);
    }

    #endregion

    #region Field.Utf8String Tests

    [Fact]
    public void Utf8String_CreatesFieldWithCorrectProperties()
    {
        var testCases = new Utf8StringFieldTestCase[]
        {
            new () { Name = "simple utf8", FieldName = "msg"u8.ToArray(), Value = "hello"u8.ToArray(), ExpectedJson = "\"msg\":\"hello\"" },
            new () { Name = "empty utf8", FieldName = "empty"u8.ToArray(), Value = Array.Empty<byte>(), ExpectedJson = "\"empty\":\"\"" },
            new () { Name = "utf8 with quotes", FieldName = "quoted"u8.ToArray(), Value = "say \"hi\""u8.ToArray(), ExpectedJson = "\"quoted\":\"say \\\"hi\\\"\"" },
            new () { Name = "utf8 with newline", FieldName = "nl"u8.ToArray(), Value = "line1\nline2"u8.ToArray(), ExpectedJson = "\"nl\":\"line1\\nline2\"" },
            new () { Name = "utf8 with tab", FieldName = "tab"u8.ToArray(), Value = "a\tb"u8.ToArray(), ExpectedJson = "\"tab\":\"a\\tb\"" },
            new () { Name = "utf8 with backslash", FieldName = "bs"u8.ToArray(), Value = "path\\file"u8.ToArray(), ExpectedJson = "\"bs\":\"path\\\\file\"" },
            new () { Name = "utf8 unicode", FieldName = "unicode"u8.ToArray(), Value = "你好世界"u8.ToArray(), ExpectedJson = "\"unicode\":\"你好世界\"" },
        };

        foreach (var tc in testCases)
        {
            var field = Field.Utf8String(tc.FieldName, tc.Value);

            // Verify field properties
            Assert.Equal(FieldDataType.Utf8String, field.DataType);
            Assert.True(field.Utf8StringValue.SequenceEqual(tc.Value));
            Assert.True(field.Name.SequenceEqual(tc.FieldName));

            // Verify WriteTo output
            Common.RentedBuffer buf = new(256);
            field.WriteTo(ref buf);
            var result = Encoding.UTF8.GetString(buf.Bytes());
            Assert.Equal(tc.ExpectedJson, result);
            buf.Dispose();
        }
    }

    #endregion

    #region Field.Bool Tests

    [Fact]
    public void Bool_CreatesFieldWithCorrectProperties()
    {
        var testCases = new BoolFieldTestCase[]
        {
            new () { Name = "true value", FieldName = "enabled"u8.ToArray(), Value = true, ExpectedJson = "\"enabled\":true" },
            new () { Name = "false value", FieldName = "disabled"u8.ToArray(), Value = false, ExpectedJson = "\"disabled\":false" },
        };

        foreach (var tc in testCases)
        {
            var field = Field.Bool(tc.FieldName, tc.Value);

            // Verify field properties
            Assert.Equal(FieldDataType.Bool, field.DataType);
            Assert.Equal(tc.Value, field.PrimitiveValue.BoolValue);
            Assert.True(field.Name.SequenceEqual(tc.FieldName));

            // Verify WriteTo output
            Common.RentedBuffer buf = new(256);
            field.WriteTo(ref buf);
            var result = Encoding.UTF8.GetString(buf.Bytes());
            Assert.Equal(tc.ExpectedJson, result);
            buf.Dispose();
        }
    }

    #endregion

    #region Field.Int64 Tests

    [Fact]
    public void Int64_CreatesFieldWithCorrectProperties()
    {
        var testCases = new Int64FieldTestCase[]
        {
            new () { Name = "positive value", FieldName = "count"u8.ToArray(), Value = 12345, ExpectedJson = "\"count\":12345" },
            new () { Name = "negative value", FieldName = "diff"u8.ToArray(), Value = -999, ExpectedJson = "\"diff\":-999" },
            new () { Name = "zero value", FieldName = "zero"u8.ToArray(), Value = 0, ExpectedJson = "\"zero\":0" },
            new () { Name = "max value", FieldName = "max"u8.ToArray(), Value = long.MaxValue, ExpectedJson = $"\"max\":{long.MaxValue}" },
            new () { Name = "min value", FieldName = "min"u8.ToArray(), Value = long.MinValue, ExpectedJson = $"\"min\":{long.MinValue}" },
            new () { Name = "large positive", FieldName = "big"u8.ToArray(), Value = 9999999999999L, ExpectedJson = "\"big\":9999999999999" },
            new () { Name = "large negative", FieldName = "neg"u8.ToArray(), Value = -9999999999999L, ExpectedJson = "\"neg\":-9999999999999" },
        };

        foreach (var tc in testCases)
        {
            var field = Field.Int64(tc.FieldName, tc.Value);

            // Verify field properties
            Assert.Equal(FieldDataType.Int64, field.DataType);
            Assert.Equal(tc.Value, field.PrimitiveValue.Int64Value);
            Assert.True(field.Name.SequenceEqual(tc.FieldName));

            // Verify WriteTo output
            Common.RentedBuffer buf = new(256);
            field.WriteTo(ref buf);
            var result = Encoding.UTF8.GetString(buf.Bytes());
            Assert.Equal(tc.ExpectedJson, result);
            buf.Dispose();
        }
    }

    #endregion

    #region Field.UInt64 Tests

    [Fact]
    public void UInt64_CreatesFieldWithCorrectProperties()
    {
        var testCases = new UInt64FieldTestCase[]
        {
            new () { Name = "positive value", FieldName = "count"u8.ToArray(), Value = 12345, ExpectedJson = "\"count\":12345" },
            new () { Name = "zero value", FieldName = "zero"u8.ToArray(), Value = 0, ExpectedJson = "\"zero\":0" },
            new () { Name = "max value", FieldName = "max"u8.ToArray(), Value = ulong.MaxValue, ExpectedJson = $"\"max\":{ulong.MaxValue}" },
            new () { Name = "one", FieldName = "one"u8.ToArray(), Value = 1, ExpectedJson = "\"one\":1" },
            new () { Name = "large value", FieldName = "large"u8.ToArray(), Value = 18446744073709551615UL, ExpectedJson = "\"large\":18446744073709551615" },
        };

        foreach (var tc in testCases)
        {
            var field = Field.UInt64(tc.FieldName, tc.Value);

            // Verify field properties
            Assert.Equal(FieldDataType.Uint64, field.DataType);
            Assert.Equal(tc.Value, field.PrimitiveValue.Uint64Value);
            Assert.True(field.Name.SequenceEqual(tc.FieldName));

            // Verify WriteTo output
            Common.RentedBuffer buf = new(256);
            field.WriteTo(ref buf);
            var result = Encoding.UTF8.GetString(buf.Bytes());
            Assert.Equal(tc.ExpectedJson, result);
            buf.Dispose();
        }
    }

    #endregion

    #region Field.Float64 Tests

    [Fact]
    public void Float64_CreatesFieldWithCorrectProperties()
    {
        var testCases = new Float64FieldTestCase[]
        {
            new () { Name = "positive decimal", FieldName = "rate"u8.ToArray(), Value = 3.14159, ExpectedJson = "\"rate\":3.14159" },
            new () { Name = "negative decimal", FieldName = "temp"u8.ToArray(), Value = -273.15, ExpectedJson = "\"temp\":-273.15" },
            new () { Name = "zero value", FieldName = "zero"u8.ToArray(), Value = 0.0, ExpectedJson = "\"zero\":0" },
            new () { Name = "integer as double", FieldName = "int"u8.ToArray(), Value = 42.0, ExpectedJson = "\"int\":42" },
            new () { Name = "small decimal", FieldName = "small"u8.ToArray(), Value = 0.001, ExpectedJson = "\"small\":0.001" },
            new () { Name = "negative zero", FieldName = "nz"u8.ToArray(), Value = -0.0, ExpectedJson = "\"nz\":-0" },
        };

        foreach (var tc in testCases)
        {
            var field = Field.Float64(tc.FieldName, tc.Value);

            // Verify field properties
            Assert.Equal(FieldDataType.Float64, field.DataType);
            Assert.Equal(tc.Value, field.PrimitiveValue.Float64Value);
            Assert.True(field.Name.SequenceEqual(tc.FieldName));

            // Verify WriteTo output
            Common.RentedBuffer buf = new(256);
            field.WriteTo(ref buf);
            var result = Encoding.UTF8.GetString(buf.Bytes());
            Assert.Equal(tc.ExpectedJson, result);
            buf.Dispose();
        }
    }

    [Fact]
    public void Float64_SpecialValues_HandledCorrectly()
    {
        // Test special floating point values
        var positiveInfinity = Field.Float64("pinf"u8, double.PositiveInfinity);
        Assert.Equal(FieldDataType.Float64, positiveInfinity.DataType);
        Assert.Equal(double.PositiveInfinity, positiveInfinity.PrimitiveValue.Float64Value);

        var negativeInfinity = Field.Float64("ninf"u8, double.NegativeInfinity);
        Assert.Equal(FieldDataType.Float64, negativeInfinity.DataType);
        Assert.Equal(double.NegativeInfinity, negativeInfinity.PrimitiveValue.Float64Value);

        var nan = Field.Float64("nan"u8, double.NaN);
        Assert.Equal(FieldDataType.Float64, nan.DataType);
        Assert.True(double.IsNaN(nan.PrimitiveValue.Float64Value));
    }

    #endregion

    #region Field.UtcDateTime Tests

    [Fact]
    public void UtcDateTime_CreatesFieldWithCorrectProperties()
    {
        var testCases = new DateTimeFieldTestCase[]
        {
            new ()
            {
                Name = "standard UTC time",
                FieldName = "timestamp"u8.ToArray(),
                Value = new DateTime(2024, 6, 15, 10, 30, 45, DateTimeKind.Utc).AddTicks(1234567),
                ExpectedContains = "2024-06-15T10:30:45",
            },
            new ()
            {
                Name = "midnight UTC",
                FieldName = "midnight"u8.ToArray(),
                Value = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ExpectedContains = "2024-01-01T00:00:00",
            },
            new ()
            {
                Name = "end of day UTC",
                FieldName = "endofday"u8.ToArray(),
                Value = new DateTime(2024, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                ExpectedContains = "2024-12-31T23:59:59",
            },
            new ()
            {
                Name = "year 2000",
                FieldName = "y2k"u8.ToArray(),
                Value = new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                ExpectedContains = "2000-01-01T12:00:00",
            },
        };

        foreach (var tc in testCases)
        {
            var field = Field.UtcDateTime(tc.FieldName, tc.Value);

            // Verify field properties
            Assert.Equal(FieldDataType.DateTime, field.DataType);
            Assert.Equal(tc.Value, field.PrimitiveValue.DateTimeValue);
            Assert.True(field.Name.SequenceEqual(tc.FieldName));

            // Verify WriteTo output
            Common.RentedBuffer buf = new(256);
            field.WriteTo(ref buf);
            var result = Encoding.UTF8.GetString(buf.Bytes());
            Assert.Contains($"\"{Encoding.UTF8.GetString(tc.FieldName)}\":", result);
            Assert.Contains(tc.ExpectedContains, result);
            Assert.Contains("Z", result); // UTC marker
            buf.Dispose();
        }
    }

    [Fact]
    public void UtcDateTime_WithLocalTime_HandledCorrectly()
    {
        var localTime = new DateTime(2024, 6, 15, 10, 30, 45, DateTimeKind.Local);
        var field = Field.UtcDateTime("local"u8.ToArray(), localTime);

        Assert.Equal(FieldDataType.DateTime, field.DataType);
        Assert.Equal(localTime, field.PrimitiveValue.DateTimeValue);

        Common.RentedBuffer buf = new(256);
        field.WriteTo(ref buf);
        var result = Encoding.UTF8.GetString(buf.Bytes());
        Assert.Contains("\"local\":", result);
        Assert.Contains("Z", result);
        buf.Dispose();
    }

    [Fact]
    public void UtcDateTime_WithUnspecifiedTime_HandledCorrectly()
    {
        var unspecifiedTime = new DateTime(2024, 6, 15, 10, 30, 45, DateTimeKind.Unspecified);
        var field = Field.UtcDateTime("unspec"u8.ToArray(), unspecifiedTime);

        Assert.Equal(FieldDataType.DateTime, field.DataType);

        Common.RentedBuffer buf = new(256);
        field.WriteTo(ref buf);
        var result = Encoding.UTF8.GetString(buf.Bytes());
        Assert.Contains("\"unspec\":", result);
        Assert.Contains("Z", result);
        buf.Dispose();
    }

    #endregion

    #region Field.RawJson (String) Tests

    [Fact]
    public void RawJson_String_CreatesFieldWithCorrectProperties()
    {
        var testCases = new RawJsonStringTestCase[]
        {
            new () { Name = "json object", FieldName = "data"u8.ToArray(), Value = "{\"a\":1}", ExpectedJson = "\"data\":{\"a\":1}" },
            new () { Name = "json array", FieldName = "arr"u8.ToArray(), Value = "[1,2,3]", ExpectedJson = "\"arr\":[1,2,3]" },
            new () { Name = "json number", FieldName = "num"u8.ToArray(), Value = "123", ExpectedJson = "\"num\":123" },
            new () { Name = "json null", FieldName = "nil"u8.ToArray(), Value = "null", ExpectedJson = "\"nil\":null" },
            new () { Name = "json boolean true", FieldName = "bool"u8.ToArray(), Value = "true", ExpectedJson = "\"bool\":true" },
            new () { Name = "json boolean false", FieldName = "boolF"u8.ToArray(), Value = "false", ExpectedJson = "\"boolF\":false" },
            new () { Name = "json string", FieldName = "str"u8.ToArray(), Value = "\"hello\"", ExpectedJson = "\"str\":\"hello\"" },
            new () { Name = "nested json object", FieldName = "nested"u8.ToArray(), Value = "{\"a\":{\"b\":2}}", ExpectedJson = "\"nested\":{\"a\":{\"b\":2}}" },
            new () { Name = "empty json object", FieldName = "empty"u8.ToArray(), Value = "{}", ExpectedJson = "\"empty\":{}" },
            new () { Name = "empty json array", FieldName = "emptyArr"u8.ToArray(), Value = "[]", ExpectedJson = "\"emptyArr\":[]" },
        };

        foreach (var tc in testCases)
        {
            var field = Field.RawJson(tc.FieldName, tc.Value);

            // Verify field properties
            Assert.Equal(FieldDataType.RawJsonString, field.DataType);
            Assert.Equal(tc.Value, field.StringValue);
            Assert.True(field.Name.SequenceEqual(tc.FieldName));

            // Verify WriteTo output
            Common.RentedBuffer buf = new(256);
            field.WriteTo(ref buf);
            var result = Encoding.UTF8.GetString(buf.Bytes());
            Assert.Equal(tc.ExpectedJson, result);
            buf.Dispose();
        }
    }

    #endregion

    #region Field.RawJson (Utf8) Tests

    [Fact]
    public void RawJson_Utf8_CreatesFieldWithCorrectProperties()
    {
        var testCases = new RawJsonUtf8TestCase[]
        {
            new () { Name = "json object", FieldName = "data"u8.ToArray(), Value = "{\"a\":1}"u8.ToArray(), ExpectedJson = "\"data\":{\"a\":1}" },
            new () { Name = "json array", FieldName = "arr"u8.ToArray(), Value = "[1,2,3]"u8.ToArray(), ExpectedJson = "\"arr\":[1,2,3]" },
            new () { Name = "json number", FieldName = "num"u8.ToArray(), Value = "456"u8.ToArray(), ExpectedJson = "\"num\":456" },
            new () { Name = "json null", FieldName = "nil"u8.ToArray(), Value = "null"u8.ToArray(), ExpectedJson = "\"nil\":null" },
            new () { Name = "empty object", FieldName = "empty"u8.ToArray(), Value = "{}"u8.ToArray(), ExpectedJson = "\"empty\":{}" },
            new () { Name = "empty array", FieldName = "emptyArr"u8.ToArray(), Value = "[]"u8.ToArray(), ExpectedJson = "\"emptyArr\":[]" },
            new () { Name = "nested object", FieldName = "nest"u8.ToArray(), Value = "{\"a\":{\"b\":1}}"u8.ToArray(), ExpectedJson = "\"nest\":{\"a\":{\"b\":1}}" },
        };

        foreach (var tc in testCases)
        {
            var field = Field.RawJson(tc.FieldName, tc.Value);

            // Verify field properties
            Assert.Equal(FieldDataType.RawJsonUtf8String, field.DataType);
            Assert.True(field.Utf8StringValue.SequenceEqual(tc.Value));
            Assert.True(field.Name.SequenceEqual(tc.FieldName));

            // Verify WriteTo output
            Common.RentedBuffer buf = new(256);
            field.WriteTo(ref buf);
            var result = Encoding.UTF8.GetString(buf.Bytes());
            Assert.Equal(tc.ExpectedJson, result);
            buf.Dispose();
        }
    }

    #endregion

    #region FieldDataType Enum Tests

    [Fact]
    public void FieldDataType_HasAllExpectedValues()
    {
        // Verify all enum values exist and can be used
        var allTypes = new FieldDataType[]
        {
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

        // Verify each enum value is distinct
        var distinctTypes = new HashSet<FieldDataType>(allTypes);
        Assert.Equal(9, distinctTypes.Count);
    }

    [Fact]
    public void FieldDataType_ValuesAreCorrect()
    {
        Assert.Equal(0, (int)FieldDataType.String);
        Assert.Equal(1, (int)FieldDataType.Utf8String);
        Assert.Equal(2, (int)FieldDataType.Bool);
        Assert.Equal(3, (int)FieldDataType.Int64);
        Assert.Equal(4, (int)FieldDataType.Uint64);
        Assert.Equal(5, (int)FieldDataType.Float64);
        Assert.Equal(6, (int)FieldDataType.DateTime);
        Assert.Equal(7, (int)FieldDataType.RawJsonString);
        Assert.Equal(8, (int)FieldDataType.RawJsonUtf8String);
    }

    #endregion

    #region WriteTo Tests - Edge Cases

    [Fact]
    public void WriteTo_WithSmallBuffer_ExtendsCorrectly()
    {
        // Start with a very small buffer to test extension
        var field = Field.String("message"u8, "This is a longer message that should trigger buffer extension");

        Common.RentedBuffer buf = new(10); // Small initial size
        field.WriteTo(ref buf);
        var result = Encoding.UTF8.GetString(buf.Bytes());
        Assert.Contains("\"message\":", result);
        Assert.Contains("This is a longer message", result);
        buf.Dispose();
    }

    [Fact]
    public void WriteTo_MultipleFields_WorksCorrectly()
    {
        Common.RentedBuffer buf = new(256);

        var field1 = Field.String("name"u8, "test");
        field1.WriteTo(ref buf);
        buf.Append(",");

        var field2 = Field.Int64("count"u8, 42);
        field2.WriteTo(ref buf);
        buf.Append(",");

        var field3 = Field.Bool("active"u8, true);
        field3.WriteTo(ref buf);

        var result = Encoding.UTF8.GetString(buf.Bytes());
        Assert.Contains("\"name\":\"test\"", result);
        Assert.Contains("\"count\":42", result);
        Assert.Contains("\"active\":true", result);
        buf.Dispose();
    }

    [Fact]
    public void WriteTo_AllFieldTypes_ProducesValidJson()
    {
        // Create a complete JSON object with all field types
        Common.RentedBuffer buf = new(512);
        buf.Append("{");

        var fields = new (Action writeField, string expectedContains)[]
        {
            (() => Field.String("str"u8, "hello").WriteTo(ref buf), "\"str\":\"hello\""),
            (() => Field.Utf8String("utf8"u8, "world"u8).WriteTo(ref buf), "\"utf8\":\"world\""),
            (() => Field.Bool("bool"u8, true).WriteTo(ref buf), "\"bool\":true"),
            (() => Field.Int64("int"u8, 123).WriteTo(ref buf), "\"int\":123"),
            (() => Field.UInt64("uint"u8, 456UL).WriteTo(ref buf), "\"uint\":456"),
            (() => Field.Float64("float"u8, 3.14).WriteTo(ref buf), "\"float\":3.14"),
            (() => Field.RawJson("raw"u8, "{\"nested\":true}").WriteTo(ref buf), "\"raw\":{\"nested\":true}"),
        };

        for (int i = 0; i < fields.Length; i++)
        {
            fields[i].writeField();
            if (i < fields.Length - 1)
            {
                buf.Append(",");
            }
        }

        buf.Append("}");

        var result = Encoding.UTF8.GetString(buf.Bytes());
        foreach (var (_, expected) in fields)
        {
            Assert.Contains(expected, result);
        }

        buf.Dispose();
    }

    #endregion

    #region Field Name Tests

    [Fact]
    public void Field_WithEmptyName_WorksCorrectly()
    {
        var field = Field.String(Array.Empty<byte>(), "value");
        Assert.Equal(FieldDataType.String, field.DataType);
        Assert.True(field.Name.IsEmpty);

        Common.RentedBuffer buf = new(64);
        field.WriteTo(ref buf);
        var result = Encoding.UTF8.GetString(buf.Bytes());
        Assert.Equal("\"\":\"value\"", result);
        buf.Dispose();
    }

    [Fact]
    public void Field_WithSpecialCharactersInName_WorksCorrectly()
    {
        var testCases = new[]
        {
            new { FieldName = "field_with_underscore"u8.ToArray(), Value = "v1" },
            new { FieldName = "field-with-dash"u8.ToArray(), Value = "v2" },
            new { FieldName = "field.with.dot"u8.ToArray(), Value = "v3" },
            new { FieldName = "field123"u8.ToArray(), Value = "v4" },
        };

        foreach (var tc in testCases)
        {
            var field = Field.String(tc.FieldName, tc.Value);
            Common.RentedBuffer buf = new(128);
            field.WriteTo(ref buf);
            var result = Encoding.UTF8.GetString(buf.Bytes());
            Assert.Contains($"\"{Encoding.UTF8.GetString(tc.FieldName)}\":", result);
            buf.Dispose();
        }
    }

    #endregion
}
