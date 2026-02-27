using System.Buffers;

namespace GrpcGeneicServer;

/// <summary>
/// 原始字节消息包装类型，用于满足 gRPC 泛型参数的引用类型约束。
/// </summary>
internal sealed class RawBytesPayload {
    /// <summary>
    /// 原始请求或响应字节序列。
    /// </summary>
    public ReadOnlySequence<byte> Bytes { get; }

    /// <summary>
    /// 初始化原始字节包装对象。
    /// </summary>
    /// <param name="bytes">原始请求或响应字节序列。</param>
    public RawBytesPayload(ReadOnlySequence<byte> bytes) {
        Bytes = bytes;
    }
}
