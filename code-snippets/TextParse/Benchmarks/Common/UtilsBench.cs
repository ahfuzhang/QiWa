using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Order;

namespace Benchmarks;

[MemoryDiagnoser]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class RentBufferBench {
    [Benchmark]
    public void RentAndReturn() {
        Common.RentedBuffer buf = default;
        int n = System.Random.Shared.Next(1, 65536);
        buf.Rent(n);
        buf.Data![System.Random.Shared.Next(0, n)] = (byte)'a';
        buf.Dispose();
    }

    [Benchmark(Baseline = true)]
    public void DirectNew() {
        int n = System.Random.Shared.Next(1, 65536);
        byte[] arr = new byte[n];
        arr[System.Random.Shared.Next(0, n)] = (byte)'b';
    }
}
