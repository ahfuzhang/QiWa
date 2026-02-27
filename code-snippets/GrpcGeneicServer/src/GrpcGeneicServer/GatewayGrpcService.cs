using System.Buffers;
using System.Threading.Tasks;
using Grpc.Core;

namespace GrpcGeneicServer;

/// <summary>
/// 单一 gRPC 入口服务，把所有原始字节请求交给全局路由器处理。
/// </summary>
internal sealed class GatewayGrpcService {
    /// <summary>
    /// 全局路由器，用于根据请求体中的 service/method 分发处理器。
    /// </summary>
    private readonly GlobalRequestRouter _router;

    /// <summary>
    /// 初始化入口服务。
    /// </summary>
    /// <param name="router">全局路由器。</param>
    public GatewayGrpcService(GlobalRequestRouter router) {
        _router = router;
    }

    /// <summary>
    /// 执行统一分发逻辑。
    /// 根据提示词意图，该方法由上层 MethodProvider 显式绑定，方法名与路径字符串无必然关系。
    /// </summary>
    /// <param name="request">原始字节请求包装对象。</param>
    /// <param name="context">调用上下文。</param>
    /// <returns>原始字节响应包装对象。</returns>
    public async Task<RawBytesPayload> DispatchAsync(RawBytesPayload request, ServerCallContext context) {
        ReadOnlySequence<byte> responseBytes = await _router.HandleAsync(request.Bytes, context).ConfigureAwait(false);
        return new RawBytesPayload(responseBytes);
    }
}
