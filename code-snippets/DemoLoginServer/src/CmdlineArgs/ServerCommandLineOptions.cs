namespace CmdlineArgs;

using System.CommandLine;

/// <summary>
/// 登录服务的命令行配置。
/// 提示词意图：把命令行参数定义为一个 struct 的成员，并由当前结构体统一负责参数解析。
/// </summary>
internal readonly struct ServerCommandLineOptions
{
    /// <summary>控制 ConsoleLogger 最低输出级别。</summary>
    public string LogLevel { get; }

    /// <summary>控制日志缓冲区 flush 的时间间隔，单位毫秒。</summary>
    public int LogFlushIntervalMs { get; }

    /// <summary>控制日志缓冲区大小，支持带单位后缀的字符串。</summary>
    public string LogBufferSize { get; }

    /// <summary>指定日志通过 HTTP POST 推送到的目标地址。</summary>
    public string? LogPushAddr { get; }

    /// <summary>指定日志上报时附带的全局 tags。</summary>
    public string? LogGlobalTags { get; }

    /// <summary>指定 HTTP/1.1 服务监听端口。</summary>
    public int Http1Port { get; }

    /// <summary>指定 HTTP/2 服务监听端口。</summary>
    public int? Http2Port { get; }

    /// <summary>指定 gRPC 服务监听端口。</summary>
    public int? GrpcPort { get; }

    /// <summary>指定线程池最大线程数。</summary>
    public int? Cores { get; }

    /// <summary>
    /// 构造命令行解析后的服务配置对象。
    /// </summary>
    private ServerCommandLineOptions(
        string logLevel,
        int logFlushIntervalMs,
        string logBufferSize,
        string? logPushAddr,
        string? logGlobalTags,
        int http1Port,
        int? http2Port,
        int? grpcPort,
        int? cores)
    {
        LogLevel = logLevel;
        LogFlushIntervalMs = logFlushIntervalMs;
        LogBufferSize = logBufferSize;
        LogPushAddr = logPushAddr;
        LogGlobalTags = logGlobalTags;
        Http1Port = http1Port;
        Http2Port = http2Port;
        GrpcPort = grpcPort;
        Cores = cores;
    }

    /// <summary>
    /// 创建根命令并在解析完成后把参数绑定到当前结构体。
    /// </summary>
    public static Task<int> InvokeAsync(string[] args, Func<ServerCommandLineOptions, Task> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        return CreateRootCommand(handler).InvokeAsync(args);
    }

    /// <summary>
    /// 生成 DemoLoginServer 使用的根命令和所有选项定义。
    /// </summary>
    private static RootCommand CreateRootCommand(Func<ServerCommandLineOptions, Task> handler)
    {
        var logLevelOption = new Option<string>("-log.level", () => "warn",
            "日志级别 (error/warn/info/debug)");
        var logFlushIntervalOption = new Option<int>("-log.flush.interval.ms", () => 1000,
            "日志 flush 时间间隔（毫秒）");
        var logBufferSizeOption = new Option<string>("-log.buffer.size", () => "64k",
            "日志 buffer 大小，支持 k/kb/m/mb/g/gb 后缀，最大 1G");
        var logPushAddrOption = new Option<string?>("-log.push.addr", () => null,
            "日志 POST 的 http 地址");
        var logGlobalTagsOption = new Option<string?>("-log.global.tags", () => null,
            "日志全局 tags，格式：a=b&c=d");
        var http1PortOption = new Option<int>("-http1.port", "HTTP/1.1 监听端口（必须设置）");
        var http2PortOption = new Option<int?>("-http2.port", () => null, "HTTP/2 监听端口（可选）");
        var grpcPortOption = new Option<int?>("-grpc.port", () => null, "gRPC 监听端口（可选）");
        var coresOption = new Option<int?>("-cores", () => null, "线程池最大线程数");
        http1PortOption.IsRequired = true;

        var root = new RootCommand("DemoLoginServer - 基于 Kestrel 的登录服务器");
        root.AddOption(logLevelOption);
        root.AddOption(logFlushIntervalOption);
        root.AddOption(logBufferSizeOption);
        root.AddOption(logPushAddrOption);
        root.AddOption(logGlobalTagsOption);
        root.AddOption(http1PortOption);
        root.AddOption(http2PortOption);
        root.AddOption(grpcPortOption);
        root.AddOption(coresOption);

        root.SetHandler(async context =>
        {
            var options = new ServerCommandLineOptions(
                logLevel: context.ParseResult.GetValueForOption(logLevelOption)!,
                logFlushIntervalMs: context.ParseResult.GetValueForOption(logFlushIntervalOption),
                logBufferSize: context.ParseResult.GetValueForOption(logBufferSizeOption)!,
                logPushAddr: context.ParseResult.GetValueForOption(logPushAddrOption),
                logGlobalTags: context.ParseResult.GetValueForOption(logGlobalTagsOption),
                http1Port: context.ParseResult.GetValueForOption(http1PortOption),
                http2Port: context.ParseResult.GetValueForOption(http2PortOption),
                grpcPort: context.ParseResult.GetValueForOption(grpcPortOption),
                cores: context.ParseResult.GetValueForOption(coresOption));
            await handler(options);
        });
        return root;
    }
}
