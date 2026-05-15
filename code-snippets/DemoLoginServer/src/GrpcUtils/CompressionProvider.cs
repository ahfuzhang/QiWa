namespace DemoLoginServer.GrpcUtils;

using System.IO;
using System.IO.Compression;
using Grpc.Net.Compression;

// PassthroughCompressionProvider.cs
public class PassthroughCompressionProvider : ICompressionProvider
{
    private readonly string _name;
    public PassthroughCompressionProvider(string name) => _name = name;

    public string EncodingName => _name;

    // 不压缩，直接返回原始流
    public Stream CreateCompressionStream(Stream stream, CompressionLevel? level) => stream;
    // 不解压，直接返回原始流
    public Stream CreateDecompressionStream(Stream stream) => stream;
}
