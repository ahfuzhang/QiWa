
using System.Reflection.Metadata;
using System.Buffers;
using System.Security.AccessControl;

namespace FileUtils {
    public class Utils {
        public static async Task<bool> FileExistsAndNotEmptyAsync(string path) {
            // 虽然这个实现很麻烦，但是这是纯异步的版本
            try {
                await using var stream = new FileStream(
                    path,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: 1,
                    options: FileOptions.Asynchronous | FileOptions.SequentialScan);
                int readLen = await stream.ReadAsync(_temp, 0, 1);
                return readLen==1;
            }
            catch (FileNotFoundException) {
                return false;
            }
            catch (DirectoryNotFoundException) {
                return false;
            }
        }
        private static byte[] _temp = new byte[1];

        /// <summary>
        /// 如果一次性加载文件的所有内容到内存，所允许的支持的最大文件长度，100mb
        /// </summary>
        private const Int64 READ_ALL_ALLOWED_MAX_BYTES = 1024 * 1024 * 100L;
        private const int default_file_buffer_size = 64 * 1024;

        /// <summary>
        /// 一次性把一个文件的全部内容读到内存。最大支持 100mb
        /// </summary>
        /// <param name="path"> 文件路径 </param>
        /// <returns>
        ///   Common.RentedBuffer: 文件内容的数组。数组是从内存池借用的，使用完成后需要归还。
        ///   Common.Error
        /// </returns>
        public static async Task<System.ValueTuple<Common.RentedBuffer, Common.Error>> ReadAllAndRentAync(string inputPath) {
            Common.RentedBuffer rent = default;
            int totalRead = 0;
            try {
                await using var stream = new FileStream(
                    inputPath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: default_file_buffer_size,
                    useAsync: true);
                if (stream.Length > READ_ALL_ALLOWED_MAX_BYTES) {
                    return (rent, new Common.Error { Code = 1, Message = $"Input file is too large: {stream.Length} bytes." });
                }
                int length = (int)stream.Length;
                if (length == 0) {
                    return (rent, new Common.Error { Code = 2, Message = "Loaded 0 bytes." });
                }
                rent.Rent(length);
                while (totalRead < length) {
                    int read = await stream.ReadAsync(rent.Data!, totalRead, length - totalRead);
                    if (read == 0) {
                        break;
                    }
                    totalRead += read;
                }
                if (totalRead != length) {
                    return (rent, new Common.Error { Code = 3, Message = $"not read all:{totalRead}/{length}" });
                }
                return (rent, default(Common.Error));
            }
            catch (Exception) {
                // 注意： ArrayPool<byte>.Shared.Rent 相当于使用了非托管资源。
                //    必须考虑异常情况下的资源释放，否则会有内存泄露
                rent.Dispose();
                throw;
            }
        }
    }
}
