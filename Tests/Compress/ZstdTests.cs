using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Compress;

namespace CompressTests {
    public class ZstdTests {
        public struct TestCase {
            public string Name;
            public byte[] Input;
            public bool IsCompressedInput; // If true, Uncompress directly. If false, Compress then Uncompress.
            public bool ExpectCompressError;
            public bool ExpectUncompressError;
            public uint ExpectedErrorCode;
            public string ExpectedErrorMsgContains;
        }

        [Fact]
        public void TestZstd_TableDriven() {
            var random = new Random(42);
            var largeData = new byte[1024 * 1024]; // 1MB
            random.NextBytes(largeData);

            // Construct corrupted data with Checksum enabled (to force TryUnwrap fail)
            using var compressorWithChecksum = new ZstdSharp.Compressor();
            compressorWithChecksum.SetParameter(ZstdSharp.Unsafe.ZSTD_cParameter.ZSTD_c_checksumFlag, 1);
            var srcSpan = System.Text.Encoding.UTF8.GetBytes("hello checksum");
            var boundChecks = ZstdSharp.Compressor.GetCompressBound(srcSpan.Length);
            var dstChecks = new byte[boundChecks];
            compressorWithChecksum.TryWrap(srcSpan, dstChecks, out int writtenChecks);
            var corruptBody = dstChecks.Take(writtenChecks).ToArray();

            // Corrupt the body (middle byte, to avoid header damage)
            if (corruptBody.Length > 10) corruptBody[corruptBody.Length / 2] ^= 0xFF;

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
                    Name = "Garbage Data for Uncompress (Unknown size)",
                    Input = new byte[] { 0x01, 0x02, 0x03, 0x04 }, // Not a valid Zstd frame
                    IsCompressedInput = true,
                    ExpectUncompressError = true,
                    ExpectedErrorCode = 3, // Code = 3, Message = "Unknown decompressed size"
                    ExpectedErrorMsgContains = "GetDecompressedSize fail"
                },
                new TestCase {
                    Name = "Corrupt Body (TryUnwrap fail)",
                    Input = corruptBody,
                    IsCompressedInput = true,
                    ExpectUncompressError = true,
                    ExpectedErrorCode = 4, // Code = 4, Message = "decompressor.TryUnwrap fail"
                    ExpectedErrorMsgContains = "decompressor.TryUnwrap fail"
                },
                new TestCase {
                    Name = "Truncated Body",
                    Input = System.Text.Encoding.UTF8.GetBytes("truncated"), // Placeholder
                    IsCompressedInput = true,
                    ExpectUncompressError = true,
                    // If GetDecompressedSize fails, it's code 3. If it succeeds but Unwrap fails, code 4.
                    // Truncation usually causes Code 3 if frame structure is broken.
                    ExpectedErrorCode = 3,
                    ExpectedErrorMsgContains = "GetDecompressedSize fail"
                },
                new TestCase {
                    Name = "Lying Header (Content Size < Actual)",
                    Input = System.Text.Encoding.UTF8.GetBytes("lying"), // Placeholder
                    IsCompressedInput = true,
                    ExpectUncompressError = true,
                    ExpectedErrorCode = 4,
                    ExpectedErrorMsgContains = "decompressor.TryUnwrap fail"
                }
            };

            // Prepare dynamic test data for placeholders
            var indexTrunc = testCases.FindIndex(t => t.Name == "Truncated Body");
            if (indexTrunc >= 0) {
                var valid = ZstdCompressor.Compress(System.Text.Encoding.UTF8.GetBytes("Hello World with enough length to have body")).Item1;
                var validBytes = valid.Bytes().ToArray();
                valid.Dispose();
                // Truncate last byte
                if (validBytes.Length > 0) {
                    var trunc = validBytes.Take(validBytes.Length - 1).ToArray();
                    var tc = testCases[indexTrunc];
                    tc.Input = trunc;
                    testCases[indexTrunc] = tc;
                }
            }

            var indexLying = testCases.FindIndex(t => t.Name == "Lying Header (Content Size < Actual)");
            if (indexLying >= 0) {
                // Create valid frame with known size (e.g. 100 'A's)
                byte[] original = new byte[100];
                for (int i = 0; i < 100; i++) original[i] = 65;

                var valid = ZstdCompressor.Compress(original).Item1;
                var validBytes = valid.Bytes().ToArray();
                valid.Dispose();

                // Heuristic: Search for the size byte (100 = 0x64) in the header (first 20 bytes) and change it to 1.
                // Zstd stores Frame_Content_Size.
                // For 100 bytes, it fits in 1 byte or 2 bytes depending on encoding.
                // If we find 0x64, change to 0x01.
                for (int i = 4; i < Math.Min(validBytes.Length, 20); i++) {
                    if (validBytes[i] == 100) {
                        validBytes[i] = 1; // Claim size is 1
                        break;
                    }
                }

                var tc = testCases[indexLying];
                tc.Input = validBytes;
                testCases[indexLying] = tc;
            }

