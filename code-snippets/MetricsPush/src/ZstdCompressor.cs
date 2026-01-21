using ZstdSharp;

namespace MetricsPush;

internal static class ZstdCompressor
{
    [ThreadStatic]
    private static Compressor? _compressor;

    public static byte[] Compress(ReadOnlySpan<byte> input)
    {
        var compressor = _compressor ??= new Compressor();
        return compressor.Wrap(input).ToArray();
    }
}
