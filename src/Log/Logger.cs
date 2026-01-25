using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Common;
using System.Text;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Log {
    /// <summary>
    /// 日志库支持的日志级别
    /// </summary>
    public enum LogLevel {
        Fatal,
        Error,
        Warn,
        Info,
        Debug
    }

    /// <summary>
    /// 系统过载的时候，使用的策略
    /// </summary>
    public enum OverloadPolicy {
        /// <summary>
        /// 写入到 Channel 的时候，阻塞线程
        /// </summary>
        Block,

        /// <summary>
        /// Channel 满的时候，丢弃当前的日志块
        /// </summary>
        Drop
    }

    /// <summary>
    /// 全局日志对象
    /// </summary>
    public class Logger : IDisposable {
        internal static System.UInt64 StdoutOccupy = 0;  // 当前 stdout 是否被占用
#pragma warning disable CS0169
        // avoid false sharing
        [SuppressMessage(
            "Usage",
            "IDE0051:Remove unused private members",
            Justification = "Used via reflection")]
        private static System.UInt64 _pad1_, _pad2_, _pad3_, _pad4_, _pad5_, _pad6_, _pad7_;
#pragma warning restore CS0169

        internal static Logger? Instance = null;

        const int maxQueueSize = 1024;
        const int minLogFlushIntervalMs = 100;
        const int defaultLogBufferSize = 1024 * 128;
        const int minLogBufferSize = 1024 * 4;
        const int defaultFlushIntervalMs = 1000;

        private int flushIntervalMs = defaultFlushIntervalMs;  // 输出日志的间隔时间
        private readonly PeriodicTimer flushTimer;

        private readonly Task consumerTask;
        private readonly Task timerTask;

        internal readonly CancellationTokenSource LoggerToken;
        internal readonly Channel<Common.RentedBuffer> BufferChannel;
        internal LogLevel Level = LogLevel.Info;  // 全局的日志级别
        internal byte[] TagPrefix = [];
        internal OverloadPolicy OverloadPolicy;
        internal int LogBufferSize = defaultLogBufferSize;

        // Registry for thread-local loggers to support global flush
        private readonly ConcurrentDictionary<ThreadLocalLogger, byte> registrations = new();


        private static readonly ILogger DiagnosticsLogger = LoggerFactory.Create(builder => {
            builder.AddConsole();
            builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Error);
        }).CreateLogger<Logger>();

        /// <summary>
        /// 使用日志库前，必须使用此函数进行初始化
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="flushIntervalMs">日志的 flush 时间</param>
        /// <param name="overload">过载时的处理策略</param>
        /// <param name="tags">公共的 tags </param>
        /// <param name="logBufferSize">日志缓冲区的大小</param>
        public static void Init(LogLevel level = LogLevel.Warn, int flushIntervalMs = 1000,
                Dictionary<string, string>? tags = null,
                OverloadPolicy overload = OverloadPolicy.Block,
                int queueSize = maxQueueSize,
                int logBufferSize = defaultLogBufferSize) {
            if (queueSize < 1) {
                queueSize = 1;
            }
            Instance = new Logger(overload, queueSize);
            Instance.Level = level;
            if (flushIntervalMs < minLogFlushIntervalMs) {
                flushIntervalMs = minLogFlushIntervalMs;
            }
            Instance.flushIntervalMs = flushIntervalMs;
            if (tags != null && tags.Count > 0) {
                Instance.SetGlobalTags(tags);
            }
            if (logBufferSize < minLogBufferSize) {
                logBufferSize = minLogBufferSize;
            }
            Instance.LogBufferSize = logBufferSize;
        }

        private Logger(OverloadPolicy overload, int queueSize) {
            OverloadPolicy = overload;
            BufferChannel = Channel.CreateBounded<Common.RentedBuffer>(new BoundedChannelOptions(queueSize) {
                FullMode = BoundedChannelFullMode.Wait,  // channel 的阻塞策略，可以配置的
                SingleReader = true,
                SingleWriter = false
            });

            LoggerToken = new CancellationTokenSource();
            flushTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(flushIntervalMs));

            consumerTask = Task.Run(ConsumeLoop);
            timerTask = Task.Run(TimerLoop);
        }

        /// <summary>
        /// 允许动态设置日志级别
        /// </summary>
        /// <param name="level"></param>
        public static void SetLevel(LogLevel level) {
            Debug.Assert(Instance != null);
            Instance.Level = level;
        }

        public static void SetFlushIntervalMs(int ms) {
            Debug.Assert(Instance != null);
            if (ms < minLogFlushIntervalMs) {
                ms = minLogFlushIntervalMs;
            }
            Instance.flushIntervalMs = ms;
        }

        /// <summary>
        /// 进程退出前调用这个，把日志输出到 stdout
        /// </summary>
        public static void Shutdown() {
            Debug.Assert(Instance != null);
            Instance.Dispose();
        }

        private void SetGlobalTags(Dictionary<string, string> tags) {
            Common.RentedBuffer buf = new(512);
            buf.Append((byte)'{');
            bool isFirst = true;
            foreach (var (key, value) in tags) {
                if (isFirst) {
                    isFirst = false;
                }
                else {
                    buf.Append((byte)',');
                }
                Field.String(Encoding.UTF8.GetBytes(key), value).WriteTo(ref buf);
            }
            TagPrefix = new byte[buf.Length];
            Array.Copy(buf.Data!, TagPrefix, buf.Length);
            buf.Dispose();
        }

        internal void Register(ThreadLocalLogger logger) {
            registrations.TryAdd(logger, 0);  // 只在不存在时添加
        }

        internal void Unregister(ThreadLocalLogger logger) {
            registrations.TryRemove(logger, out _);
        }

        private async Task Consume() {
            var reader = BufferChannel.Reader;
            while (reader.TryRead(out var buffer)) {
                if (buffer.Length == 0) {
                    buffer.Dispose();
                    continue;
                }
                // 占用 stdout
                // todo: 记录等锁的时间
                while (Interlocked.CompareExchange(ref StdoutOccupy, 1, 0) != 0) {
                    await Task.Yield();
                }
                // 加锁成功
                try {
                    // todo: 记录 io 的总时间
                    Common.NativeWrite.WriteStdout(buffer.Data.AsSpan(0, buffer.Length));
                }
                finally {
                    // 释放锁
                    Interlocked.CompareExchange(ref StdoutOccupy, 0, 1);
                    buffer.Dispose();  // 借用的内存还回去
                }
            }
        }

        private async Task ConsumeLoop() {
            var reader = BufferChannel.Reader;
            try {
                while (await reader.WaitToReadAsync(LoggerToken.Token)) {
                    // channel 为空的时候，不会进入这里
                    // 因此不必担心会引发死循环
                    await Consume();
                }
                // 收到退出通知后，再消费一次
                // 不能保障后面有数据
                await Consume();
            }
            catch (OperationCanceledException err) {
                DiagnosticsLogger.LogError(err, "ConsumeLoop canceled. IsCancellationRequested={IsCancellationRequested}.", LoggerToken.IsCancellationRequested);
            }
        }

        public void Dispose() {
            LoggerToken.Cancel();
            LoggerToken.Dispose();
            flushTimer.Dispose();
            try {
                consumerTask.Wait(1000);
                timerTask.Wait(1000);
            }
            catch { }
        }

        /// <summary>
        /// 定时器。到时间后，让每个线程把数据丢到 channel
        /// </summary>
        /// <returns></returns>
        private async Task TimerLoop() {
            try {
                while (await flushTimer.WaitForNextTickAsync(LoggerToken.Token)) {  // 检查退出信号，且等待定时器触发
                    var list = new List<ThreadLocalLogger>();
                    foreach (var kvp in registrations) {
                        if (!kvp.Key.SwitchBufferOnTime()) {
                            list.Add(kvp.Key);
                        }
                    }
                    // 如果存在通知失败，再次通知
                    if (list.Count > 0) {
                        _ = Task.Run(async () => {
                            await Task.Delay(1);
                            // todo: 可能的风险 ——— 在 1 毫秒内，os thread 退出了。这里导致了一个空对象引用。
                            foreach (var item in list) {
                                item.SwitchBufferOnTime();
                                // todo: 就算我等了一毫秒，也仍然可能导致通知失败。
                                // 1. 只能等下个周期再通知
                                // 2. 只能等 buffer 写满
                            }
                        });
                    }
                }
            }
            catch (OperationCanceledException err) {
                DiagnosticsLogger.LogError(err, "TimerLoop canceled. IsCancellationRequested={IsCancellationRequested}.", LoggerToken.IsCancellationRequested);
            }
        }
    }
}
