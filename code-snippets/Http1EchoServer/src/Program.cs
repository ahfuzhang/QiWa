using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MetricsPush;
using System.Runtime.CompilerServices;

using OpenTelemetry.Metrics;
using System.Security.AccessControl;

namespace Http1EchoServer;

internal static class Program {
    private static readonly Meter AppMeter = new("Http1EchoServer");
    private static readonly Counter<long> HttpRequestTotal = AppMeter.CreateCounter<long>(
        "http_request_total",
        "1",
        "Total HTTP requests.");
    private static readonly long ServiceStartTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    private static readonly System.IO.Stream Stdout = Console.OpenStandardOutput();
    private static readonly object locker = new object();

    public static async Task<int> Main(string[] args) {
        // Define metric
        AppMeter.CreateObservableGauge("up", () => ServiceStartTime, description: "Service start time");

        var portOption = new Option<int>("-http1.port", "HTTP/1.1 listen port.");
        portOption.AddAlias("--http1.port"); // Support double dash as well
        portOption.IsRequired = true;

        var pushIntervalOption = new Option<int>("-metrics.push.interval.seconds", () => 15, "Metrics push interval (seconds).");
        var pushAddrOption = new Option<string?>("-metrics.push.addr", "Metrics push address.");
        pushAddrOption.Arity = ArgumentArity.ZeroOrOne;
        var extraLabelsOption = new Option<string?>("-metrics.push.extra.labels", "Extra labels (a=b&c=d).");
        var maxThreadsOption = new Option<int?>("-threadpool.max", "ThreadPool max threads.");
        var outputRequestLogOption = new Option<bool>("-output.request.log", () => false, "Output request logs.");
        var logBufferSizeKbOption = new Option<int>("-log.buffer.size.kb", () => 16, "Log buffer size (KB).");
        var logFlushIntervalMsOption = new Option<int>("-log.flush.interval.ms", () => 1000, "Log flush interval (ms).");

        var root = new RootCommand("Http1EchoServer");
        root.AddOption(portOption);
        root.AddOption(pushIntervalOption);
        root.AddOption(pushAddrOption);
        root.AddOption(extraLabelsOption);
        root.AddOption(maxThreadsOption);
        root.AddOption(outputRequestLogOption);
        root.AddOption(logBufferSizeKbOption);
        root.AddOption(logFlushIntervalMsOption);

        root.SetHandler(async (context) => {
            var port = context.ParseResult.GetValueForOption(portOption);
            var pushInterval = context.ParseResult.GetValueForOption(pushIntervalOption);
            var pushAddr = context.ParseResult.GetValueForOption(pushAddrOption);
            var extraLabels = context.ParseResult.GetValueForOption(extraLabelsOption);
            var maxThreads = context.ParseResult.GetValueForOption(maxThreadsOption);
            var outputRequestLog = context.ParseResult.GetValueForOption(outputRequestLogOption);
            var logBufferSizeKb = context.ParseResult.GetValueForOption(logBufferSizeKbOption);
            var logFlushIntervalMs = context.ParseResult.GetValueForOption(logFlushIntervalMsOption);

            await RunServerAsync(port, pushInterval, pushAddr, extraLabels, maxThreads, outputRequestLog, logBufferSizeKb, logFlushIntervalMs);
        });

        return await root.InvokeAsync(args);
    }

    private static async Task RunServerAsync(
        int port,
        int pushInterval,
        string? pushAddr,
        string? extraLabels,
        int? maxThreads,
        bool outputRequestLog,
        int logBufferSizeKb,
        int logFlushIntervalMs) {
        // 1. ThreadPool
        if (maxThreads.HasValue) {
            ThreadPool.SetMinThreads(maxThreads.Value, maxThreads.Value);
            ThreadPool.SetMaxThreads(maxThreads.Value, maxThreads.Value);
        }

        // 2. Check Port
        if (!IsPortAvailable(port)) {
            Console.Error.WriteLine($"Port {port} is unavailable.");
            Environment.Exit(1);
        }

        // 3. Build WebApp
        var builder = WebApplication.CreateBuilder();

        // Configure Logging
        builder.Services.AddLogging(logging => {
            logging.ClearProviders();
            logging.SetMinimumLevel(LogLevel.Warning);
            logging.AddJsonConsole(options => {
                options.IncludeScopes = false;
                options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffffffZ";
                options.JsonWriterOptions = new JsonWriterOptions {
                    Indented = false
                };
            });
        });
        // 初始化日志对象
        // Log.Logger.Init(
        //     level: Log.LogLevel.Info, 
        //     flushIntervalMs: 1000, 
        //     tags: new Dictionary<string, string>{}, 
        //     overload: Log.OverloadPolicy.Direct, 
        //     queueSize:1, 
        //     logBufferSize: 1024*16
        // );
        var logBufferSizeBytes = logBufferSizeKb * 1024;
        ConsoleLogger.Logger.Init(
            global::ConsoleLogger.LogLevel.Debug, 
            logFlushIntervalMs, 
            new Dictionary<string, string>(){{"namespace","backend-team"}}, 
            logBufferSizeBytes
        );

        builder.WebHost.ConfigureKestrel(options => {
            options.ListenAnyIP(port, listenOptions => listenOptions.Protocols = HttpProtocols.Http1);
        });

        // 4. Configure OpenTelemetry (Base)
        // Ensure Prometheus exporter and standard instrumentation are always present
        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics => {
                metrics.AddPrometheusExporter();

                metrics.AddProcessInstrumentation();
                metrics.AddRuntimeInstrumentation();
                metrics.AddHttpClientInstrumentation();
                metrics.AddAspNetCoreInstrumentation();

                // Add standard meters
                metrics.AddMeter(
                    "System.Runtime",
                    "System.Net.Http",
                    "System.Net.Sockets",
                    "Microsoft.AspNetCore.Hosting",
                    "Microsoft.AspNetCore.Server.Kestrel",
                    "Microsoft.AspNetCore.Http.Connections",
                    "System.Net.NameResolution",
                    "Microsoft.AspNetCore.RateLimiting",
                    "Http1EchoServer"
                );
            });

