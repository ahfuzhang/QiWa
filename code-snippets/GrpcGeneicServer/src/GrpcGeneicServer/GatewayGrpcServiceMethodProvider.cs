using System;
using Grpc.AspNetCore.Server.Model;
using Grpc.Core;

namespace GrpcGeneicServer;

/// <summary>
/// GatewayGrpcService 的方法提供器。
/// 根据提示词意图在 BindService 上层显式注册路由，避免 null service 触发框架反射回退。
/// </summary>
internal sealed class GatewayGrpcServiceMethodProvider : IServiceMethodProvider<GatewayGrpcService> {
    /// <summary>
    /// 统一的 Unary 方法描述，定义传输层入口路径与编解码器。
    /// </summary>
    private static readonly Method<RawBytesPayload, RawBytesPayload> RawInvokeMethod =
        new(MethodType.Unary, "greet.Greeter", "SayHello", RawPayloadMarshaller.Instance, RawPayloadMarshaller.Instance);

    /// <summary>
    /// 在应用初始化阶段向 gRPC 注册 GatewayGrpcService 的入口方法。
    /// </summary>
    /// <param name="context">方法发现上下文。</param>
    public void OnServiceMethodDiscovery(ServiceMethodProviderContext<GatewayGrpcService> context) {
        context.AddUnaryMethod(
            RawInvokeMethod,
            Array.Empty<object>(),
            static (service, request, serverCallContext) => service.DispatchAsync(request, serverCallContext));
    }
}
