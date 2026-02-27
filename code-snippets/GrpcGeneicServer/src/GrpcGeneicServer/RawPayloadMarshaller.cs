using System.Buffers;
using Grpc.AspNetCore.Server;
using Grpc.Core;

namespace GrpcGeneicServer;

/// <summary>
/// 原始字节编解码器，负责直接读写 gRPC payload，绕开 proto 生成模型。
/// </summary>
internal static class RawPayloadMarshaller {
    /// <summary>
    /// 原始字节 marshaller 实例。
    /// </summary>
    public static readonly Marshaller<RawBytesPayload> Instance = new(Serialize, Deserialize);

    /// <summary>
    /// 将 ReadOnlySequence 原样写入 gRPC payload。
    /// </summary>
    /// <param name="payload">待写入的原始字节包装对象。</param>
    /// <param name="context">序列化上下文。</param>
    private static void Serialize(RawBytesPayload payload, SerializationContext context) {
        var writer = context.GetBufferWriter();
        foreach (ReadOnlyMemory<byte> segment in payload.Bytes) {
            writer.Write(segment.Span);
        }

        context.Complete();
    }

    /// <summary>
    /// 从 gRPC payload 提取原始字节序列。
    /// </summary>
    /// <param name="context">反序列化上下文。</param>
    /// <returns>原始字节包装对象。</returns>
    private static RawBytesPayload Deserialize(DeserializationContext context) {
        // 按本次提示词意图修复 make req 调试链路：复制请求字节，避免读取到生命周期已结束的底层序列。
        byte[] payloadBytes = context.PayloadAsNewBuffer();
        return new RawBytesPayload(new ReadOnlySequence<byte>(payloadBytes));
    }
}
