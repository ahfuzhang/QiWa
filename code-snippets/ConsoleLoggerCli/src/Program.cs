using System;
using System.Threading;
using System.CommandLine;
using System.Threading.Tasks;
using ConsoleLogger;

internal static class Program {
    private const int UnhandledExceptionExitCode = 99;
    private static int _hasPrintedUnhandledException;
    private static Timer? _unobservedTaskExceptionWatchdog;
    static Option<int?> maxThreads = null!;
    static Program()
    {
        ConfigureGlobalExceptionHandling();
    }

    public static async Task Main(string[] args) {
        // try
        // {
            maxThreads = new Option<int?>("--threadpool.max", "Set ThreadPool maximum worker threads.");
            maxThreads.AddAlias("-threadpool.max");
            var rootCommand = new RootCommand("configuration CLI.");
            rootCommand.AddOption(maxThreads);
            rootCommand.SetHandler(async (int? max) => {
                ConfigureThreadPool(max);
            }, maxThreads);
            var exitCode = await rootCommand.InvokeAsync(args);
            if (exitCode != 0) {
                return;
            }
            // test
            int workThreads;
            int completionPortThreads;
            ThreadPool.GetMaxThreads(out workThreads, out completionPortThreads);
            Console.WriteLine($"workThreads={workThreads}, completionPortThreads={completionPortThreads}");
            //
            testLogger();
        // }
        // catch (Exception ex)
        // {
        //     PrintUnhandledException("Program.Main", ex);
        // }
    }

    static void testLogger()
    {
        const string url= "http://127.0.0.1:9428/insert/jsonline?_time_field=_time&_msg_field=_msg&_stream_fields=level&ignore_fields=&decolorize_fields=&AccountID=0&ProjectID=0&debug=false&extra_fields=";
        Logger.Init(global::ConsoleLogger.LogLevel.Debug, 2000, new Dictionary<string, string>(){{"namespace","backend-team"}}, 1024*4, url);
        ThreadLocalLogger.Current.Info(Field.String("abc"u8, "1234"));
        var l = Logger.Get();
        l.Debug(Field.String("feildxx"u8, "wertyuiop"));
        Logger.Return(l);
        Console.WriteLine("end of testLogger");
        Thread.Sleep(3001);
    }

    private static void ConfigureGlobalExceptionHandling()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) =>
        {
            PrintUnhandledException("AppDomain.CurrentDomain.UnhandledException", eventArgs.ExceptionObject as Exception);
        };

        TaskScheduler.UnobservedTaskException += (_, eventArgs) =>
        {
            PrintUnhandledException("TaskScheduler.UnobservedTaskException", eventArgs.Exception);
        };

        StartUnobservedTaskExceptionWatchdog();
    }

    private static void StartUnobservedTaskExceptionWatchdog()
    {
        _unobservedTaskExceptionWatchdog = new Timer(_ =>
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
        }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
    }

    private static void PrintUnhandledException(string source, Exception? exception)
    {
        if (Interlocked.Exchange(ref _hasPrintedUnhandledException, 1) == 1)
        {
            return;
        }

        Console.Error.WriteLine($"[{DateTimeOffset.UtcNow:u}] Unhandled exception caught from {source}");
        if (exception is null)
        {
            Console.Error.WriteLine("Exception object was null.");
        }
        else
        {
            Console.Error.WriteLine(exception);
        }

        Console.Error.Flush();
        Environment.Exit(UnhandledExceptionExitCode);
    }

    private static void ConfigureThreadPool(int? max) {
        if (max.HasValue) {
            // it must: set min first then set max
            ThreadPool.SetMinThreads(max.Value, max.Value);
            ThreadPool.SetMaxThreads(max.Value, max.Value);
            Console.WriteLine($"set: workThreads={max.Value}, completionPortThreads={max.Value}");
        }
    }
}
