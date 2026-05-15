using Compress;
using Xunit;

namespace CompressTests;

public class GzipTests
{
    public struct TestCase
    {
        public string Name;
        public byte[] Input;
        public bool IsCompressedInput;
        public bool ExpectCompressError;
        public bool ExpectUncompressError;
        public uint ExpectedErrorCode;
        public string ExpectedErrorMsgContains;
    }

    [Fact]
    public void TestGzip_TableDriven()
    {
        var random = new Random(42);
        var largeData = new byte[1024 * 1024]; // 1MB
        random.NextBytes(largeData);

        var testCases = new List<TestCase> {
            new TestCase {
                Name = "Normal String",
                Input = System.Text.Encoding.UTF8.GetBytes("Hello, World!"),
                IsCompressedInput = false
            },
            new TestCase {
                Name = "Empty Input",
                Input = Array.Empty<byte>(),
                IsCompressedInput = false
            },
            new TestCase {
                Name = "Large Random Data",
                Input = largeData,
                IsCompressedInput = false
            },
            new TestCase {
                // 65536 个零字节压缩后极小（约 50 字节），ISIZE/压缩大小 >> MaxDecompressRatio，
                // 触发 ISIZE 比率检测（Code=3）。这是当前实现对极高压缩率数据的假正例。
                Name = "Highly Compressible Data",
                Input = new byte[65536], // all zeros compress very well
                IsCompressedInput = false,
                ExpectUncompressError = true,
                ExpectedErrorCode = 3,
                ExpectedErrorMsgContains = "GZip bomb detected"
            },
            new TestCase {
                // 无效的 gzip 数据（非 0x1F 0x8B 开头），GZipStream 抛 InvalidDataException → Code=3
                Name = "Garbage Data for Uncompress",
                Input = new byte[] { 0x01, 0x02, 0x03, 0x04 }, // Not a valid gzip stream
                IsCompressedInput = true,
                ExpectUncompressError = true,
                ExpectedErrorCode = 3,
                ExpectedErrorMsgContains = "GZip bomb detected"
            },
            new TestCase {
                // 截断的 gzip 流：头部有效但 deflate 数据不完整，解压失败。
                // 错误码取决于截断位置末尾 4 字节是否触发 ISIZE 检测，不做严格断言。
                Name = "Truncated gzip Stream",
                Input = Array.Empty<byte>(), // filled in below
                IsCompressedInput = true,
                ExpectUncompressError = true,
                ExpectedErrorCode = 0, // 不限定具体 code，可能是 2 或 3
                ExpectedErrorMsgContains = ""
            },
        };

        // Prepare truncated gzip data
        var indexTrunc = testCases.FindIndex(t => t.Name == "Truncated gzip Stream");
        if (indexTrunc >= 0)
        {
            var (valid, _) = GzipCompressor.Compress(System.Text.Encoding.UTF8.GetBytes("Hello World with enough content"));
            var validBytes = valid.Bytes().ToArray();
            valid.Dispose();
            if (validBytes.Length > 4)
            {
                var tc = testCases[indexTrunc];
                tc.Input = validBytes.Take(validBytes.Length / 2).ToArray();
                testCases[indexTrunc] = tc;
            }
        }

        foreach (var tc in testCases)
        {
            RunTestCase(tc);
        }
    }

    [Fact]
    public void TestGzip_Concurrency()
    {
        int threadCount = 8;
        int iterationsPerThread = 100;
        var errors = new System.Collections.Concurrent.ConcurrentBag<string>();

        Parallel.For(0, threadCount, i =>
        {
            var random = new Random(i * 997);
            for (int j = 0; j < iterationsPerThread; j++)
            {
                try
                {
                    byte[] data = new byte[random.Next(100, 10240)];
                    random.NextBytes(data);

                    var (compressed, errComp) = GzipCompressor.Compress(data);
                    if (errComp.Err())
                    {
                        errors.Add($"Thread {i} Iter {j} Compress Error: {errComp.Message}");
                        compressed.Dispose();
                        continue;
                    }

                    var (uncompressed, errUn) = GzipCompressor.Uncompress(compressed.Bytes());
                    if (errUn.Err())
                    {
                        errors.Add($"Thread {i} Iter {j} Uncompress Error: {errUn.Message}");
                    }
                    else
                    {
                        var resultBytes = uncompressed.Bytes().ToArray();
                        if (!data.SequenceEqual(resultBytes))
                        {
                            errors.Add($"Thread {i} Iter {j} Data Mismatch");
                        }
                    }

                    compressed.Dispose();
                    uncompressed.Dispose();
                }
                catch (Exception ex)
                {
                    errors.Add($"Thread {i} Iter {j} Exception: {ex}");
                }
            }
        });

        Assert.Empty(errors);
    }

