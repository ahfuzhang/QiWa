namespace ConsoleLogger
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    using Common;

    internal class BufferWrapper
    {
        internal Common.RentedBuffer Rented;
        internal BufferWrapper(int len)
        {
            Rented = new Common.RentedBuffer(len);
        }
    }

    public partial class ThreadLocalLogger

    {
        //internal static Action<ReadOnlySpan<byte>>? TestOutputCapture;

        private const int ReservedBufferLen = 1024;  // 预留的 buffer 长度
        internal BufferWrapper Buffer;
        private readonly Task timerTask;
        private readonly PeriodicTimer flushTimer;
        private readonly object locker = new object();

        private static readonly ThreadLocal<ThreadLocalLogger> _threadLocal =
            new ThreadLocal<ThreadLocalLogger>(() => new ThreadLocalLogger(), trackAllValues: true);
        internal static ThreadLocalLogger Current => _threadLocal.Value!;

        public ThreadLocalLogger()
        {
            if (Logger.Instance == null)
            {
                throw new Exception("use init first");
            }
            this.Buffer = new BufferWrapper(Logger.Instance.LogBufferSize);
            flushTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(Logger.Instance.FlushIntervalMs));
            timerTask = Task.Run(TimerLoop);
        }

        ~ThreadLocalLogger()
        {
            Buffer.Rented.Dispose();
        }

        internal ref Common.RentedBuffer GetBuffer()
        {
            BufferWrapper w = Volatile.Read(ref this.Buffer);
            return ref w.Rented;
        }

        internal BufferWrapper NewAndGetOld()
        {
            System.Diagnostics.Debug.Assert(Logger.Instance!=null);
            BufferWrapper old = Volatile.Read(ref this.Buffer);
            var newObject = new BufferWrapper(Logger.Instance.LogBufferSize);
            Volatile.Write(ref this.Buffer, newObject);
            return old;
        }

        internal void Flush(ref Common.RentedBuffer buf)
        {
            System.Diagnostics.Debug.Assert(buf.Data!=null);
            if (buf.Length < buf.Data.Length - ReservedBufferLen)
            {
                return;
            }
            // 上层已经加锁了
            var wrapper = NewAndGetOld();
            _ = Task.Run(() =>
            {
                try
                {
                    // if (TestOutputCapture != null)
                    // {
                    //     TestOutputCapture(wrapper.Rented.Data.AsSpan(0, wrapper.Rented.Length));
                    // }
                    // else
                    // {
                        Common.NativeWrite.WriteStdout(wrapper.Rented.Data.AsSpan(0, wrapper.Rented.Length));
                    //}
                }
                finally
                {
                    wrapper.Rented.Dispose();
                }
            });
        }

        private async Task TimerLoop()
        {
            System.Diagnostics.Debug.Assert(Logger.Instance!=null);
            // 疑问：能保证这个 task 一定在当前物理线程执行吗？不一定哦
            try
            {
                while (await flushTimer.WaitForNextTickAsync(Logger.Instance.LoggerToken.Token))
                {
                    Console.WriteLine("timer run");
                    // 检查退出信号，且等待定时器触发
                    BufferWrapper? wrapper;
                    lock (locker)
                    {
                        // 在 buffer 交换期间，一定没有在写入日志
                        var rent = GetBuffer();
                        if (rent.Length == 0)
                        {
                            Console.WriteLine("timer run: empty");
                            continue;
                        }
                        wrapper = NewAndGetOld();
                    }
                    _ = Task.Run(() =>
                    {
                        try
                        {
                            // if (TestOutputCapture != null)
                            // {
                            //     TestOutputCapture(wrapper.Rented.Data.AsSpan(0, wrapper.Rented.Length));
                            // }
                            // else
                            // {
                            Console.WriteLine("timer run: output");
                                Common.NativeWrite.WriteStdout(wrapper.Rented.Data.AsSpan(0, wrapper.Rented.Length));
                            //}
                        }
                        finally
                        {
                            wrapper.Rented.Dispose();
                        }
                    });
                }
            }
            catch (OperationCanceledException err)
            {
                Logger.DiagnosticsLogger.LogError(err, "TimerLoop canceled. IsCancellationRequested={IsCancellationRequested}.", Logger.Instance.LoggerToken.IsCancellationRequested);
            }
        }
    }
}
