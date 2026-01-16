using System;
using System.Threading;
using System.CommandLine;
using System.Threading.Tasks;

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
        return;
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
