using System.Runtime.InteropServices;
using CmdlineArgs;
using DemoLoginServer.Config;
using DemoLoginServer.Redis;

namespace DemoLoginServer;

/// <summary>
/// 程序入口，负责调用命令行配置、服务初始化、Kestrel 配置和 graceful shutdown。
/// 提示词意图：实现基于 Kestrel 的登录服务器，支持 http1/http2/grpc 多端口，mysql + redis 登录和 session 管理。
/// </summary>
internal static class Program
{
    /// <summary>
    /// 经典 Main 入口，只负责触发命令行解析并启动服务。
    /// </summary>
    public static async Task<int> Main(string[] args)
    {
        // 提示词意图：在服务启动最早期注册全局异常捕获，确保任何未处理异常都能被结构化日志记录并退出进程。
        GlobalExceptionHandler.GlobalExceptionHandler.Configure();
        return await ServerCommandLineOptions.InvokeAsync(args, RunServerAsync);
    }

    /// <summary>
    /// 根据命令行配置启动登录服务并负责关闭前清理。
    /// </summary>
    private static async Task RunServerAsync(ServerCommandLineOptions options)
    {
        // 1. 设置线程池最大线程数
        if (options.Cores.HasValue)
        {
            ThreadPool.SetMinThreads(options.Cores.Value, options.Cores.Value);
            ThreadPool.SetMaxThreads(options.Cores.Value, options.Cores.Value);
        }

        // 2. 初始化日志库（来自 src/ConsoleLogger/Logger.cs）
        var logLevelEnum = Logger.ParseLogLevel(options.LogLevel);
        var (logBufferLong, bufErr) = Common.Utils.ParseBufferSize(options.LogBufferSize);
        if (bufErr.Err())
        {
            Console.Error.WriteLine(bufErr.Message);
            Environment.Exit(1);
        }
        var logBufferBytes = (int)logBufferLong;
        var (tags, tagsErr) = Logger.ParseTags(options.LogGlobalTags);
        if (tagsErr.Err())
        {
            Console.Error.WriteLine(tagsErr.Message);
            Environment.Exit(1);
        }
        Logger.Init(
            level: logLevelEnum,
            flushIntervalMs: options.LogFlushIntervalMs,
            tags: tags,
            logBufferSize: logBufferBytes,
            jsonlineUrl: options.LogPushAddr ?? "");

        // 3. 加载配置文件
        var configErr = AppConfig.Load("config.yaml");
        if (configErr.Err())
        {
            Console.Error.WriteLine(configErr.Message);
            Environment.Exit(1);
        }

        // 4. 初始化 Redis
        RedisManager.Initialize();

        // 5. 构建 Kestrel Web 应用（端口监听、OpenTelemetry Metrics、路由注册）
        var app = KestrelInit.Build(options.Http1Port, options.Http2Port, options.GrpcPort);

        // 8. Graceful Shutdown
        var cts = new CancellationTokenSource();
        // 注册 SIGTERM 信号处理（k8s 会发送 SIGTERM 关闭 pod）
        using var sigterm = PosixSignalRegistration.Create(PosixSignal.SIGTERM, ctx =>
        {
            ctx.Cancel = true;
            cts.Cancel();
        });
        // Ctrl+C
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        await app.StartAsync(cts.Token);  // 开始监听端口

        var log = Logger.Get();
        log.Info(
            Field.String("event"u8, "server_started"),
            Field.Int64("http1_port"u8, options.Http1Port));
        Logger.Return(log);

        try
        {
            await Task.Delay(-1, cts.Token);  // Main() 函数会阻塞在这里
        }
        catch (TaskCanceledException) { }

        await app.StopAsync();

        // 关闭前清理资源
        RedisManager.Shutdown();
        Logger.Shutdown();
    }

}
