using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Benchmarks;

class Program
{
    static void Main()
    {
        var config = DefaultConfig.Instance.WithArtifactsPath("../../build/benchmarks");
        BenchmarkRunner.Run<RentBufferBench>(config);
    }
}
