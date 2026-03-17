namespace ConsoleLogger;

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

internal sealed class BufferWrapper
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
    internal BufferWrapper Buffer;
    private readonly Task timerTask;
    private readonly PeriodicTimer flushTimer;
    private readonly object locker = new object();
    private readonly HttpClient? httpClient;
    /// <summary>
    /// 仅供测试使用，用于截获输出文本，避免直接写入 stdout。
    /// </summary>
    internal static Action<string>? TestOutputCapture;

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
        if (Logger.Instance.JsonLineUrl != "")
        {
            httpClient = new HttpClient();
        }
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
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        BufferWrapper old = Volatile.Read(ref this.Buffer);
        var newObject = new BufferWrapper(Logger.Instance.LogBufferSize);
        Volatile.Write(ref this.Buffer, newObject);
        return old;
    }

    internal void Flush(ref Common.RentedBuffer buf)
    {
        System.Diagnostics.Debug.Assert(buf.Data != null);
        if (TestOutputCapture != null)
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
            await writeLog(wrapper).ConfigureAwait(false);
            wrapper = null;
        });
    }

    private async Task<Common.Error> writeJsonline(BufferWrapper wrapper)
    {
        System.Diagnostics.Debug.Assert(httpClient != null);
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        //Console.WriteLine("writeJsonline:");
        var (compressed, error) = Compress.ZstdCompressor.Compress(wrapper.Rented.Data.AsSpan(0, wrapper.Rented.Length));
        if (error.Err())
        {
            //Console.WriteLine("\twriteJsonline: Compress error");
            return error;
        }
        try
        {
            using var ms = new MemoryStream(compressed.Data!, 0, compressed.Length, false, true);
            using var content = new StreamContent(ms);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.ContentEncoding.Add("zstd");
            //Console.WriteLine("\twriteJsonline: ready to post");
            try
            {
                // see: https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.postasync?view=net-10.0#system-net-http-httpclient-postasync(system-uri-system-net-http-httpcontent)
                using var response = await httpClient.PostAsync(Logger.Instance.JsonLineUrl, content, Logger.Instance.LoggerToken.Token).ConfigureAwait(false);
                //Console.WriteLine("\twriteJsonline: post end");
                if (!response.IsSuccessStatusCode)
                {
                    //Console.WriteLine("\twriteJsonline: post fail");
                    return new Common.Error { Code = 1, Message = $"response code={response.StatusCode}, url={Logger.Instance.JsonLineUrl}" };
                }
                //Console.WriteLine("\twriteJsonline: post success");
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
                //Console.WriteLine("writeLog: jsonline");
                var err = await writeJsonline(wrapper).ConfigureAwait(false);
                if (!err.Err())
                {
                    //Console.WriteLine("\twriteLog: jsonline, success");
                    return;
                }
                //Console.WriteLine("\twriteLog: jsonline, fail");
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
        // 疑问：能保证这个 task 一定在当前物理线程执行吗？不一定哦
        try
        {
            while (await flushTimer.WaitForNextTickAsync(Logger.Instance.LoggerToken.Token).ConfigureAwait(false))
            {
                //Console.WriteLine("run timer:");
                // 检查退出信号，且等待定时器触发
                BufferWrapper? wrapper;
                lock (locker)
                {
                    // 在 buffer 交换期间，一定没有在写入日志
                    var rent = GetBuffer();
                    if (rent.Length == 0)
                    {
                        //Console.WriteLine("\trun timer:no data");
                        continue;
                    }
                    wrapper = NewAndGetOld();
                }
                _ = Task.Run(async () =>
                {
                    //Console.WriteLine("\trun timer: writeLog");
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
            var exceptionLocation = GetExceptionLocation(err);
            Logger.DiagnosticsLogger.LogError(err, "TimerLoop canceled. IsCancellationRequested={IsCancellationRequested}. ExceptionLocation={ExceptionLocation}.",
                Logger.Instance.LoggerToken.IsCancellationRequested, exceptionLocation);
        }
    }

    private static string GetExceptionLocation(Exception err)
    {
        if (err == null)
        {
            return string.Empty;
        }
        var trace = new StackTrace(err, true);
        var frames = trace.GetFrames();
        if (frames == null || frames.Length == 0)
        {
            return string.Empty;
        }
        foreach (var frame in frames)
        {
            var file = frame.GetFileName();
            var line = frame.GetFileLineNumber();
            if (!string.IsNullOrEmpty(file) && line > 0)
            {
                return $"file={file}, line={line}";
            }
        }
        return string.Empty;
    }
}
