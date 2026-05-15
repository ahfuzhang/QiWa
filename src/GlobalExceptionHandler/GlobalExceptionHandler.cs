namespace GlobalExceptionHandler;

/// <summary>
/// 全局未捕获异常处理，注册 AppDomain / TaskScheduler 级别的异常捕获，
/// 并可启动看门狗定时器以触发 UnobservedTaskException 事件。
/// </summary>
public static class GlobalExceptionHandler
{
    /// <summary>用于确保未捕获异常只打印一次的标志位（0=未打印，1=已打印）。</summary>
    private static int _hasPrinted;

    /// <summary>定期触发 GC 以使 TaskScheduler.UnobservedTaskException 能够被捕获的看门狗 Timer。</summary>
    private static Timer? _watchdog;

    /// <summary>
    /// 注册全局未捕获异常处理器，包括 AppDomain 级别、TaskScheduler 级别，
    /// 以及启动看门狗定时器以触发 UnobservedTaskException 事件。
    /// </summary>
    public static void Configure()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) =>
        {
            PrintUnhandledException("AppDomain.CurrentDomain.UnhandledException", eventArgs.ExceptionObject as Exception);
        };

        TaskScheduler.UnobservedTaskException += (_, eventArgs) =>
        {
            PrintUnhandledException("TaskScheduler.UnobservedTaskException", eventArgs.Exception);
        };

#if DEBUG
        // Debug 版本每秒触发一次 GC，尽快暴露未观察的 Task 异常；Release 版本降低频率以避免影响延迟。
        StartWatchdog(TimeSpan.FromSeconds(1));
#else
        //StartWatchdog(TimeSpan.FromSeconds(60));
        // Release 版本没必要扫描。 AppDomain.CurrentDomain.UnhandledException 已经能够捕获错误
#endif
    }

    /// <summary>
    /// 启动看门狗定时器，按指定间隔触发 GC，使被遗忘的 Task 异常能够被 TaskScheduler.UnobservedTaskException 捕获。
    /// </summary>
    /// <param name="interval">GC 扫描间隔，Debug 建议 1s，Release 建议 60s。</param>
    private static void StartWatchdog(TimeSpan interval)
    {
        _watchdog = new Timer(_ =>
        {
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception ex)
            {
                PrintUnhandledException("UnobservedTaskExceptionWatchdog", ex);
            }
        }, null, interval, interval);
    }

    /// <summary>
    /// 打印未捕获的异常信息，使用 ConsoleLogger 输出结构化日志。
    /// 若日志库尚未初始化则回退到 Console.Error。
    /// 通过原子操作保证仅输出一次，之后调用 Environment.Exit 终止进程。
    /// </summary>
    /// <param name="source">异常来源描述。</param>
    /// <param name="exception">异常对象，可为 null。</param>
    private static void PrintUnhandledException(string source, Exception? exception)
    {
        // 保证只输出一次，防止多线程重入
        if (Interlocked.Exchange(ref _hasPrinted, 1) == 1)
            return;

        const int exitCode = 99;

        if (ConsoleLogger.Logger.Instance != null)
        {
            // 日志库已初始化，使用结构化日志输出
            var log = ConsoleLogger.Logger.Get();
            log.Fatal(
                ConsoleLogger.Field.String("event"u8, "unhandled_exception"),
                ConsoleLogger.Field.String("source"u8, source),
                ConsoleLogger.Field.String("error"u8, exception?.ToString() ?? "(null exception)"));
            ConsoleLogger.Logger.Return(log);
            ConsoleLogger.Logger.Shutdown();
        }
        else
        {
            // 日志库尚未初始化，回退到标准错误输出
            Console.Error.WriteLine($"[{DateTimeOffset.UtcNow:u}] Unhandled exception caught from {source}");
            if (exception is null)
                Console.Error.WriteLine("Exception object was null.");
            else
                Console.Error.WriteLine(exception);
            Console.Error.Flush();
        }

        Environment.Exit(exitCode);
    }
}
