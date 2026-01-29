using System;
using System.Threading;
using System.CommandLine;
using System.Threading.Tasks;
using ConsoleLogger;

internal static class Program {
    static Option<int?> maxThreads;
    public static async Task Main(string[] args) {
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
        return;
    }

    static void testLogger()
    {
        Logger.Init(global::ConsoleLogger.LogLevel.Debug, 1000, new Dictionary<string, string>(){{"namespace","backend-team"}}, 1024*4);
        ThreadLocalLogger.Current.Info(Field.String("abc"u8, "1234"));
        var l = Logger.Get();
        l.Debug(Field.String("feildxx"u8, "wertyuiop"));
        Logger.Return(l);
        Thread.Sleep(2001);
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
