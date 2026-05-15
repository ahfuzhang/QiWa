using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using DemoLoginServer.Handlers;

namespace DemoLoginServer;

/// <summary>
/// 负责构建并配置 Kestrel WebApplication，包括端口监听、OpenTelemetry Metrics 和路由注册。
/// </summary>
internal static class KestrelInit
{
    /// <summary>
    /// 构建并配置 WebApplication。
    /// </summary>
    /// <param name="http1Port">HTTP/1.1 监听端口（必须）。</param>
    /// <param name="http2Port">HTTP/2 监听端口（可选）。</param>
    /// <param name="grpcPort">gRPC 监听端口（可选，基于 HTTP/2）。</param>
    /// <returns>已注册路由、尚未启动的 WebApplication 实例。</returns>
    public static WebApplication Build(int http1Port, int? http2Port, int? grpcPort)
    {
        var builder = WebApplication.CreateBuilder();

        // 关闭 ASP.NET Core 内置日志，避免干扰自定义日志库；启用 JSON 格式控制台输出
        builder.Services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Warning);
            logging.AddJsonConsole(options =>
            {
                options.JsonWriterOptions = new System.Text.Json.JsonWriterOptions
                {
                    Indented = false,
                };
            });
        });

        // 配置监听端口
        builder.WebHost.ConfigureKestrel(kestrelOptions =>
        {
            // HTTP/1.1 端口（必须）
            kestrelOptions.ListenAnyIP(
                http1Port,
                listenOptions => listenOptions.Protocols = HttpProtocols.Http1);

            // HTTP/2 端口（可选）
            if (http2Port.HasValue)
            {
                kestrelOptions.ListenAnyIP(
                    http2Port.Value,
                    listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
            }

            // gRPC 端口（可选，gRPC 基于 HTTP/2）
            if (grpcPort.HasValue)
            {
                kestrelOptions.ListenAnyIP(
                    grpcPort.Value,
                    listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
            }
        });

        // 配置响应压缩：仅对 text/plain（Prometheus metrics）启用 gzip
        builder.Services.AddResponseCompression(opts =>
        {
            opts.EnableForHttps = false;
            opts.Providers.Add<GzipCompressionProvider>();
            opts.MimeTypes = new[] { "text/plain" };
        });

        // 配置 OpenTelemetry Metrics（含 Kestrel/Runtime 指标上报）
        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddPrometheusExporter();
                metrics.AddProcessInstrumentation();
                metrics.AddRuntimeInstrumentation();
                metrics.AddHttpClientInstrumentation();
                metrics.AddAspNetCoreInstrumentation();
                metrics.AddMeter(
                    "System.Runtime",
                    "System.Net.Http",
                    "System.Net.Sockets",
                    "Microsoft.AspNetCore.Hosting",
                    "Microsoft.AspNetCore.Server.Kestrel",
                    "DemoLoginServer");
            });
        if (grpcPort.HasValue)
        {
            //builder.Services.AddGrpc();
            builder.Services.AddGrpc(options =>
                {
                    // 替换掉真正的 gzip provider，用透传版本
                    options.CompressionProviders.Add(new DemoLoginServer.GrpcUtils.PassthroughCompressionProvider("gzip"));
                    options.CompressionProviders.Add(new DemoLoginServer.GrpcUtils.PassthroughCompressionProvider("zstd"));

                    // 全局默认用 gzip（框架会设 compressed-flag=1 并写 grpc-encoding: gzip）
                    options.ResponseCompressionAlgorithm = "gzip";
                    options.ResponseCompressionLevel = System.IO.Compression.CompressionLevel.NoCompression; // 无意义，但保持一致
                }
            );

        }
        var app = builder.Build();

        // 启用响应压缩中间件（需在路由之前，/metrics 响应将按 Accept-Encoding 自动 gzip 压缩）
        app.UseResponseCompression();

        // 注册路由
        // /metrics、/healthz、/ready 仅在 HTTP/1 端口上生效，避免暴露到 gRPC/HTTP2 端口
        var http1HostFilter = $"*:{http1Port}";
        // /metrics - Prometheus 格式的 metrics 数据
        app.MapPrometheusScrapingEndpoint().RequireHost(http1HostFilter);
        // /healthz - k8s 健康检查
        app.MapGet("/healthz", () => "OK").RequireHost(http1HostFilter);
        // /ready - k8s 就绪检查
        app.MapGet("/ready", () => "OK").RequireHost(http1HostFilter);

        // /login、/biz_logic 仅在 HTTP/1 和 HTTP/2 端口上生效，不暴露到 gRPC 端口
        var bizHostFilters = new List<string> { $"*:{http1Port}" };
        if (http2Port.HasValue)
            bizHostFilters.Add($"*:{http2Port.Value}");
        var bizHostFilterArray = bizHostFilters.ToArray();
        // /login - 用户登录
        app.MapPost("/login", LoginHandler.HandleAsync).RequireHost(bizHostFilterArray);
        // /biz_logic - 业务接口（需要鉴权）
        app.MapPost("/biz_logic", BizHandler.HandleAsync).RequireHost(bizHostFilterArray);
        // 所有 HTTP/1.1 未匹配路由的请求，统一走 Http1Handler 兜底处理
        app.MapFallback(Http1Handler.HandleAsync).RequireHost(http1HostFilter);
        // 所有 HTTP/2 未匹配路由的请求，统一走 Http2Handler 兜底处理
        if (http2Port.HasValue)
        {
            app.MapFallback(Http2Handler.HandleAsync).RequireHost($"*:{http2Port.Value}");
        }
        //
        if (grpcPort.HasValue)
        {
            // todo: 使用全局的拦截器
            //app.MapGrpcService<EchoService>().RequireHost($"*:{grpcPort.Value}");
        }
        return app;
    }
}
