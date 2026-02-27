using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.AspNetCore.Server.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;

namespace GrpcGeneicServer;

/// <summary>
/// gRPC 服务宿主，负责构建容器并启动 HTTP/2 监听。
/// </summary>
internal sealed class GrpcServerHost {
    /// <summary>
    /// 运行 gRPC 服务并阻塞到进程结束。
    /// </summary>
    /// <param name="options">命令行配置。</param>
    /// <param name="cancellationToken">取消信号。</param>
    /// <returns>进程退出码。</returns>
    public async Task<int> RunAsync(CliOptions options, CancellationToken cancellationToken = default) {
        var builder = WebApplication.CreateBuilder();

        builder.WebHost.ConfigureKestrel(kestrel => {
            kestrel.ListenAnyIP(options.Http2Port, endpoint => {
                endpoint.Protocols = HttpProtocols.Http2;
            });
        });

        builder.Services.AddGrpc();
        builder.Services.AddSingleton<RequestEnvelopeDecoder>();
        builder.Services.AddSingleton<RawResponseEncoder>();
        builder.Services.AddSingleton<EchoRouteHandler>();
        builder.Services.AddSingleton<GlobalRequestRouter>();
        // 根据提示词意图在更上层注册方法发现器，避免 BindService 收到 null 并走反射匹配。
        builder.Services.AddSingleton<IServiceMethodProvider<GatewayGrpcService>, GatewayGrpcServiceMethodProvider>();
        //builder.Services.AddGrpcReflection();  // 输出反射信息。生产环境避免使用
        var app = builder.Build();
        app.MapGrpcService<GatewayGrpcService>();
        app.MapGet("/", () => "gRPC generic raw-bytes server. Use a gRPC client.");

        Console.WriteLine($"Listening on 0.0.0.0:{options.Http2Port} with HTTP/2.");
        Console.WriteLine("Request wire format: [2-byte service length][service utf8][2-byte method length][method utf8][payload].");

        await app.RunAsync(cancellationToken).ConfigureAwait(false);
        return 0;
    }
}
