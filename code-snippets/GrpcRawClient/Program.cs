// RawGrpcClient.cs
using System.IO.Compression; // needed by ZstdCompressionProvider and GzipCompressionProvider
using System.Runtime.CompilerServices;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Compression;

/// <summary>
/// 底层 gRPC 客户端：不依赖代码生成，收发原始字节，支持 gzip/zstd
/// </summary>
public sealed class RawGrpcClient : IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly CallInvoker _invoker;

    // 恒等 Marshaller：byte[] 直接透传，不做任何序列化
    private static readonly Marshaller<byte[]> BytesMarshaller = new(
        serializer: bytes => bytes,
        deserializer: bytes => bytes
    );

    /// <param name="host">目标 IP 或域名</param>
    /// <param name="port">端口</param>
    /// <param name="useTls">是否 TLS</param>
    public RawGrpcClient(string host, int port, bool useTls = false)
    {
        var scheme = useTls ? "https" : "http";
        var options = new GrpcChannelOptions
        {
            CompressionProviders = new List<ICompressionProvider>
            {
                new GzipCompressionProvider(CompressionLevel.Fastest),
                new ZstdCompressionProvider(),
            }
        };
        _channel = GrpcChannel.ForAddress($"{scheme}://{host}:{port}", options);
        _invoker = _channel.CreateCallInvoker();
    }

    /// <summary>Unary 调用：发送预序列化字节，返回原始响应字节</summary>
    /// <param name="serviceName">如 "helloworld.Greeter"</param>
    /// <param name="methodName">如 "SayHello"</param>
    /// <param name="compression">"gzip" / "zstd" / null</param>
    public async Task<byte[]> UnaryCallAsync(
        string serviceName,
        string methodName,
        byte[] requestBytes,
        string? compression = null,
        CancellationToken ct = default)
    {
        var method = MakeMethod(MethodType.Unary, serviceName, methodName);
        var options = BuildCallOptions(compression, ct);
        using var call = _invoker.AsyncUnaryCall(method, null, options, requestBytes);
        return await call.ResponseAsync;
    }

    /// <summary>Server-Streaming 调用</summary>
    public async IAsyncEnumerable<byte[]> ServerStreamingCallAsync(
        string serviceName,
        string methodName,
        byte[] requestBytes,
        string? compression = null,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var method = MakeMethod(MethodType.ServerStreaming, serviceName, methodName);
        var options = BuildCallOptions(compression, ct);
        using var call = _invoker.AsyncServerStreamingCall(method, null, options, requestBytes);
        await foreach (var item in call.ResponseStream.ReadAllAsync(ct))
            yield return item;
    }

    // ---- 私有辅助 ----

    private static Method<byte[], byte[]> MakeMethod(
        MethodType type, string service, string method) =>
        new(type, service, method, BytesMarshaller, BytesMarshaller);

    private static CallOptions BuildCallOptions(string? compression, CancellationToken ct)
    {
        var headers = new Metadata();
        if (compression != null)
        {
            headers.Add("grpc-encoding", compression);          // 请求体压缩
            headers.Add("grpc-accept-encoding", compression);   // 期望响应也压缩
        }
        return new CallOptions(headers: headers, cancellationToken: ct);
    }

    public void Dispose() => _channel.Dispose();
}

/// <summary>Zstd 压缩 Provider（基于 ZstdSharp.Port）</summary>
public sealed class ZstdCompressionProvider : ICompressionProvider
{
    private readonly int _level;
    public ZstdCompressionProvider(int level = 3) => _level = level;
    public string EncodingName => "zstd";

    public Stream CreateCompressionStream(Stream stream, CompressionLevel? _)
        => new ZstdSharp.CompressionStream(stream, _level);

    public Stream CreateDecompressionStream(Stream stream)
        => new ZstdSharp.DecompressionStream(stream);
}

// ---- 使用示例 ----
class Program
{
    static async Task Main(string[] args)
    {
        var addr = args.Select(a => a.StartsWith("-addr=") ? a["-addr=".Length..] : null)
                       .FirstOrDefault(v => v != null) ?? "127.0.0.1:50051";
        var parts = addr.Split(':');
        if (parts.Length != 2 || !int.TryParse(parts[1], out int port))
        {
            Console.Error.WriteLine("Usage: -addr=ip:port");
            return;
        }
        using var client = new RawGrpcClient(parts[0], port);

        // 调用者自己序列化 protobuf（可用 Google.Protobuf / MemoryPack 等）
        // 此处手工编码演示：HelloRequest { string name = 1; } = "World"
        byte[] req = ProtoEncodeString(fieldNumber: 1, value: new string('A', 1024));

        // Unary 调用，启用 gzip 压缩
        byte[] resp = await client.UnaryCallAsync(
            serviceName: "greet.Greeter",
            methodName:  "SayHello",
            requestBytes: req,
            compression: "gzip");

        // 调用者自己反序列化
        string msg = ProtoDecodeString(resp, fieldNumber: 1);
        Console.WriteLine($"Response: {msg}");

        // Server-streaming 示例（换 zstd）
        // try
        // {
        //     await foreach (var chunk in client.ServerStreamingCallAsync(
        //         "helloworld.Greeter", "SayHelloStream", req, compression: "zstd"))
        //     {
        //         Console.WriteLine(ProtoDecodeString(chunk, 1));
        //     }
        // }
        // catch (Grpc.Core.RpcException ex)
        // {
        //     Console.Error.WriteLine($"ServerStreaming failed: {ex.Status.StatusCode} - {ex.Status.Detail}");
        // }
    }

    // protobuf varint 编码
    static int WriteVarint(byte[] buf, int pos, uint value)
    {
        while (value > 0x7F)
        {
            buf[pos++] = (byte)((value & 0x7F) | 0x80);
            value >>= 7;
        }
        buf[pos++] = (byte)value;
        return pos;
    }

    // protobuf varint 解码，返回读取的字节数
    static uint ReadVarint(byte[] data, int pos, out int bytesRead)
    {
        uint result = 0;
        int shift = 0;
        bytesRead = 0;
        while (pos < data.Length)
        {
            byte b = data[pos++];
            bytesRead++;
            result |= (uint)(b & 0x7F) << shift;
            if ((b & 0x80) == 0) break;
            shift += 7;
        }
        return result;
    }

    // protobuf 字符串字段编码（支持任意长度）
    static byte[] ProtoEncodeString(int fieldNumber, string value)
    {
        var data = System.Text.Encoding.UTF8.GetBytes(value);
        // tag + length 各最多 5 字节 varint
        var tmp = new byte[10 + data.Length];
        int pos = WriteVarint(tmp, 0, (uint)((fieldNumber << 3) | 2)); // tag
        pos = WriteVarint(tmp, pos, (uint)data.Length);                 // length varint
        data.CopyTo(tmp, pos);
        return tmp[..(pos + data.Length)];
    }

    static string ProtoDecodeString(byte[] data, int fieldNumber)
    {
        for (int i = 0; i < data.Length;)
        {
            uint tag = ReadVarint(data, i, out int tagBytes);
            i += tagBytes;
            if ((tag & 0x7) == 2)   // wire type 2 = length-delimited
            {
                uint len = ReadVarint(data, i, out int lenBytes);
                i += lenBytes;
                if ((tag >> 3) == fieldNumber)
                    return System.Text.Encoding.UTF8.GetString(data, i, (int)len);
                i += (int)len;
            }
        }
        return string.Empty;
    }
}
