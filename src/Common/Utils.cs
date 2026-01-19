
using System.Buffers;

namespace Common {
    public class Utils {

    }

    /// <summary>
    /// 这个类是为了提供类似 golang 中的 defer 语句的效果
    /// </summary>
    /// <example>
    /// ```csharp
    /// // 退出作用域时，自动调用 Cleanup() 函数。本质上是 try...finally 的另一个写法
    /// using (var _ = new ScopeGuard(() => Cleanup(obj))){
    ///     Step1();
    ///     Step2();
    /// }
    /// ```
    /// </example>
    public struct ScopeGuard : IDisposable {
        private Action? _onDispose;

        public ScopeGuard(Action onDispose) {
            _onDispose = onDispose;
        }

        public void Dispose() {
            _onDispose?.Invoke();
            _onDispose = null;
        }
    }

    /// <summary>
    /// 封装一个 error 对象
    /// </summary>
    public struct Error {
        public System.UInt32 Code;
        public string Message;

        /// <summary>
        /// 判断是否有错误发生
        /// </summary>
        /// <returns></returns>
        public readonly bool Err() {
            return Code != 0;
        }
    }

    public struct RentedBuffer : IDisposable {
        public byte[]? Data;
        public System.Int32 Length;

        /// <summary>
        /// 从内存池借用内存
        /// </summary>
        /// <param name="length">需要的内存大小</param>
        /// <exception>out of memory</exception>
        public void Rent(System.Int32 length) {
            Data = ArrayPool<byte>.Shared.Rent(length);
            Length = length;
        }

        public void Dispose() {
            if (Data != null) {
                ArrayPool<byte>.Shared.Return(Data);
                Data = null;
                Length = 0;
            }
        }

        public Span<byte> Bytes() {
            if (Data == null || Length == 0) {
                return Span<byte>.Empty;
            }
            return Data.AsSpan(0, Length);
        }
    }
}
