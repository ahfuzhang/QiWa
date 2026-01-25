
using System.Runtime.CompilerServices;
using Xunit;

public class ScopeGuardTests {
    [Fact]
    public void UseScope() {
        var notCleanup = true;
        {
            using (var _ = new Common.ScopeGuard(() => { notCleanup = false; })) {
                Console.WriteLine("\ttest output: biz logic");
            }
        }
        Assert.False(notCleanup);
    }
}

public class ErrorTests {
    [Fact]
    public void HasError() {
        Common.Error err = default;
        Assert.False(err.Err());
        Common.Error err1 = new Common.Error { Code = 1, Message = "err happend" };
        Assert.True(err1.Err());
    }
}

public class RentedBuffer {
    [Fact]
    public void Rent() {
        Common.RentedBuffer buffer = default;
        var span1 = buffer.Bytes();
        Assert.Equal(0, span1.Length);
        int bytes = System.Random.Shared.Next(100, 63336);
        buffer.Rent(bytes);
        Assert.NotNull(buffer.Data);
        Assert.True(buffer.Data.Length >= bytes);
        Assert.Equal(0, buffer.Length);
        ReadOnlySpan<byte> src = "hello\n"u8;
        src.CopyTo(buffer.Data);
        Assert.Equal(0, buffer.Bytes().Length);
        buffer.Length = src.Length;
        var span2 = buffer.Bytes();
        Assert.Equal(buffer.Length, span2.Length);
        Assert.Equal((byte)'h', span2[0]);
        buffer.Dispose();
        Assert.Null(buffer.Data);
        Assert.Equal<int>(0, buffer.Length);
    }

    [Fact]
    public void Extend_DoublesCapacityAndPreservesData() {
        Common.RentedBuffer buffer = default;
        buffer.Rent(8);
        Assert.NotNull(buffer.Data);
        int usedLength = 5;
        for (int i = 0; i < usedLength; i++) {
            buffer.Data[i] = (byte)('a' + i);
        }
        buffer.Length = usedLength;

        byte[] oldData = buffer.Data;
        int oldCapacity = buffer.Data.Length;

        buffer.Extend(oldCapacity);

        Assert.NotNull(buffer.Data);
        Assert.True(buffer.Data.Length >= oldCapacity * 2);
        Assert.False(object.ReferenceEquals(oldData, buffer.Data));
        Assert.Equal(usedLength, buffer.Length);
        for (int i = 0; i < usedLength; i++) {
            Assert.Equal((byte)('a' + i), buffer.Data[i]);
        }

        buffer.Dispose();
    }



    [Fact]
    public void Append_AddsStringsCorrectly() {
        Common.RentedBuffer buffer = default;
        buffer.Rent(10);

        // Append simple string
        buffer.Append("Hello");
        var span = buffer.Bytes();
        Assert.Equal(5, span.Length);
        Assert.Equal("Hello", System.Text.Encoding.UTF8.GetString(span));

        // Append multiple strings
        buffer.Append(", ", "World!");
        span = buffer.Bytes();
        Assert.Equal(13, span.Length);
        Assert.Equal("Hello, World!", System.Text.Encoding.UTF8.GetString(span));

        // Append to trigger extension
        // Current capacity is likely 10 or close to it. "Hello, World!" is 13 bytes? 
        // Wait, Rent(10) means capacity >= 10. 
        // 13 > 10, so it should have extended already.
        // Let's verify capacity.
        Assert.True(buffer.Data!.Length >= 13);

        string longString = new string('a', 100);
        buffer.Append(longString);
        span = buffer.Bytes();
        Assert.Equal(113, span.Length);
        Assert.EndsWith(longString, System.Text.Encoding.UTF8.GetString(span));

        buffer.Dispose();
    }

    [Fact]
    public void Append_AddsByteCorrectly() {
        Common.RentedBuffer buffer = default;
        buffer.Rent(1);

        buffer.Append((byte)'A');
        var span = buffer.Bytes();
        Assert.Equal(1, span.Length);
        Assert.Equal((byte)'A', span[0]);

        // Trigger extension
        buffer.Append((byte)'B');
        span = buffer.Bytes();
        Assert.Equal(2, span.Length);
        Assert.Equal((byte)'A', span[0]);
        Assert.Equal((byte)'B', span[1]);

        buffer.Dispose();
    }

    [Fact]
    public void Append_AddsInt64Correctly() {
        Common.RentedBuffer buffer = default;
        buffer.Rent(10);

        long val1 = 1234567890;
        buffer.Append(val1);
        var span = buffer.Bytes();
        Assert.Equal(10, span.Length);
        Assert.Equal("1234567890", System.Text.Encoding.UTF8.GetString(span));

        // Negative value
        long val2 = -1;
        buffer.Append(val2);
        span = buffer.Bytes();
        Assert.Equal(12, span.Length);
        Assert.Equal("1234567890-1", System.Text.Encoding.UTF8.GetString(span));

        buffer.Dispose();
    }

