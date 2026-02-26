using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Logging;

namespace GrpcEchoServer;

/// <summary>
/// gRPC 服务器运行器，负责创建并运行基于 Kestrel 的 HTTP/2 服务。
/// </summary>
internal sealed class GrpcServer {
    /// <summary>
    /// 请求处理器，用于处理每个 HTTP/2 请求。
    /// </summary>
    private readonly GrpcEchoRequestHandler _requestHandler;

    /// <summary>
    /// 初始化 gRPC 服务器运行器。
    /// </summary>
    /// <param name="requestHandler">请求处理器。</param>
    public GrpcServer(GrpcEchoRequestHandler requestHandler) {
        _requestHandler = requestHandler;
    }

    /// <summary>
    /// 启动 HTTP/2 服务并等待关闭信号。
    /// </summary>
    /// <param name="port">监听端口。</param>
    public async Task RunAsync(int port) {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions {
            Args = Array.Empty<string>()
        });

        builder.Logging.ClearProviders();
        builder.Logging.SetMinimumLevel(LogLevel.Warning);
        builder.WebHost.ConfigureKestrel(options => {
            options.ListenAnyIP(port, listenOptions => {
                listenOptions.Protocols = HttpProtocols.Http2;
            });
        });

        var app = builder.Build();
        app.Run(_requestHandler.HandleAsync);

        using var cts = new CancellationTokenSource();
        using var sigterm = PosixSignalRegistration.Create(PosixSignal.SIGTERM, context => {
            context.Cancel = true;
            cts.Cancel();
        });
        Console.CancelKeyPress += (_, eventArgs) => {
            eventArgs.Cancel = true;
            cts.Cancel();
        };

        await app.StartAsync(cts.Token);
        try {
            await Task.Delay(Timeout.Infinite, cts.Token);
        }
        catch (TaskCanceledException) {
        }

        await app.StopAsync();
    }
}