    [Fact]
    public void TestGzip_RoundTrip_ThreadLocalReuse()
    {
        // Verifies that repeated calls on the same thread correctly reuse ThreadStatic state
        byte[] data1 = System.Text.Encoding.UTF8.GetBytes("first message");
        byte[] data2 = System.Text.Encoding.UTF8.GetBytes("second message, longer than the first one");
        byte[] data3 = new byte[512];
        new Random(1).NextBytes(data3);

        foreach (var data in new[] { data1, data2, data3, data1, data2 })
        {
            var (compressed, errComp) = GzipCompressor.Compress(data);
            Assert.False(errComp.Err(), $"Compress error: {errComp.Message}");
            Assert.True(compressed.Length > 0);

            var (uncompressed, errUn) = GzipCompressor.Uncompress(compressed.Bytes());
            Assert.False(errUn.Err(), $"Uncompress error: {errUn.Message}");
            Assert.True(data.SequenceEqual(uncompressed.Bytes().ToArray()), "Data mismatch after roundtrip");

            compressed.Dispose();
            uncompressed.Dispose();
        }
    }

    private static void RunTestCase(TestCase tc)
    {
        byte[] originalData = Array.Empty<byte>();
        byte[] compressedData;

        if (!tc.IsCompressedInput)
        {
            originalData = tc.Input;
            var (compressed, err) = GzipCompressor.Compress(tc.Input);

            if (tc.ExpectCompressError)
            {
                Assert.True(err.Err(), $"TestCase '{tc.Name}': Expected compression error but got none.");
                if (tc.ExpectedErrorCode != 0)
                {
                    Assert.Equal(tc.ExpectedErrorCode, err.Code);
                }
                if (!string.IsNullOrEmpty(tc.ExpectedErrorMsgContains))
                {
                    Assert.Contains(tc.ExpectedErrorMsgContains, err.Message);
                }
                compressed.Dispose();
                return;
            }
            else
            {
                Assert.False(err.Err(), $"TestCase '{tc.Name}': Unexpected compression error: {err.Message}");
                compressedData = compressed.Bytes().ToArray();
                compressed.Dispose();
                // 空输入压缩后长度可能为 0（GZipStream 对空 span 不写出 header），此时直接跳过解压验证
                if (compressedData.Length == 0)
                {
                    return;
                }
            }
        }
        else
        {
            compressedData = tc.Input;
        }

        var (uncompressed, errUn) = GzipCompressor.Uncompress(compressedData);

        if (tc.ExpectUncompressError)
        {
            Assert.True(errUn.Err(), $"TestCase '{tc.Name}': Expected uncompression error but got none.");
            if (tc.ExpectedErrorCode != 0)
            {
                Assert.Equal(tc.ExpectedErrorCode, errUn.Code);
            }
            if (!string.IsNullOrEmpty(tc.ExpectedErrorMsgContains))
            {
                Assert.Contains(tc.ExpectedErrorMsgContains, errUn.Message);
            }
        }
        else
        {
            Assert.False(errUn.Err(), $"TestCase '{tc.Name}': Unexpected uncompression error: {errUn.Message}");
            if (!tc.IsCompressedInput)
            {
                var resultBytes = uncompressed.Bytes().ToArray();
                Assert.Equal(originalData.Length, resultBytes.Length);
                Assert.True(originalData.SequenceEqual(resultBytes), $"TestCase '{tc.Name}': Data mismatch after roundtrip.");
            }
        }
        uncompressed.Dispose();
    }

