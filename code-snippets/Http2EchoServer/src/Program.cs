using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Http2EchoServer;

internal static class Program {
    private static readonly System.IO.Stream Stdout = Console.OpenStandardOutput();
    private static readonly object LogLock = new object();

    public static async Task<int> Main(string[] args) {
        var portOption = new Option<int>("-http2.port", "HTTP/2 listen port.");
        portOption.AddAlias("--http2.port");
        portOption.IsRequired = true;
        portOption.AddValidator(result => {
            if (result.GetValueOrDefault<int>() <= 0) {
                result.ErrorMessage = "http2.port must be greater than 0.";
            }
        });

        var http1PortOption = new Option<int?>("-http1.port", "HTTP/1.1 listen port (e.g. 8082).");
        http1PortOption.AddAlias("--http1.port");
        http1PortOption.AddValidator(result => {
            int? value = result.GetValueOrDefault<int?>();
            if (value.HasValue && value.Value <= 0) {
                result.ErrorMessage = "http1.port must be greater than 0.";
            }
        });

        var outputLogOption = new Option<bool>("-outputlog", () => false, "Output request logs.");
        outputLogOption.AddAlias("--outputlog");

        var maxThreadsOption = new Option<int?>("-threadpool.max", "Set ThreadPool maximum worker threads.");
        maxThreadsOption.AddAlias("--threadpool.max");

        var root = new RootCommand("Http2EchoServer");
        root.AddOption(portOption);
        root.AddOption(http1PortOption);
        root.AddOption(outputLogOption);
        root.AddOption(maxThreadsOption);

        root.SetHandler(async (InvocationContext context) => {
            int port = context.ParseResult.GetValueForOption(portOption);
            int? http1Port = context.ParseResult.GetValueForOption(http1PortOption);
            bool outputLog = context.ParseResult.GetValueForOption(outputLogOption);
            int? maxThreads = context.ParseResult.GetValueForOption(maxThreadsOption);

            ConfigureThreadPool(maxThreads);

            if (!TryEnsurePortAvailable(port, out string errorMessage)) {
                Console.Error.WriteLine(errorMessage);
                context.ExitCode = 1;
                return;
            }

            if (http1Port.HasValue) {
                if (http1Port.Value == port) {
                    Console.Error.WriteLine("http1.port must be different from http2.port.");
                    context.ExitCode = 1;
                    return;
                }
                if (!TryEnsurePortAvailable(http1Port.Value, out errorMessage)) {
                    Console.Error.WriteLine(errorMessage);
                    context.ExitCode = 1;
                    return;
                }
            }

            await RunServerAsync(port, http1Port, outputLog);
        });

        return await root.InvokeAsync(args);
    }

    private static void ConfigureThreadPool(int? maxThreads) {
        if (!maxThreads.HasValue) {
            return;
        }
        ThreadPool.SetMinThreads(maxThreads.Value, maxThreads.Value);
        ThreadPool.SetMaxThreads(maxThreads.Value, maxThreads.Value);
    }

    private static async Task RunServerAsync(int port, int? http1Port, bool outputLog) {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions {
            Args = Array.Empty<string>()
        });

        builder.Logging.ClearProviders();
        builder.Logging.SetMinimumLevel(LogLevel.Warning);

        builder.WebHost.ConfigureKestrel(options => {
            options.ListenAnyIP(port, listenOptions => {
                listenOptions.Protocols = HttpProtocols.Http2;
            });
            if (http1Port.HasValue) {
                options.ListenAnyIP(http1Port.Value, listenOptions => {
                    listenOptions.Protocols = HttpProtocols.Http1;
                });
            }
        });

        var app = builder.Build();
        app.Map("/echo", async context => {
            await HandleEchoRequest(context, outputLog);
        });

        using var cts = new CancellationTokenSource();
        using var sigterm = PosixSignalRegistration.Create(PosixSignal.SIGTERM, context => {
            context.Cancel = true;
            cts.Cancel();
        });
        Console.CancelKeyPress += (_, e) => {
            e.Cancel = true;
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

    private static async Task HandleEchoRequest(HttpContext context, bool outputLog) {
        var req = context.Request;
        using var rent = new Common.RentedBuffer(2048);
        rent.Append(req.Method);
        rent.Append((byte)' ');
        rent.Append(req.Path.ToString());
        rent.Append(req.QueryString.ToString());
        rent.Append((byte)' ');
        rent.Append(req.Protocol);
        rent.Append("\r\n"u8);
        foreach (var header in req.Headers) {
            rent.Append(header.Key);
            rent.Append(": "u8);
            rent.Append(header.Value.ToString());
            rent.Append("\r\n"u8);
        }
        rent.Append("\r\n"u8);

        context.Response.ContentType = "text/plain";
        context.Response.StatusCode = StatusCodes.Status200OK;
        await context.Response.BodyWriter.WriteAsync(rent.Data!.AsMemory(0, rent.Length), context.RequestAborted);

        if (outputLog) {
            LogRequest(context, context.Response.StatusCode);
        }
    }

    private static void LogRequest(HttpContext context, int statusCode) {
        var req = context.Request;
        using var rent = new Common.RentedBuffer(512);
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
        rent.Append("}\n"u8);
        lock (LogLock) {
            Stdout.Write(rent.Bytes());
        }
    }

    private static bool TryEnsurePortAvailable(int port, out string errorMessage) {
        if (TryBindPort(AddressFamily.InterNetworkV6, IPAddress.IPv6Any, port, dualMode: true, out SocketError? socketError, out bool unsupported)) {
            errorMessage = string.Empty;
            return true;
        }

        if (unsupported) {
            if (TryBindPort(AddressFamily.InterNetwork, IPAddress.Any, port, dualMode: false, out socketError, out _)) {
                errorMessage = string.Empty;
                return true;
            }
        }

        errorMessage = socketError == SocketError.AddressAlreadyInUse
            ? $"Port {port} is already in use."
            : $"Port {port} is unavailable.";
        return false;
    }

    private static bool TryBindPort(AddressFamily family, IPAddress address, int port, bool dualMode, out SocketError? socketError, out bool unsupported) {
        socketError = null;
        unsupported = false;

        try {
            using var socket = new Socket(family, SocketType.Stream, ProtocolType.Tcp);
            if (dualMode) {
                socket.DualMode = true;
            }

            socket.Bind(new IPEndPoint(address, port));
            return true;
        }
        catch (PlatformNotSupportedException) {
            unsupported = true;
            return false;
        }
        catch (NotSupportedException) {
            unsupported = true;
            return false;
        }
        catch (SocketException ex) when (
            ex.SocketErrorCode == SocketError.AddressFamilyNotSupported ||
            ex.SocketErrorCode == SocketError.ProtocolNotSupported ||
            ex.SocketErrorCode == SocketError.OperationNotSupported) {
            socketError = ex.SocketErrorCode;
            unsupported = true;
            return false;
        }
        catch (SocketException ex) {
            socketError = ex.SocketErrorCode;
            return false;
        }
    }
}
