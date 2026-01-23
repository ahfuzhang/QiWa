using System;
using System.Threading;
using Common;

namespace Log {
    public class ThreadLocalLogger {
        private static readonly ThreadLocal<ThreadLocalLogger> _threadLocal = 
            new ThreadLocal<ThreadLocalLogger>(() => new ThreadLocalLogger(), trackAllValues: true);

        internal static ThreadLocalLogger Current => _threadLocal.Value!;

        // 128KB buffer threshold
        private const int FlushThreshold = 128 * 1024;
        
        internal Common.RentedBuffer Buffer;
        private readonly System.IO.Stream Stdout = Console.OpenStandardOutput();
        //private readonly object _lock = new object(); 
        private ulong _locker = 0;

        internal void Lock() {
            while (Interlocked.CompareExchange(ref _locker, 1, 0) != 0) {
                // spin lock
            }
        }

        internal void UnLock() {
            while (Interlocked.CompareExchange(ref _locker, 0, 1) != 1) {
                // spin lock
            }
        }

        internal ThreadLocalLogger() {
            Buffer = new Common.RentedBuffer(FlushThreshold);
            Logger.Instance.Register(this);  // 注册到全局
        }

        ~ThreadLocalLogger() {
            Logger.Instance.Unregister(this);  // 从全局注销
        }

        internal void SwitchBuffer() {
            if (Buffer.Length == 0) return;
            Lock();
            while (!Logger.Instance.BufferChannel.Writer.TryWrite(Buffer)) {
                // todo: 危险，这里可能导致问题
                Thread.Yield();
            }
            Buffer = new Common.RentedBuffer(FlushThreshold);
            UnLock();
        }

        // 每次写日志后 Flush
        internal void Flush() {
            if (Buffer.Length < FlushThreshold - 1024) {
                // 127 kb 写一次 stdout
                return;
            }
            if (Interlocked.CompareExchange(ref Logger.StdoutOccupy, 1, 0) != 0) {
                SwitchBuffer();
                // Lock();
                // while (!Logger.Instance.BufferChannel.Writer.TryWrite(Buffer)) {
                //     // todo: 危险，这里可能导致问题
                //     Thread.Yield();
                // }
                // Buffer = new Common.RentedBuffer(FlushThreshold);
                // UnLock();
                return;
            }
            try {
                Stdout.WriteAsync(Buffer.Data.AsMemory<byte>(0, Buffer.Length));
                Buffer.Length = 0;  // 写完后，清空 buffer
            }
            finally {
                Interlocked.CompareExchange(ref Logger.StdoutOccupy, 0, 1);
            }
        }
        // // -----------------------

        // public void Append(ReadOnlySpan<byte> data) {
        //     // Check capacity
        //     if (Buffer.Data != null && Buffer.Length + data.Length > FlushThreshold) {
        //         Flush1();
        //     }
            
        //     Buffer.Append(data);
        // }

        // public void Flush1() {
        //     if (Buffer.Length == 0) return;

        //     // Try to write to channel
        //     bool written = Logger.Instance.BufferChannel.Writer.TryWrite(Buffer);
            
        //     if (written) {
        //         // Buffer ownership transferred, rent new one
        //         Buffer = new Common.RentedBuffer(FlushThreshold);
        //     } else {
        //         // Channel full, fallback to stdout
        //         WriteToStdout(Buffer);
        //         Buffer.Length = 0; // Reuse same buffer since we handled the data
        //     }
        // }

        // private void WriteToStdout(Common.RentedBuffer buf) {
        //      // CAS lock for stdout
        //      // Check global atomic int
        //      int lockTaken = Interlocked.CompareExchange(ref Logger.Instance.StdoutLock, 1, 0);
        //      if (lockTaken == 0) {
        //          try {
        //              var span = buf.Bytes();
        //              if (span.Length > 0) {
        //                  using var stdout = Console.OpenStandardOutput();
        //                  stdout.Write(span);
        //                  stdout.Flush();
        //              }
        //          } finally {
        //              Interlocked.Exchange(ref Logger.Instance.StdoutLock, 0);
        //          }
        //      } else {
        //          // Stdout busy? The design doc says "当 全局的 atomic int 为 1... 说明 stdout 已经被占用。 这时候把日志 buffer 通过 channel 发送出去"
        //          // But we are here because Channel was full? This is a deadlock/drop scenario.
        //          // If channel is full AND stdout is busy, we might have to drop or spin wait.
        //          // For high performance, verify if we should just force write or drop.
        //          // Simpler fallback: just write to console using standard api which has internal locks, 
        //          // but the requirement is to use the CAS lock optimization.
                 
        //          // If locking fails, maybe we just spin briefly or drop. 
        //          // Let's try to write to channel again? 
        //          // Or just use System.Console.WriteLine which locks internally as a last resort.
        //          try {
        //              var span = buf.Bytes();
        //              if (span.Length > 0) {
        //                  Console.OpenStandardOutput().Write(span);
        //              }
        //          } catch { } // Ignore errors
        //      }
        // }
    }
}