    [Fact]
    public void Append_AddsUInt64Correctly() {
        Common.RentedBuffer buffer = default;
        buffer.Rent(10);

        ulong val1 = 1234567890;
        buffer.Append(val1);
        var span = buffer.Bytes();
        Assert.Equal(10, span.Length);
        Assert.Equal("1234567890", System.Text.Encoding.UTF8.GetString(span));

        // Max value
        ulong val2 = ulong.MaxValue;
        buffer = default;
        buffer.Rent(20);
        buffer.Append(val2);
        span = buffer.Bytes();
        Assert.Equal(20, span.Length);
        Assert.Equal(ulong.MaxValue.ToString(), System.Text.Encoding.UTF8.GetString(span));

        buffer.Dispose();
    }

    [Fact]
    public void Append_AddsDoubleCorrectly() {
        Common.RentedBuffer buffer = default;
        buffer.Rent(1);

        double value1 = 123.5;
        double value2 = -0.25;
        buffer.Append("a=");
        buffer.Append(value1);
        buffer.Append(",b=");
        buffer.Append(value2);

        var text = System.Text.Encoding.UTF8.GetString(buffer.Bytes());
        var expected = "a=" + value1.ToString(System.Globalization.CultureInfo.InvariantCulture)
            + ",b=" + value2.ToString(System.Globalization.CultureInfo.InvariantCulture);
        Assert.Equal(expected, text);

        buffer.Dispose();
    }

    [Fact]
    public void Append_AddsBooleanCorrectly() {
        Common.RentedBuffer buffer = default;
        buffer.Rent(10);

        buffer.Append(true);
        var span = buffer.Bytes();
        Assert.Equal(4, span.Length);
        Assert.Equal("true", System.Text.Encoding.UTF8.GetString(span));

        buffer.Append(false);
        span = buffer.Bytes();
        Assert.Equal(9, span.Length);
        Assert.Equal("truefalse", System.Text.Encoding.UTF8.GetString(span));

        buffer.Dispose();
    }

    [Fact]
    public void Append_AddsReadOnlySpanCorrectly() {
        Common.RentedBuffer buffer = default;
        buffer.Rent(10);

        ReadOnlySpan<byte> data = "Hello"u8;
        buffer.Append(data);
        var span = buffer.Bytes();
        Assert.Equal(5, span.Length);
        Assert.Equal("Hello", System.Text.Encoding.UTF8.GetString(span));

        // Empty span
        buffer.Append(ReadOnlySpan<byte>.Empty);
        span = buffer.Bytes();
        Assert.Equal(5, span.Length);

        // Large span triggering extend
        byte[] largeData = new byte[100];
        for (int i = 0; i < 100; i++) largeData[i] = (byte)'x';
        buffer.Append((ReadOnlySpan<byte>)largeData);
        span = buffer.Bytes();
        Assert.Equal(105, span.Length);

        buffer.Dispose();
    }

    [Fact]
    public void AppendAsJsonEscapedString_EscapesSpecialBytes() {
        Common.RentedBuffer buffer = default;
        buffer.Rent(4);

        ReadOnlySpan<byte> input = "a\tb\nc\\d\"e"u8;
        buffer.AppendAsJsonEscapedString(input);

        var text = System.Text.Encoding.UTF8.GetString(buffer.Bytes());
        Assert.Equal("a\\tb\\nc\\\\d\\\"e", text);
        Assert.Equal(13, buffer.Length);

        buffer.Dispose();
    }

    [Fact]
    public void AppendAsJsonEscapedString_AppendsToExistingData() {
        Common.RentedBuffer buffer = default;
        buffer.Rent(8);

        buffer.Append("prefix");
        buffer.AppendAsJsonEscapedString(ReadOnlySpan<byte>.Empty);
        buffer.AppendAsJsonEscapedString("x"u8);

        var text = System.Text.Encoding.UTF8.GetString(buffer.Bytes());
        Assert.Equal("prefixx", text);

        buffer.Dispose();
    }

    [Fact]
    public void AppendAsJsonEscapedString_StringEscapes() {
        Common.RentedBuffer buffer = default;
        buffer.Rent(4);

        string input = "a\tb\nc\\d\"e";
        var encoded = System.Text.Json.JsonEncodedText.Encode(input);

        buffer.Append("prefix");
        int prefixLength = buffer.Length;
        buffer.AppendAsJsonEscapedString(input);

        var text = System.Text.Encoding.UTF8.GetString(buffer.Bytes());
        var encodedText = System.Text.Encoding.UTF8.GetString(encoded.EncodedUtf8Bytes);
        Assert.Equal("prefix" + encodedText, text);
        Assert.Equal(prefixLength + encoded.EncodedUtf8Bytes.Length, buffer.Length);

        buffer.Dispose();
    }

