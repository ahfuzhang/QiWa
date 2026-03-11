using System.IO;
using System.IO.Compression;
using Grpc.Net.Compression;
using ZstdSharp;

/// <summary>
/// 为 gRPC 提供 zstd 压缩支持的 <see cref="ICompressionProvider"/> 实现。
/// </summary>
internal sealed class ZstdCompressionProvider : ICompressionProvider
{
    public string EncodingName => "zstd";

    public Stream CreateCompressionStream(Stream stream, CompressionLevel? compressionLevel)
    {
        int level = compressionLevel switch
        {
            CompressionLevel.Fastest => 1,
            CompressionLevel.Optimal => 9,
            CompressionLevel.SmallestSize => 22,
            _ => 3,
        };
        return new CompressionStream(stream, level, leaveOpen: true);
    }

    public Stream CreateDecompressionStream(Stream stream) =>
        new DecompressionStream(stream, leaveOpen: true);
}
