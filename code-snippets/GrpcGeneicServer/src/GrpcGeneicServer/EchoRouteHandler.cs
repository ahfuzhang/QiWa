using System.Buffers;
using System.Threading.Tasks;
using Grpc.Core;

namespace GrpcGeneicServer;

/// <summary>
/// Echo 处理器，按提示词要求把未 decode 的请求体原样作为响应返回。
/// </summary>
internal sealed class EchoRouteHandler : IRawRouteHandler {
    /// <summary>
    /// 返回输入 payload 本身，实现原样回显。
    /// </summary>
    /// <param name="payload">解包后的原始请求体。</param>
    /// <param name="context">gRPC 调用上下文。</param>
    /// <returns>原样回显的响应体。</returns>
    public ValueTask<ReadOnlySequence<byte>> HandleAsync(ReadOnlySequence<byte> payload, ServerCallContext context) {
        return new ValueTask<ReadOnlySequence<byte>>(payload);
    }
}