    [Fact]
    public void Clone_CopiesBufferAndLength() {
        Common.RentedBuffer buffer = default;
        buffer.Rent(8);

        buffer.Append("clone");
        var clone = buffer.Clone();

        Assert.NotNull(clone.Data);
        Assert.Equal(buffer.Length, clone.Length);
        Assert.False(object.ReferenceEquals(buffer.Data, clone.Data));
        Assert.Equal(
            System.Text.Encoding.UTF8.GetString(buffer.Bytes()),
            System.Text.Encoding.UTF8.GetString(clone.Bytes())
        );

        clone.Append((byte)'x');
        Assert.NotEqual(buffer.Length, clone.Length);
        Assert.Equal("clone", System.Text.Encoding.UTF8.GetString(buffer.Bytes()));
        Assert.Equal("clonex", System.Text.Encoding.UTF8.GetString(clone.Bytes()));

        clone.Dispose();
        buffer.Dispose();
    }

    [Fact]
    public void AppendUtcDatetime_FormatsUtc() {
        Common.RentedBuffer buffer = default;
        buffer.Rent(64);

        var dtm = new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc).AddTicks(1_234_567);
        buffer.AppendUtcDatetime(dtm);

        var text = System.Text.Encoding.UTF8.GetString(buffer.Bytes());
        Assert.Equal("2024-01-02T03:04:05.1234567Z", text);

        buffer.Dispose();
    }

    [Fact]
    public void AppendUtcDatetime_ConvertsLocalToUtc() {
        Common.RentedBuffer buffer = default;
        buffer.Rent(64);

        var utc = new DateTime(2024, 6, 7, 8, 9, 10, DateTimeKind.Utc).AddTicks(7_654_321);
        var local = utc.ToLocalTime();
        buffer.AppendUtcDatetime(local);

        var text = System.Text.Encoding.UTF8.GetString(buffer.Bytes());
        var expected = local.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ", System.Globalization.CultureInfo.InvariantCulture);
        Assert.Equal(expected, text);

        buffer.Dispose();
    }

    [Fact]
    public void AppendUtcDatetime_UnspecifiedIsTreatedAsUtc() {
        Common.RentedBuffer buffer = default;
        buffer.Rent(64);

        var dtm = new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Unspecified).AddTicks(9_876_543);
        buffer.AppendUtcDatetime(dtm);

        var text = System.Text.Encoding.UTF8.GetString(buffer.Bytes());
        var expected = DateTime.SpecifyKind(dtm, DateTimeKind.Utc)
            .ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ", System.Globalization.CultureInfo.InvariantCulture);
        Assert.Equal(expected, text);

        buffer.Dispose();
    }



    [Fact]
    public void Append_EdgeCases() {
        Common.RentedBuffer buffer = default;
        buffer.Rent(1); // Small buffer to force extension for bool

        // Test Append(bool) extension
        buffer.Append(true);
        var span = buffer.Bytes();
        Assert.Equal(4, span.Length);
        Assert.True(buffer.Data!.Length >= 5);

        // Test Append(string) with empty string
        buffer.Append("");
        Assert.Equal(4, buffer.Length); // Should not change

        // Test Append(params string[]) with empty string
        buffer.Append("A", "", "B");
        span = buffer.Bytes();
        Assert.Equal(6, span.Length);
        Assert.Equal("trueAB", System.Text.Encoding.UTF8.GetString(span));

        buffer.Dispose();
    }

    [Fact]
    public void Append_ForceExtension() {
        Common.RentedBuffer buffer = default;
        buffer.Rent(1);
        // ArrayPool might return more than 1. Fill it up.
        int initialCapacity = buffer.Data!.Length;
        for (int i = 0; i < initialCapacity; i++) {
            buffer.Append((byte)'x');
        }
        Assert.Equal(initialCapacity, buffer.Length);

        // Now next Append(byte) must trigger Extend
        buffer.Append((byte)'y');
        Assert.True(buffer.Data.Length > initialCapacity);

        // Test Append(params string[]) extension
        // Fill up again close to limit if needed, or just append enough strings
        int currentCapacity = buffer.Data.Length;
        int remaining = currentCapacity - buffer.Length;
        // Append small strings to fill remaining
        for (int i = 0; i < remaining; i++) {
            buffer.Append((byte)'z');
        }

        // Now Append(params string[]) triggering extension
        buffer.Append(new string[] { "Extension", "Test" });
        Assert.True(buffer.Data.Length > currentCapacity);

        // Test Append(bool) extension
        // Fill up buffer until less than 5 bytes remain
        int maxByteCountBool = 5;
        currentCapacity = buffer.Data.Length;
        int targetLength = currentCapacity - maxByteCountBool + 1; // force extend

        // Use byte append to fill up
        while (buffer.Length < targetLength) {
            buffer.Append((byte)'b');
        }

        int lengthBeforeBool = buffer.Length;
        // Verify we are in the zone where Extend is needed: (Capacity - Length < 5)
        Assert.True(buffer.Data.Length - buffer.Length < maxByteCountBool);

        buffer.Append(true); // Should trigger Extend
        Assert.True(buffer.Data.Length > currentCapacity);

        buffer.Dispose();
    }
}