            foreach (var tc in testCases) {
                RunTestCase(tc);
            }
        }

        [Fact]
        public void TestZstd_Concurrency() {
            int threadCount = 8;
            int iterationsPerThread = 100;
            var errors = new System.Collections.Concurrent.ConcurrentBag<string>();

            Parallel.For(0, threadCount, i => {
                var random = new Random(i * 997); // Different seed per thread/iteration
                for (int j = 0; j < iterationsPerThread; j++) {
                    try {
                        // 1. Generate random data
                        byte[] data = new byte[random.Next(100, 10240)];
                        random.NextBytes(data);

                        // 2. Compress
                        var (compressed, errComp) = ZstdCompressor.Compress(data);
                        if (errComp.Err()) {
                            errors.Add($"Thread {i} Iter {j} Compress Error: {errComp.Message}");
                            compressed.Dispose();
                            continue;
                        }

                        // 3. Uncompress
                        var (uncompressed, errUn) = ZstdCompressor.Uncompress(compressed.Bytes());
                        if (errUn.Err()) {
                            errors.Add($"Thread {i} Iter {j} Uncompress Error: {errUn.Message}");
                        }
                        else {
                            // 4. Verify
                            var resultBytes = uncompressed.Bytes().ToArray();
                            if (!data.SequenceEqual(resultBytes)) {
                                errors.Add($"Thread {i} Iter {j} Data Mismatch");
                            }
                        }

                        compressed.Dispose();
                        uncompressed.Dispose();

                    }
                    catch (Exception ex) {
                        errors.Add($"Thread {i} Iter {j} Exception: {ex}");
                    }
                }
            });

            Assert.Empty(errors);
        }

        private void RunTestCase(TestCase tc) {
            byte[] compressedData = Array.Empty<byte>();
            byte[] originalData = Array.Empty<byte>();

            if (!tc.IsCompressedInput) {
                originalData = tc.Input;
                // Test Compress
                var (compressed, err) = ZstdCompressor.Compress(tc.Input);

                if (tc.ExpectCompressError) {
                    Assert.True(err.Err(), $"TestCase '{tc.Name}': Expected compression error but got none.");
                    if (tc.ExpectedErrorCode != 0) Assert.Equal(tc.ExpectedErrorCode, err.Code);
                    if (!string.IsNullOrEmpty(tc.ExpectedErrorMsgContains)) Assert.Contains(tc.ExpectedErrorMsgContains, err.Message);
                    compressed.Dispose();
                    return; // Done
                }
                else {
                    Assert.False(err.Err(), $"TestCase '{tc.Name}': Unexpected compression error: {err.Message}");
                    // Zstd empty input produces a frame (9 bytes usually), so length > 0
                    Assert.True(compressed.Length > 0 || (tc.Input.Length == 0 && compressed.Length >= 0));
                    compressedData = compressed.Bytes().ToArray();
                    Assert.True(compressedData.Length > 0);
                    compressed.Dispose();
                }
            }
            else {
                compressedData = tc.Input;
            }

            // Test Uncompress
            var (uncompressed, errUn) = ZstdCompressor.Uncompress(compressedData);

            if (tc.ExpectUncompressError) {
                Assert.True(errUn.Err(), $"TestCase '{tc.Name}': Expected uncompression error but got none.");
                if (tc.ExpectedErrorCode != 0) Assert.Equal(tc.ExpectedErrorCode, errUn.Code);
                if (!string.IsNullOrEmpty(tc.ExpectedErrorMsgContains)) Assert.Contains(tc.ExpectedErrorMsgContains, errUn.Message);
            }
            else {
                Assert.False(errUn.Err(), $"TestCase '{tc.Name}': Unexpected uncompression error: {errUn.Message}");

                // If we started with uncompressed data, verify roundtrip equality
                if (!tc.IsCompressedInput) {
                    var resultBytes = uncompressed.Bytes().ToArray();
                    Assert.Equal(originalData.Length, resultBytes.Length);
                    Assert.True(originalData.SequenceEqual(resultBytes), $"TestCase '{tc.Name}': Data mismatch after roundtrip.");
                }
            }
            uncompressed.Dispose();
        }
    }
}
