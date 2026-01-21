using System;
using System.Collections.Generic;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Diagnosers;

namespace Benchmarks;

[MemoryDiagnoser]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[Config(typeof(Config))]
public class ParseTextUtf8Bench {
    private byte[] _data = Array.Empty<byte>();
    private Dictionary<string, string> _tags = new Dictionary<string, string>();

    private class Config : ManualConfig
    {
        public Config()
        {
            // 生成 CPU profile 的 .nettrace 文件
            AddDiagnoser(new EventPipeProfiler(EventPipeProfile.CpuSampling));
        }
    }

    [GlobalSetup]
    public void Setup() {
        string path = FindMetricsPath();
        _data = File.ReadAllBytes(path);
        _tags = new Dictionary<string, string> {
            ["namespace"] = "default",
            ["app"] = "textparse",
            ["pod"] = "textparse-0",
        };
    }

    [Benchmark]
    public byte[] ParseUtf8ByNeon() {
        return global::Program.parseTextUtf8ByNeon(_data, _tags);
    }

    [Benchmark]
    public byte[] ParseUtf8() {
        return global::Program.parseTextUtf8(_data, _tags);
    }

    [Benchmark(Baseline = true)]
    public byte[] ParseUtf16() {
        return global::Program.parseText(_data, _tags);
    }

    private static string FindMetricsPath() {
        DirectoryInfo? current = new DirectoryInfo(AppContext.BaseDirectory);
        for (int i = 0; i < 10 && current != null; i++) {
            string candidate = Path.Combine(current.FullName, "Tests", "data", "metrics.txt");
            if (File.Exists(candidate)) {
                return candidate;
            }
            current = current.Parent;
        }
        throw new FileNotFoundException("Tests/data/metrics.txt not found.");
    }
}
