namespace Compress {
    public class ZstdCompressor {
        [ThreadStatic]
        private static ZstdSharp.Compressor? _compressor;
        [ThreadStatic]
        private static ZstdSharp.Decompressor? _decompressor;

        /// <summary>
        /// 对数据进行 zstd 压缩
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static System.ValueTuple<Common.RentedBuffer, Common.Error> Compress(ReadOnlySpan<byte> input) {
            var compressor = _compressor ??= new ZstdSharp.Compressor();
            Common.RentedBuffer dst = default;
            int bound = ZstdSharp.Compressor.GetCompressBound(input.Length);
            dst.Rent(bound);
            bool success = compressor.TryWrap(input, dst.Data.AsSpan(), out int written);
            if (!success) {
                dst.Dispose();
                return (default(Common.RentedBuffer), new Common.Error { Code = 1, Message = "compressor.TryWrap fail" });
            }
            if (written <= 0) {
                dst.Dispose();
                return (default(Common.RentedBuffer), new Common.Error { Code = 2, Message = $"compressor.TryWrap fail,written={written}" });
            }
            dst.Length = written;
            return (dst, default(Common.Error));
        }

        /// <summary>
        /// 对数据进行 zstd 解压
        /// </summary>
        /// <param name="compressed"></param>
        /// <returns></returns>
        public static System.ValueTuple<Common.RentedBuffer, Common.Error> Uncompress(ReadOnlySpan<byte> compressed) {
            var decompressor = _decompressor ??= new ZstdSharp.Decompressor();
            ulong size;
            try {
                size = ZstdSharp.Decompressor.GetDecompressedSize(compressed);
            } catch (System.Exception ex) {
                 return (default(Common.RentedBuffer), new Common.Error { Code = 3, Message = $"GetDecompressedSize fail: {ex.Message}" });
            }
            Common.RentedBuffer dst = default;
            dst.Rent((int)size);
            bool success;
            int written;
            try {
                success = decompressor.TryUnwrap(compressed, dst.Data.AsSpan(), out written);
            } catch (System.Exception ex) {
                dst.Dispose();
                return (default(Common.RentedBuffer), new Common.Error { Code = 4, Message = $"decompressor.TryUnwrap fail: {ex.Message}" });
            }
            if (!success) {
                dst.Dispose();
                return (default(Common.RentedBuffer), new Common.Error { Code = 4, Message = "decompressor.TryUnwrap fail" });
            }
            dst.Length = written;
            return (dst, default(Common.Error));
        }
    }
}
