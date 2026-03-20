namespace ConsoleLogger;

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

internal sealed class BufferWrapper  // 包装一层是为了做原子轮换
{
    internal Common.RentedBuffer Rented;
    internal BufferWrapper(int len)
    {
        Rented = new Common.RentedBuffer(len);
    }
}

public partial class ThreadLocalLogger
{
    private const int ReservedBufferLen = 1024;  // 预留的 buffer 长度
    internal BufferWrapper Buffer;  // 便于做原子轮换. 这个线程上的日志缓冲区
    private readonly Task timerTask;  // 定时器 Task
    private readonly PeriodicTimer flushTimer;  // 定时器
    private readonly object locker = new object();  // 锁
    private readonly HttpClient? httpClient;  // 当使用 jsonline 模式推送日志时，此对象有效

    /// <summary>
    /// 仅供测试使用，用于截获输出文本，避免直接写入 stdout。
    /// </summary>
    internal static Action<string>? TestOutputCapture;

    // ThreadLocal
    private static readonly ThreadLocal<ThreadLocalLogger> _threadLocal =
        new ThreadLocal<ThreadLocalLogger>(() => new ThreadLocalLogger(), trackAllValues: true);
    internal static ThreadLocalLogger Current => _threadLocal.Value!;

    public ThreadLocalLogger()
    {
        if (Logger.Instance == null)
        {
            throw new Exception("use Logger.Init() first");
        }
        this.Buffer = new BufferWrapper(Logger.Instance.LogBufferSize);
        flushTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(Logger.Instance.FlushIntervalMs));
        timerTask = Task.Run(TimerLoop);
        if (Logger.Instance.JsonLineUrl != "")
        {
            httpClient = new HttpClient();
        }
    }

    ~ThreadLocalLogger()
    {
        Buffer.Rented.Dispose();
    }

    /// <summary>
    /// 获取日志的缓冲区
    /// </summary>
    /// <returns></returns>
    internal ref Common.RentedBuffer GetBuffer()
    {
        BufferWrapper w = Volatile.Read(ref this.Buffer);
        return ref w.Rented;
    }

    /// <summary>
    /// 对日志的缓冲区做轮换
    /// </summary>
    /// <returns>旧的缓冲区对象</returns>
    internal BufferWrapper NewAndGetOld()
    {
        // 上层会加锁，这个函数内不要加锁
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        BufferWrapper old = Volatile.Read(ref this.Buffer);
        var newObject = new BufferWrapper(Logger.Instance.LogBufferSize);
        Volatile.Write(ref this.Buffer, newObject);
        return old;
    }

    /// <summary>
    /// 决定要不要对日志缓冲区进行轮换
    /// </summary>
    /// <param name="buf"></param>
    internal void Flush(ref Common.RentedBuffer buf)
    {
        System.Diagnostics.Debug.Assert(buf.Data != null);
        if (TestOutputCapture != null)  // 这部分代码仅仅是为了方便单元测试
        {
            var testWrapper = NewAndGetOld();
            try
            {
                TestOutputCapture(Encoding.UTF8.GetString(testWrapper.Rented.Data!, 0, testWrapper.Rented.Length));
            }
            finally
            {
                testWrapper.Rented.Dispose();
            }
            return;
        }
        if (buf.Length < buf.Data.Length - ReservedBufferLen)
        {
            return;
        }
        // 上层已经加锁了
        var wrapper = NewAndGetOld();
        _ = Task.Run(async () =>
        {
            // ConfigureAwait: 恢复执行时直接在线程池的任意线程上继续，不切换上下文
            await writeLog(wrapper).ConfigureAwait(false);
            wrapper = null;
        });
    }

    private async Task<Common.Error> writeJsonline(BufferWrapper wrapper)
    {
        System.Diagnostics.Debug.Assert(httpClient != null);
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        var (compressed, error) = Compress.ZstdCompressor.Compress(wrapper.Rented.Data.AsSpan(0, wrapper.Rented.Length));
        if (error.Err())
        {
            return error;
        }
        try
        {
            using var ms = new MemoryStream(compressed.Data!, 0, compressed.Length, false, true);
            using var content = new StreamContent(ms);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.ContentEncoding.Add("zstd");
            try
            {
                // see: https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.postasync?view=net-10.0#system-net-http-httpclient-postasync(system-uri-system-net-http-httpcontent)
                using var response = await httpClient.PostAsync(Logger.Instance.JsonLineUrl, content, Logger.Instance.LoggerToken.Token).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    return new Common.Error { Code = 1, Message = $"response code={response.StatusCode}, url={Logger.Instance.JsonLineUrl}" };
                }
                return default;
            }
            catch (Exception ex) when (
                ex is HttpRequestException ||
                ex is OperationCanceledException
            )
            {
                return new Common.Error { Code = 2, Message = $"exception={ex.Message}, url={Logger.Instance.JsonLineUrl}" };
            }
        }
        finally
        {
            compressed.Dispose();
        }
    }

    private async Task writeLog(BufferWrapper wrapper)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        try
        {
            if (Logger.Instance.JsonLineUrl != "")
            {
                var err = await writeJsonline(wrapper).ConfigureAwait(false);
                if (!err.Err())
                {
                    return;
                }
                Logger.DiagnosticsLogger.LogError(null,
                    $"writeJsonline fail: code={err.Code}, msg={err.Message}");
            }
            var outputCapture = TestOutputCapture;
            if (outputCapture != null)
            {
                outputCapture(Encoding.UTF8.GetString(wrapper.Rented.Data!, 0, wrapper.Rented.Length));
                return;
            }
            Common.NativeWrite.WriteStdout(wrapper.Rented.Data.AsSpan(0, wrapper.Rented.Length));
        }
        finally
        {
            wrapper.Rented.Dispose();
        }
    }

    private async Task TimerLoop()
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        try
        {
            while (await flushTimer.WaitForNextTickAsync(Logger.Instance.LoggerToken.Token).ConfigureAwait(false))
            {
                // 检查退出信号，且等待定时器触发
                BufferWrapper? wrapper;
                lock (locker)
                {
                    // 在 buffer 交换期间，一定没有在写入日志
                    var rent = GetBuffer();
                    if (rent.Length == 0)
                    {
                        continue;
                    }
                    wrapper = NewAndGetOld();
                }
                _ = Task.Run(async () =>
                {
                    await writeLog(wrapper).ConfigureAwait(false);
                    wrapper = null;
                });
            }
        }
        catch (OperationCanceledException err)
        {
            // Prompt intent: `make test` should not print false failure logs during normal logger shutdown.
            if (Logger.Instance == null || Logger.Instance.LoggerToken.IsCancellationRequested)
            {
                return;
            }
            var exceptionLocation = Common.Utils.GetExceptionLocation(err);
            Logger.DiagnosticsLogger.LogError(err, "TimerLoop canceled. IsCancellationRequested={IsCancellationRequested}. ExceptionLocation={ExceptionLocation}.",
                Logger.Instance.LoggerToken.IsCancellationRequested, exceptionLocation);
        }
    }
}
