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
using Microsoft.Extensions.ObjectPool;

namespace ConsoleLogger
{
    /// <summary>
    /// 日志库支持的日志级别
    /// </summary>
    public enum LogLevel
    {
        Fatal,
        Error,
        Warn,
        Info,
        Debug
    }

    public class Logger
    {
        internal static Logger? Instance = null;
        const int minLogFlushIntervalMs = 100;
        const int defaultLogBufferSize = 1024 * 128;
        const int minLogBufferSize = 1024 * 4;
        const int defaultFlushIntervalMs = 1000;
        internal int FlushIntervalMs = defaultFlushIntervalMs;  // 输出日志的间隔时间
        internal LogLevel Level = LogLevel.Info;  // 全局的日志级别
        internal readonly byte[] TagPrefix = [];
        internal readonly int LogBufferSize = defaultLogBufferSize;
        internal readonly CancellationTokenSource LoggerToken;

        internal static readonly ILogger DiagnosticsLogger = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Error);
        }).CreateLogger<Logger>();

        // 内存池
        DefaultObjectPool<TaskLogger> pool;


        public static void Init(LogLevel level = LogLevel.Warn, int flushIntervalMs = 1000,
            Dictionary<string, string>? tags = null,
            int logBufferSize = defaultLogBufferSize)
        {
            Instance = new Logger(logBufferSize, tags);
            Instance.Level = level;
            if (flushIntervalMs < minLogFlushIntervalMs)
            {
                flushIntervalMs = minLogFlushIntervalMs;
            }
            Instance.FlushIntervalMs = flushIntervalMs;
            // if (tags != null && tags.Count > 0)
            // {
            //     Instance.SetGlobalTags(tags);
            // }
            // if (logBufferSize < minLogBufferSize)
            // {
            //     logBufferSize = minLogBufferSize;
            // }
            // Instance.LogBufferSize = logBufferSize;
        }

        class BufferPolicy : PooledObjectPolicy<TaskLogger>
        {
            public override TaskLogger Create()
                => new TaskLogger();
            const int maxBufferSize = 1024 * 4;
            public override bool Return(TaskLogger l)
            {
                if (l == null || l.prefix.Data==null)
                {
                    return false;
                }
                if (l.prefix.Data.Length >= maxBufferSize)
                {
                    l.prefix.Dispose();
                    return false;
                }
                l.prefix.Length = 0;
                return true; // true = 放回池
            }
        }

        internal Logger(int logBufferSize, Dictionary<string, string>? tags)
        {
            if (logBufferSize < minLogBufferSize)
            {
                logBufferSize = minLogBufferSize;
            }
            LogBufferSize = logBufferSize;
            //TagPrefix = ;
            if (tags != null && tags.Count > 0)
            {
                TagPrefix = SetGlobalTags(tags);
            }
            LoggerToken = new CancellationTokenSource();
            pool = new DefaultObjectPool<TaskLogger>(new BufferPolicy());
        }

        public static TaskLogger Get()
        {
            Debug.Assert(Logger.Instance != null);
            var l = Logger.Instance.pool.Get();
            l.prefix.Length = 0;
            return l;
        }

        public static void Return(TaskLogger l)
        {
            Debug.Assert(Logger.Instance != null);
            Logger.Instance.pool.Return(l);
        }

        internal void Dispose()
        {
            LoggerToken.Cancel();
            LoggerToken.Dispose();
        }

        public static void Shutdown()
        {
            Debug.Assert(Instance != null);
            Instance.Dispose();
        }

        private static byte[] SetGlobalTags(Dictionary<string, string> tags)
        {
            Common.RentedBuffer buf = new(512);
            buf.Append((byte)'{');
            bool isFirst = true;
            foreach (var (key, value) in tags)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    buf.Append((byte)',');
                }
                Field.String(Encoding.UTF8.GetBytes(key), value).WriteTo(ref buf);
            }
            var temp = new byte[buf.Length];
            Array.Copy(buf.Data!, temp, buf.Length);
            buf.Dispose();
            return temp;
        }

        public static void SetLevel(LogLevel level)
        {
            Debug.Assert(Instance != null);
            Instance.Level = level;
        }

        public static void SetFlushIntervalMs(int ms)
        {
            Debug.Assert(Instance != null);
            if (ms < minLogFlushIntervalMs)
            {
                ms = minLogFlushIntervalMs;
            }
            Instance.FlushIntervalMs = ms;
        }
    }
}
