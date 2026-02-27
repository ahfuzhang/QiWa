using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;

namespace GrpcGeneicServer;

/// <summary>
/// 全局请求路由器，按提示词要求从请求体提取 service/method 并执行自定义分发。
/// </summary>
internal sealed class GlobalRequestRouter {
    /// <summary>
    /// 请求体解包器，用于读取 service/method 与 payload。
    /// </summary>
    private readonly RequestEnvelopeDecoder _decoder;

    /// <summary>
    /// 响应编码器，用于统一响应体编码策略。
    /// </summary>
    private readonly RawResponseEncoder _responseEncoder;

    /// <summary>
    /// 路由表，键为 service/method，值为对应处理器。
    /// </summary>
    private readonly Dictionary<string, IRawRouteHandler> _routes;

    /// <summary>
    /// 初始化全局路由器并注册默认 echo 路由。
    /// </summary>
    /// <param name="decoder">请求体解包器。</param>
    /// <param name="responseEncoder">响应编码器。</param>
    /// <param name="echoRouteHandler">echo 路由处理器。</param>
    public GlobalRequestRouter(RequestEnvelopeDecoder decoder, RawResponseEncoder responseEncoder, EchoRouteHandler echoRouteHandler) {
        _decoder = decoder;
        _responseEncoder = responseEncoder;
        _routes = new Dictionary<string, IRawRouteHandler>(StringComparer.Ordinal) {
            [RouteKeyFormatter.Build("echo", "raw")] = echoRouteHandler,
            // 根据提示词意图补充对 Greeter 客户端的兼容：当请求体不是自定义 envelope 时，使用 gRPC 路径做路由。
            [RouteKeyFormatter.Build("greet.Greeter", "SayHello")] = echoRouteHandler
        };
    }

    /// <summary>
    /// 处理原始请求：解包、路由、业务处理与响应编码。
    /// </summary>
    /// <param name="request">原始请求体。</param>
    /// <param name="context">gRPC 调用上下文。</param>
    /// <returns>编码后的原始响应体。</returns>
    public async ValueTask<ReadOnlySequence<byte>> HandleAsync(ReadOnlySequence<byte> request, ServerCallContext context) {
        string routeKey;
        ReadOnlySequence<byte> payload;

        if (_decoder.TryDecode(request, out RequestEnvelope envelope, out _)) {
            routeKey = RouteKeyFormatter.Build(envelope.ServiceName, envelope.MethodName);
            payload = envelope.Payload;
        }
        else if (RouteKeyFormatter.TryParseGrpcPath(context.Method, out string grpcRouteKey)) {
            routeKey = grpcRouteKey;
            payload = request;
        }
        else {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request envelope and gRPC path."));
        }

        if (!_routes.TryGetValue(routeKey, out IRawRouteHandler? handler)) {
            throw new RpcException(new Status(StatusCode.Unimplemented, $"No route for {routeKey}."));
        }

        ReadOnlySequence<byte> responsePayload = await handler.HandleAsync(payload, context).ConfigureAwait(false);
        return _responseEncoder.Encode(responsePayload);
    }
}