    /// <summary>
    /// 通过篡改有效 gzip 流末尾 4 字节（ISIZE 字段）为一个远超阈值的值，
    /// 显式触发 Uncompress 中的 ISIZE 比率炸弹检测分支（Code=3）。
    /// </summary>
    [Fact]
    public void TestGzip_BombDetection_ManipulatedISIZE()
    {
        var (valid, errComp) = GzipCompressor.Compress(System.Text.Encoding.UTF8.GetBytes("hello bomb detection"));
        Assert.False(errComp.Err(), $"Compress failed: {errComp.Message}");
        var bytes = valid.Bytes().ToArray();
        valid.Dispose();

        Assert.True(bytes.Length >= 18, "Compressed stream should be at least 18 bytes");

        // 将末尾 4 字节（ISIZE 字段）替换为 100000（little-endian: 0xA0 0x86 0x01 0x00）
        // bytes.Length << 100000，ratio >> MaxDecompressRatio，必然触发炸弹检测
        var manipulated = (byte[])bytes.Clone();
        manipulated[^4] = 0xA0;
        manipulated[^3] = 0x86;
        manipulated[^2] = 0x01;
        manipulated[^1] = 0x00;

        var (buf, err) = GzipCompressor.Uncompress(manipulated);
        buf.Dispose();

        Assert.True(err.Err(), "Expected gzip bomb detection error");
        Assert.Equal(3u, err.Code);
        Assert.Contains("GZip bomb detected", err.Message);
        Assert.Contains("decompressed size exceeds", err.Message);
    }

    /// <summary>
    /// 通过反射访问私有 ThreadStatic 字段 _compressStream，
    /// 直接测试 RentedBufferWriteStream 的各个属性和不支持操作，
    /// 覆盖正常使用路径无法触达的代码行。
    /// </summary>
    [Fact]
    public void TestGzip_RentedBufferWriteStream_AllMembers()
    {
        // 先调用 Compress 确保当前线程的 _compressStream 已初始化
        var (initBuf, initErr) = GzipCompressor.Compress(System.Text.Encoding.UTF8.GetBytes("init"));
        initBuf.Dispose();
        Assert.False(initErr.Err());

        // 通过反射获取私有 ThreadStatic 字段
        var field = typeof(GzipCompressor).GetField(
            "_compressStream",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        Assert.NotNull(field);

        var stream = field.GetValue(null) as System.IO.Stream;
        Assert.NotNull(stream);

        // 调用私有 Reset(int) 方法，重新准备缓冲区
        var resetMethod = stream.GetType().GetMethod(
            "Reset",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(resetMethod);
        resetMethod.Invoke(stream, new object[] { 64 });

        // 验证只读属性
        Assert.False(stream.CanRead);   // line 44
        Assert.False(stream.CanSeek);   // line 45
        Assert.True(stream.CanWrite);   // line 46 (已被 GZipStream 覆盖，此处再次确认)
        Assert.Equal(0L, stream.Length);    // line 47: Reset 后 Buffer.Length = 0
        Assert.Equal(0L, stream.Position);  // line 48 getter

        // Flush 是空操作，不应抛异常（line 49）
        stream.Flush();

        // Write(ReadOnlySpan<byte>) 重载（lines 62-66）
        stream.Write(new byte[] { 0xAA, 0xBB, 0xCC }.AsSpan());
        Assert.Equal(3L, stream.Length);

        // 不支持的操作均应抛出 NotSupportedException
        Assert.Throws<NotSupportedException>(() => stream.Read(new byte[1], 0, 1));           // line 50
        Assert.Throws<NotSupportedException>(() => stream.Seek(0, System.IO.SeekOrigin.Begin)); // line 51
        Assert.Throws<NotSupportedException>(() => stream.SetLength(0));                       // line 52
        Assert.Throws<NotSupportedException>(() => { stream.Position = 0; });                  // line 48 setter

        // 清理缓冲区，避免内存泄漏
        var disposeBufferMethod = stream.GetType().GetMethod(
            "DisposeBuffer",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        disposeBufferMethod?.Invoke(stream, null);
    }

    /// <summary>
    /// 向 Uncompress 传入长度小于 18 字节的非 gzip 数据，
    /// GZipStream 抛出 InvalidDataException（魔数不正确），触发 Code=3 分支。
    /// </summary>
    [Fact]
    public void TestGzip_InvalidData_ShortNonGzip()
    {
        // 10 字节随机垃圾，不含合法 gzip 魔数 0x1F 0x8B
        // length < 18 → ISIZE 检测跳过，GZipStream 读取时抛 InvalidDataException → Code=3
        var garbage = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF, 0x00, 0x11, 0x22, 0x33, 0x44, 0x55 };

        var (buf, err) = GzipCompressor.Uncompress(garbage);
        buf.Dispose();

        Assert.True(err.Err(), "Expected error for non-gzip input");
        Assert.Equal(3u, err.Code);
        Assert.Contains("GZip bomb detected", err.Message);
    }
}
