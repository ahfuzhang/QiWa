using System.Buffers;
using System.Threading.Tasks;
using Grpc.Core;

namespace GrpcGeneicServer;

/// <summary>
/// 原始路由处理器接口，定义服务方法处理契约。
/// </summary>
internal interface IRawRouteHandler {
    /// <summary>
    /// 处理解包后的 payload 并返回响应 payload。
    /// </summary>
    /// <param name="payload">解包后的原始请求体。</param>
    /// <param name="context">gRPC 调用上下文。</param>
    /// <returns>响应原始字节序列。</returns>
    ValueTask<ReadOnlySequence<byte>> HandleAsync(ReadOnlySequence<byte> payload, ServerCallContext context);
}
