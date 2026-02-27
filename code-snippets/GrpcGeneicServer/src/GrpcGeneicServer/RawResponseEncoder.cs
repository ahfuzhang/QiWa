using System.Buffers;

namespace GrpcGeneicServer;

/// <summary>
/// 响应编码器，按提示词意图控制响应体 encode 过程并返回原始字节。
/// </summary>
internal sealed class RawResponseEncoder {
    /// <summary>
    /// 将业务响应 payload 编码为最终 gRPC 响应体。
    /// </summary>
    /// <param name="payload">业务处理后的原始响应体。</param>
    /// <returns>可直接写入 gRPC 的原始响应字节。</returns>
    public ReadOnlySequence<byte> Encode(ReadOnlySequence<byte> payload) {
        return payload;
    }
}
