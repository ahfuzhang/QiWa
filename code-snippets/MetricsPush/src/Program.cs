using System.CommandLine;
using System.CommandLine.Invocation;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;

namespace MetricsPush;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var portOption = new Option<int>("-http1.port", () => 8081, "Kestrel HTTP/1.1 listen port.");
        portOption.AddAlias("--http1.port");
        portOption.AddValidator(result =>
        {
            if (result.GetValueOrDefault<int>() <= 0)
            {
                result.ErrorMessage = "http1.port must be greater than 0.";
            }
        });

        var intervalOption = new Option<int>("-push.interval.seconds", () => 10, "Metrics push interval in seconds.");
        intervalOption.AddAlias("--push.interval.seconds");
        intervalOption.AddValidator(result =>
        {
            if (result.GetValueOrDefault<int>() <= 0)
            {
                result.ErrorMessage = "push.interval.seconds must be greater than 0.";
            }
        });

        var addrOption = new Option<string>("-push.addr", "Metrics push target address.");
        addrOption.AddAlias("--push.addr");
        addrOption.IsRequired = true;
        addrOption.AddValidator(result =>
        {
            string? value = result.GetValueOrDefault<string>();
            if (string.IsNullOrWhiteSpace(value) || !Uri.TryCreate(value, UriKind.Absolute, out _))
            {
                result.ErrorMessage = "push.addr must be a valid absolute URI.";
            }
        });

        var extraLabelsOption = new Option<string>("-extraLabels", () => string.Empty, "Extra labels, e.g. a=b&c=d.");
        extraLabelsOption.AddAlias("--extraLabels");

        var root = new RootCommand("OpenTelemetry metrics push CLI.");
        root.AddOption(portOption);
        root.AddOption(intervalOption);
        root.AddOption(addrOption);
        root.AddOption(extraLabelsOption);

        root.SetHandler(async (InvocationContext context) =>
        {
            int httpPort = context.ParseResult.GetValueForOption(portOption);
            int pushIntervalSeconds = context.ParseResult.GetValueForOption(intervalOption);
            string pushAddr = context.ParseResult.GetValueForOption(addrOption) ?? string.Empty;
            string extraLabels = context.ParseResult.GetValueForOption(extraLabelsOption) ?? string.Empty;

            var options = BuildOptions(httpPort, pushIntervalSeconds, pushAddr, extraLabels);
            if (!TryEnsurePortAvailable(options.HttpPort, out string errorMessage))
            {
                Console.Error.WriteLine(errorMessage);
                context.ExitCode = 1;
                return;
            }

            await RunAsync(options);
        });

        return await root.InvokeAsync(args);
    }

    private static MetricsPushOptions BuildOptions(int httpPort, int pushIntervalSeconds, string pushAddr, string extraLabels)
    {
        var uri = new Uri(pushAddr, UriKind.Absolute);
        var labels = ExtraLabelsParser.Parse(extraLabels);
        return new MetricsPushOptions(httpPort, pushIntervalSeconds, uri, labels);
    }

    private static bool TryEnsurePortAvailable(int port, out string errorMessage)
    {
        if (TryBindPort(AddressFamily.InterNetworkV6, IPAddress.IPv6Any, port, dualMode: true, out SocketError? socketError, out bool unsupported))
        {
            errorMessage = string.Empty;
            return true;
        }

        if (unsupported)
        {
            if (TryBindPort(AddressFamily.InterNetwork, IPAddress.Any, port, dualMode: false, out socketError, out _))
            {
                errorMessage = string.Empty;
                return true;
            }
        }

        errorMessage = socketError == SocketError.AddressAlreadyInUse
            ? $"Port {port} is already in use."
            : $"Port {port} is unavailable.";
        return false;
    }

    private static bool TryBindPort(AddressFamily family, IPAddress address, int port, bool dualMode, out SocketError? socketError, out bool unsupported)
    {
        socketError = null;
        unsupported = false;

        try
        {
            using var socket = new Socket(family, SocketType.Stream, ProtocolType.Tcp);
            if (dualMode)
            {
                socket.DualMode = true;
            }

            socket.Bind(new IPEndPoint(address, port));
            return true;
        }
        catch (PlatformNotSupportedException)
        {
            unsupported = true;
            return false;
        }
        catch (NotSupportedException)
        {
            unsupported = true;
            return false;
        }
        catch (SocketException ex) when (
            ex.SocketErrorCode == SocketError.AddressFamilyNotSupported ||
            ex.SocketErrorCode == SocketError.ProtocolNotSupported ||
            ex.SocketErrorCode == SocketError.OperationNotSupported)
        {
            socketError = ex.SocketErrorCode;
            unsupported = true;
            return false;
        }
        catch (SocketException ex)
        {
            socketError = ex.SocketErrorCode;
            return false;
        }
    }

    private static async Task RunAsync(MetricsPushOptions options)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = Array.Empty<string>()
        });

        builder.Logging.ClearProviders();
        builder.Logging.AddJsonConsole();
        builder.Logging.SetMinimumLevel(LogLevel.Warning);

        builder.WebHost.ConfigureKestrel(kestrelOptions =>
        {
            kestrelOptions.ListenAnyIP(options.HttpPort, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http1;
            });
        });

        var inProcessExporter = new InProcessMetricsExporter(options.ExtraLabels);
        builder.Services.AddSingleton(inProcessExporter);
        builder.Services.AddSingleton(options);
        builder.Services.AddHttpClient();
        builder.Services.AddHostedService<MetricsPushService>();

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddMeter(MetricsPushTelemetry.Meter.Name);
                metrics.AddProcessInstrumentation();
                metrics.AddRuntimeInstrumentation();
                metrics.AddMeter(
                    "System.Runtime", 
                    "System.Net.Http", 
                    "System.Net.Sockets", 
                    "Microsoft.AspNetCore.Hosting",
                    "Microsoft.AspNetCore.Server.Kestrel", 
                    "Microsoft.AspNetCore.Http.Connections", 
                    "System.Net.NameResolution",
                    "Microsoft.AspNetCore.Hosting",  //"http.server.request.duration",
                    "Microsoft.AspNetCore.RateLimiting",  //"aspnetcore.rate_limiting.request.time_in_queue"
                    //"Microsoft.AspNetCore.Server.Kestrel"
                    "OpenTelemetry.Instrumentation.AspNet",
                    "OpenTelemetry.Instrumentation.AspNetCore",
                    "OpenTelemetry.Instrumentation.Http",
                    "System.Net.NameResolution",
                    "Microsoft.AspNetCore.Http.Connections"
                    //"Microsoft.AspNetCore.Server.Kestrel"
                );
                metrics.AddPrometheusExporter();
                metrics.AddReader(new PeriodicExportingMetricReader(
                    inProcessExporter,
                    options.PushIntervalSeconds * 1000));
            });

        var app = builder.Build();
        app.MapPrometheusScrapingEndpoint("/metrics");

        await app.RunAsync();
    }
}