        // 5. Metrics Pusher
        MetricsPusher? pusher = null;
        if (!string.IsNullOrWhiteSpace(pushAddr)) {
            var labels = ParseLabels(extraLabels);
            // MetricsPusher will attach its own Reader and additional configuration
            pusher = new MetricsPusher(pushInterval, pushAddr, labels, builder);
        }

        var app = builder.Build();
        var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Http1EchoServer");

        app.MapPrometheusScrapingEndpoint();

        // 5. Echo + Logging
        app.Map("/echo", async context => {
            await HandleEchoRequest(context, logger, outputRequestLog);
        });

        // 6. Graceful Shutdown
        var cts = new CancellationTokenSource();
        // Hook SIGTERM
        using var reg = PosixSignalRegistration.Create(PosixSignal.SIGTERM, ctx => {
            ctx.Cancel = true;
            cts.Cancel();
        });
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

        await app.StartAsync(cts.Token);

        // Wait for cancellation
        try { await Task.Delay(-1, cts.Token); } catch (TaskCanceledException) { }

        await app.StopAsync();
        pusher?.Dispose();
    }

    private static async Task HandleEchoRequest(HttpContext context, ILogger logger, bool outputRequestLog) {
        HttpRequestTotal.Add(1);
        var req = context.Request;
        using var rent = new Common.RentedBuffer(1024 * 2);
        rent.Append(req.Method);
        rent.Append((byte)' ');
        rent.Append(req.Host.ToString());
        rent.Append(req.Path);
        rent.Append(req.QueryString.ToString());
        rent.Append((byte)' ');
        rent.Append(req.Protocol);
        rent.Append("\r\n"u8);
        //var sb = new StringBuilder();
        //sb.Append($"{req.Method} {req.Path}{req.QueryString} {req.Protocol}\r\n");
        foreach (var h in req.Headers) {
            rent.Append(h.Key);
            rent.Append(": "u8);
            rent.Append(h.Value.ToString());
            rent.Append("\r\n"u8);
            //sb.Append($"{h.Key}: {h.Value}\r\n");
        }
        //sb.Append("\r\n");
        rent.Append("\r\n"u8);
        context.Response.ContentType = "text/plain";
        await context.Response.BodyWriter.WriteAsync(rent.Data!.AsMemory(0, rent.Length), context.RequestAborted);
        if (outputRequestLog) {
            LogRequestV7(context, 200);
        }
    }

    // private static void LogRequestV5(HttpContext ctx, int statusCode) {
    //     var req = ctx.Request;
    //     var logger = new Log.TaskLogger();
    //     logger.Info(
    //         Log.Field.String("method"u8, req.Method),
    //         Log.Field.String("host"u8, req.Host.ToString()),
    //         Log.Field.String("path"u8, req.Path.ToString()),
    //         Log.Field.String("querystring"u8, req.QueryString.ToString()),
    //         Log.Field.Int64("status_code"u8, statusCode)
    //     );
    // }
    private static void LogRequestV6(HttpContext ctx, int statusCode) {
        var req = ctx.Request;
        var logger = ConsoleLogger.Logger.Get();
        try{
            logger.Info(
                ConsoleLogger.Field.String("method"u8, req.Method),
                ConsoleLogger.Field.String("host"u8, req.Host.ToString()),
                ConsoleLogger.Field.String("path"u8, req.Path.ToString()),
                ConsoleLogger.Field.String("querystring"u8, req.QueryString.ToString()),
                ConsoleLogger.Field.Int64("status_code"u8, statusCode)
            );
        }finally{
            ConsoleLogger.Logger.Return(logger);
        }
    }

    private static void LogRequestV7(HttpContext ctx, int statusCode, 
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
        var req = ctx.Request;
        using var rent = new Common.RentedBuffer(1024);
        rent.Append("{\"_time\":\""u8);
        rent.AppendUtcDatetime(DateTime.UtcNow);
        rent.Append("\",\"method\":\""u8);
        rent.Append(req.Method);
        rent.Append("\",\"host\":\""u8);
        rent.Append(req.Host.ToString());
        rent.Append("\",\"path\":\""u8);
        rent.Append(req.Path.ToString());
        rent.Append("\",\"querystring\":\""u8);
        rent.Append(req.QueryString.ToString());
        rent.Append("\",\"status_code\":"u8);
        rent.Append(statusCode);
        //
        rent.Append("\",\"namespace\":\""u8);
        rent.Append("backend-team");
        rent.Append("\",\"level\":\""u8);rent.Append("info");
        rent.Append("\",\"_file\":\""u8);rent.Append(file);
        rent.Append("\",\"_member\":\""u8);rent.Append(member);
        rent.Append("\",\"_line\":\""u8);rent.Append(line);
        rent.Append("}\n");
        lock(locker){
            Stdout.Write(rent.Bytes());
        }
    }

    private static void LogRequestV0(HttpContext ctx, int statusCode) {
        var req = ctx.Request;
        using var rent = new Common.RentedBuffer(1024);
        rent.Append("{\"_time\":\""u8);
        rent.AppendUtcDatetime(DateTime.UtcNow);
        rent.Append("\",\"method\":\""u8);
        rent.Append(req.Method);
        rent.Append("\",\"host\":\""u8);
        rent.Append(req.Host.ToString());
        rent.Append("\",\"path\":\""u8);
        rent.Append(req.Path.ToString());
        rent.Append("\",\"querystring\":\""u8);
        rent.Append(req.QueryString.ToString());
        rent.Append("\",\"status_code\":"u8);
        rent.Append(statusCode);
        rent.Append("}\n");
        Stdout.Write(rent.Bytes());
    }

    private static async Task LogRequest2(HttpContext ctx, int statusCode, ILogger logger) {
        var req = ctx.Request;
        using var rent = new Common.RentedBuffer(1024);
        rent.Append("{\"_time\":\""u8);
        rent.AppendUtcDatetime(DateTime.UtcNow);
        rent.Append("\",\"method\":\""u8);
        rent.Append(req.Method);
        rent.Append("\",\"host\":\""u8);
        rent.Append(req.Host.ToString());
        rent.Append("\",\"path\":\""u8);
        rent.Append(req.Path.ToString());
        rent.Append("\",\"querystring\":\""u8);
        rent.Append(req.QueryString.ToString());
        rent.Append("\",\"status_code\":"u8);
        rent.Append(statusCode);
        rent.Append("}\n");
        await Stdout.WriteAsync(rent.Data.AsMemory(0, rent.Length), ctx.RequestAborted);
    }

    private static void LogRequest1(HttpContext ctx, int statusCode, ILogger logger) {
        var log = new LogRecord
        (
            DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"),
            ctx.Request.Method,
            ctx.Request.Host.ToString(),
            ctx.Request.Path.ToString(),
            ctx.Request.QueryString.ToString(),
            statusCode
        );
        Console.WriteLine(JsonSerializer.Serialize(log, LogRecordContext.Default.LogRecord));
    }

    private static bool IsPortAvailable(int port) {
        try {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            return true;
        }
        catch {
            return false;
        }
    }

    private static Dictionary<string, string> ParseLabels(string? extraLabels) {
        var dict = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(extraLabels)) return dict;
        foreach (var pair in extraLabels.Split('&', StringSplitOptions.RemoveEmptyEntries)) {
            var parts = pair.Split('=', 2);
            if (parts.Length == 2) dict[parts[0]] = parts[1];
        }
        return dict;
    }
}

// AOT-safe Log Record
internal record struct LogRecord(
    [property: System.Text.Json.Serialization.JsonPropertyName("_time")] string Time,
    [property: System.Text.Json.Serialization.JsonPropertyName("method")] string Method,
    [property: System.Text.Json.Serialization.JsonPropertyName("host")] string Host,
    [property: System.Text.Json.Serialization.JsonPropertyName("path")] string Path,
    [property: System.Text.Json.Serialization.JsonPropertyName("querystring")] string QueryString,
    [property: System.Text.Json.Serialization.JsonPropertyName("status_code")] int StatusCode
);

[System.Text.Json.Serialization.JsonSerializable(typeof(LogRecord))]
internal partial class LogRecordContext : System.Text.Json.Serialization.JsonSerializerContext {
}
