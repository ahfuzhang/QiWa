using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Common;
using System.Runtime.InteropServices;
using System.Text;


namespace Log {
    public enum LogLevel {
        Fatal,
        Error,
        Warn,
        Info,
        Debug
    }

    static class Syscall
    {
        [DllImport("libc", SetLastError = true)]
        public static extern long write(int fd, byte[] buf, ulong count);
    }

    public class Logger : IDisposable {
       
        internal static readonly Logger Instance = new Logger();

        public static void SetLevel(LogLevel level) {
            Instance.Level = level;
        }

        public static void SetFlushIntervalMs(int ms) {
            if (ms < 100) {
                ms = 100;
            }
            Instance._flushIntervalMs = ms;
        }

        public static void Shutdown() {
            //Instance._cts.Cancel();
            Instance.Dispose();
        }

        public static void SetGlobalTags(Dictionary<string, string> tags) {
            Common.RentedBuffer buf = new(512);
            buf.Append((byte)'{');
            bool isFirst = true;
            foreach(var (key,value) in tags) {
                if (isFirst) {
                    isFirst = false;
                } else {
                    buf.Append((byte)',');
                }
                Field.String(Encoding.UTF8.GetBytes(key), value).WriteTo(ref buf);
            }
            Instance.tagPrefix = new byte[buf.Length];
            Array.Copy(buf.Data!, Instance.tagPrefix, buf.Length);
            Console.WriteLine("\t\t Instance.tagPrefix={0}", Encoding.UTF8.GetString(Instance.tagPrefix));
            Console.WriteLine("\t\t tags={0}", tags.ToString());
            buf.Dispose();
        }

        internal static System.UInt64 StdoutOccupy = 0;  // 当前 stdout 是否被占用
        internal readonly Channel<Common.RentedBuffer> BufferChannel;
        internal LogLevel Level = LogLevel.Info;  // 全局的日志级别
        internal int _flushIntervalMs = 1000;  // 输出日志的间隔时间
        private readonly System.IO.Stream Stdout = Console.OpenStandardOutput();
        internal byte[] tagPrefix = [];

        // Registry for thread-local loggers to support global flush
        private readonly ConcurrentDictionary<ThreadLocalLogger, byte> _registrations = new();
        internal void Register(ThreadLocalLogger logger) {
            _registrations.TryAdd(logger, 0);  // 只在不存在时添加
        }

        internal void Unregister(ThreadLocalLogger logger) {
            _registrations.TryRemove(logger, out _);
        }


        private readonly PeriodicTimer _flushTimer;
        private readonly CancellationTokenSource _cts;
        private readonly Task _consumerTask;
        private readonly Task _timerTask;

        const int maxQueueSize = 1024;

        private Logger() {
            // Channel capacity 1024 as per design
            BufferChannel = Channel.CreateBounded<Common.RentedBuffer>(new BoundedChannelOptions(maxQueueSize) {
                FullMode = BoundedChannelFullMode.Wait, 
                SingleReader = true,
                SingleWriter = false
            });

            _cts = new CancellationTokenSource();
            _flushTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(_flushIntervalMs));
            
            _consumerTask = Task.Run(ConsumeLoop);
            _timerTask = Task.Run(TimerLoop);
        }
        
        private async Task TimerLoop() {
            try {
                while (await _flushTimer.WaitForNextTickAsync(_cts.Token)) {  // 检查退出信号
                    foreach (var kvp in _registrations) {
                        try {
                            // todo: 这里可能会因为加锁导致问题。应该改为 tryLock
                            kvp.Key.SwitchBuffer();
                        } catch {
                            // Ignore flush errors
                        }
                    }
                }
            } catch (OperationCanceledException) {
                // Ignore
            }
        }

        private async Task ConsumeLoop() {
            var reader = BufferChannel.Reader;
            try {
                while (await reader.WaitToReadAsync(_cts.Token)) {
                    while (reader.TryRead(out var buffer)) {
                        if (buffer.Length == 0) {
                            buffer.Dispose();
                            continue;
                        }
                        while (Interlocked.CompareExchange(ref StdoutOccupy, 1, 0) != 0) {
                            await Task.Yield();
                        }
                        // 加锁成功
                        try {
                            //Stdout.WriteAsync(buffer.Bytes())
                            //WriteToStdout(buffer);
                            //byte[] data = Encoding.UTF8.GetBytes("hello\n");
                            Syscall.write(1, buffer.Data!, (ulong)buffer.Length);
                        } finally{
                            // 释放锁
                            Interlocked.CompareExchange(ref StdoutOccupy, 0, 1);
                            buffer.Dispose();  // 借用的内存还回去
                        }
                        // try {
                        //     // Process buffer (Write to stdout for now, Zstd/Http later)
                        //     WriteToStdout(buffer);
                        // } finally {
                        //     buffer.Dispose(); // Return to pool
                        // }
                    }
                }
            } catch (OperationCanceledException) {
                // Ignore
            }
            // todo: 收到退出信号后，再消费一次
        }

        // private void WriteToStdout(Common.RentedBuffer buf) {
        //     if (buf.Length == 0) {
        //         return;
        //     }
        //     // 加锁
        //     Stdout.Wr

        //      var span = buf.Bytes();
        //      if (span.Length > 0) {
        //          try {
        //              using var stdout = Console.OpenStandardOutput();
        //              stdout.Write(span);
        //              stdout.Flush();
        //          } catch {
        //              // Ignore write errors to avoid crashing app
        //          }
        //      }
        // }

        
        // // Configuration
        // public string? JsonLineUrl { get; set; }
        // public string GlobalTags { get; set; } = "";

        // // Global atomic int for stdout lock (0 = free, 1 = busy)
        // internal int StdoutLock = 0;






        public void Dispose() {
            _cts.Cancel();
            _cts.Dispose();
            _flushTimer.Dispose();
            try {
                _consumerTask.Wait(1000);
                _timerTask.Wait(1000);
            } catch { }
        }
    }
}
