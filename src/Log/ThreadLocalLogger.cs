using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Log;

public class ThreadLocalLogger
{
    private ulong _locker;
#pragma warning disable CS0169
    // avoid false sharing
    [SuppressMessage(
        "Usage",
        "IDE0051:Remove unused private members",
        Justification = "Used via reflection")]
    private readonly System.UInt64 _pad2, _pad7;
#pragma warning restore CS0169

    //private const int FlushThreshold = 128 * 1024;  // todo: 要不要修改成可配置的?
    private const int reservedBufferLen = 1024;  // 预留的 buffer 长度
    //private const int minLogBufferLen = 1024*4;  // 日志缓冲区的最小长度

    internal Common.RentedBuffer Buffer;

    private static readonly ThreadLocal<ThreadLocalLogger> _threadLocal =
        new ThreadLocal<ThreadLocalLogger>(() => new ThreadLocalLogger(), trackAllValues: true);
    internal static ThreadLocalLogger Current => _threadLocal.Value!;

    internal ThreadLocalLogger()
    {
        Debug.Assert(Logger.Instance != null);
        Buffer = new Common.RentedBuffer(Logger.Instance.LogBufferSize);
        Logger.Instance.Register(this);  // 注册到全局
    }

    ~ThreadLocalLogger()
    {
        Debug.Assert(Logger.Instance != null);
        Logger.Instance.Unregister(this);  // 从全局注销
        Lock();
        Flush();
        Thread.Sleep(10);  // 等一会儿，避免 Logger 中还在对这个对象有引用
    }

    internal void Lock()
    {
        while (Interlocked.CompareExchange(ref _locker, 1, 0) != 0)
        {
            // spin lock
        }
    }

    internal void UnLock()
    {
        while (Interlocked.CompareExchange(ref _locker, 0, 1) != 1)
        {
            // spin lock
        }
    }

    // 每次写日志后 Flush
    internal void Flush()
    {
        if (Buffer.Length < Buffer.Data!.Length - reservedBufferLen)
        {
            return;
        }
        if (Logger.Instance!.OverloadPolicy == OverloadPolicy.Direct)
        {
            lock (Logger.Instance.Locker)
            {
                Common.NativeWrite.WriteStdout(Buffer.Data.AsSpan(0, Buffer.Length));
                Buffer.Length = 0;
            }
            return;
        }
        // 抢占 stdout 的锁
        bool taken = Monitor.TryEnter(Logger.Instance!.Locker);
        if (!taken)
        {
            SwitchBuffer();
            return;
        }
        // if (Interlocked.CompareExchange(ref Logger.StdoutOccupy, 1, 0) != 0) {
        //     SwitchBuffer();
        //     return;
        // }
        try
        {
            // todo: 计时，并记录字节数
            Common.NativeWrite.WriteStdout(Buffer.Data.AsSpan(0, Buffer.Length));
            Buffer.Length = 0;  // 写完后，清空 buffer
        }
        finally
        {
            // 释放 stdout 的锁
            //Interlocked.CompareExchange(ref Logger.StdoutOccupy, 0, 1);
            Monitor.Exit(Logger.Instance!.Locker);
        }
    }

    internal void SwitchBuffer()
    {
        Debug.Assert(Logger.Instance != null);
        if (Buffer.Length == 0)
        {
            return;
        }
        // 要先检查 overload 的策略
        Debug.Assert(Logger.Instance.BufferChannel != null);
        var writer = Logger.Instance.BufferChannel.Writer;
        if (Logger.Instance.OverloadPolicy == OverloadPolicy.Block)
        {
            if (!writer.TryWrite(Buffer))
            {
                Thread.Yield();  // 因为马上要阻塞 os thread，所以先放弃掉时间片
                // 这里会阻塞 os thread，直到写入 channel 成功
                writer.WriteAsync(Buffer).GetAwaiter().GetResult();
            }
        }
        else if (Logger.Instance.OverloadPolicy == OverloadPolicy.Drop)
        {
            if (!writer.TryWrite(Buffer))
            {
                Buffer.Length = 0;
                return;
            }
        }
        else
        {
            throw new NotImplementedException();
        }
        Buffer = new Common.RentedBuffer(Logger.Instance.LogBufferSize);
    }

    internal bool SwitchBufferOnTime()
    {
        Debug.Assert(Logger.Instance != null);
        // 这个方法专门提供给 Logger 对象使用
        // 尝试加锁
        if (Interlocked.CompareExchange(ref _locker, 1, 0) != 0)
        {
            return false;
            // 上层对加锁失败的 thread 做额外的异步通知
        }
        try
        {
            // 查看队列的策略
            var writer = Logger.Instance!.BufferChannel.Writer;
            if (Logger.Instance.OverloadPolicy == OverloadPolicy.Block)
            {
                if (!writer.TryWrite(Buffer))
                {
                    // 因为队列满，而通知失败
                    return false;
                }
            }
            else if (Logger.Instance.OverloadPolicy == OverloadPolicy.Drop)
            {
                if (!writer.TryWrite(Buffer))
                {
                    Buffer.Length = 0;
                    return true;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
            Buffer = new Common.RentedBuffer(Logger.Instance.LogBufferSize);
            return true;
        }
        finally
        {
            // 释放锁
            Interlocked.CompareExchange(ref _locker, 0, 1);
        }
    }
}
