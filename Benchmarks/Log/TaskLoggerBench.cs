using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Order;
using Log;

namespace Benchmarks;

[MemoryDiagnoser]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class TaskLoggerBench {
    private TaskLogger _logger = null!;

    [GlobalSetup]
    public void Setup() {
        if (Logger.Instance != null) {
            try { Logger.Shutdown(); } catch { }
        }
        Logger.Init(
            level: LogLevel.Info,
            flushIntervalMs: 1000,
            overload: OverloadPolicy.Direct,
            queueSize: 1,
            logBufferSize: 1024 * 4
        );
        _logger = new TaskLogger();
    }

    [GlobalCleanup]
    public void Cleanup() {
        if (Logger.Instance != null) {
            try { Logger.Shutdown(); } catch { }
        }
    }

    [Benchmark]
    public void Info_SingleField() {
        _logger.Info(Field.String("msg"u8, "benchmark"));
        // Reset the current thread buffer so Flush won't hit stdout on long runs.
        //ThreadLocalLogger.Current.Buffer.Length = 0;
    }

    [Benchmark(Baseline = true)]
    public void DirectOutput() {
        string value = "benchmark";
        Console.WriteLine($"{{\"msg\":\"{value}\"}}");
    } 
}
