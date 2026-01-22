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

using OpenTelemetry.Metrics;

namespace Http1EchoServer;

internal static class Program
{
    private static readonly Meter AppMeter = new("Http1EchoServer");
    private static readonly long ServiceStartTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    public static async Task<int> Main(string[] args)
    {
        // Define metric
        AppMeter.CreateObservableGauge("up", () => ServiceStartTime, description: "Service start time");
        
        var portOption = new Option<int>("-http1.port", "HTTP/1.1 listen port.");
        portOption.AddAlias("--http1.port"); // Support double dash as well
        portOption.IsRequired = true;

        var pushIntervalOption = new Option<int>("-metrics.push.interval.seconds", () => 15, "Metrics push interval (seconds).");
        var pushAddrOption = new Option<string?>("-metrics.push.addr", "Metrics push address.");
        var extraLabelsOption = new Option<string?>("-metrics.push.extra.labels", "Extra labels (a=b&c=d).");
        var maxThreadsOption = new Option<int?>("-threadpool.max", "ThreadPool max threads.");

        var root = new RootCommand("Http1EchoServer");
        root.AddOption(portOption);
        root.AddOption(pushIntervalOption);
        root.AddOption(pushAddrOption);
        root.AddOption(extraLabelsOption);
        root.AddOption(maxThreadsOption);

        root.SetHandler(async (context) =>
        {
            var port = context.ParseResult.GetValueForOption(portOption);
            var pushInterval = context.ParseResult.GetValueForOption(pushIntervalOption);
            var pushAddr = context.ParseResult.GetValueForOption(pushAddrOption);
            var extraLabels = context.ParseResult.GetValueForOption(extraLabelsOption);
            var maxThreads = context.ParseResult.GetValueForOption(maxThreadsOption);

            await RunServerAsync(port, pushInterval, pushAddr, extraLabels, maxThreads);
        });

        return await root.InvokeAsync(args);
    }

    private static async Task RunServerAsync(int port, int pushInterval, string? pushAddr, string? extraLabels, int? maxThreads)
    {
        // 1. ThreadPool
        if (maxThreads.HasValue)
        {
            ThreadPool.SetMinThreads(maxThreads.Value, maxThreads.Value);
            ThreadPool.SetMaxThreads(maxThreads.Value, maxThreads.Value);
        }

        // 2. Check Port
        if (!IsPortAvailable(port))
        {
            Console.Error.WriteLine($"Port {port} is unavailable.");
            Environment.Exit(1);
        }

        // 3. Build WebApp
        var builder = WebApplication.CreateBuilder();
        
        // Configure Logging
        builder.Services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.SetMinimumLevel(LogLevel.Warning);
            logging.AddJsonConsole(options =>
            {
                options.IncludeScopes = false;
                options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffffffZ";
                options.JsonWriterOptions = new JsonWriterOptions
                {
                    Indented = false
                };
            });
        });

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(port, listenOptions => listenOptions.Protocols = HttpProtocols.Http1);
        });

        // 4. Configure OpenTelemetry (Base)
        // Ensure Prometheus exporter and standard instrumentation are always present
        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
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
        if (!string.IsNullOrWhiteSpace(pushAddr))
        {
            var labels = ParseLabels(extraLabels);
            // MetricsPusher will attach its own Reader and additional configuration
            pusher = new MetricsPusher(pushInterval, pushAddr, labels, builder);
        }

        var app = builder.Build();
        var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Http1EchoServer");

        app.MapPrometheusScrapingEndpoint();
        
        // 5. Echo + Logging
        app.Map("/echo", async context =>
        {
           var req = context.Request;
           var sb = new StringBuilder();
           sb.Append($"{req.Method} {req.Path}{req.QueryString} {req.Protocol}\r\n");
           foreach (var h in req.Headers)
           {
               sb.Append($"{h.Key}: {h.Value}\r\n");
           }
           sb.Append("\r\n");
           
           context.Response.ContentType = "text/plain";
           await context.Response.WriteAsync(sb.ToString());
           //LogRequest(context, 200, logger);
        });

        // 6. Graceful Shutdown
        var cts = new CancellationTokenSource();
        // Hook SIGTERM
        using var reg = PosixSignalRegistration.Create(PosixSignal.SIGTERM, ctx =>
        {
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

    private static void LogRequest(HttpContext ctx, int statusCode, ILogger logger)
    {
        // try 
        // {
            // AOT-safe structured logging
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
       // }
        // catch (Exception ex)
        // {
        //     logger.LogError(ex, "Error while logging request");
        //     // print detailed info to console as requested
        //     Console.WriteLine($"[Error] LogRequest failed: {ex}");
        // }
    }

    private static bool IsPortAvailable(int port)
    {
        try
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static Dictionary<string, string> ParseLabels(string? extraLabels)
    {
        var dict = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(extraLabels)) return dict;
        foreach (var pair in extraLabels.Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
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
internal partial class LogRecordContext : System.Text.Json.Serialization.JsonSerializerContext
{
}
