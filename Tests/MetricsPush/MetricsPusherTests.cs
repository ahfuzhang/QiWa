using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Xunit;
using MetricsPush;
using ZstdSharp;

namespace Tests.MetricsPush;

public class MetricsPusherTests : IDisposable {
    private readonly HttpListener _listener;
    private readonly string _listenUrl;
    private readonly Task _serverTask;
    private readonly CancellationTokenSource _serverCts;
    private readonly TaskCompletionSource<bool> _requestReceived;
    private byte[]? _lastReceivedBody;
    private string? _lastContentEncoding;

    public MetricsPusherTests() {
        int port = new Random().Next(20000, 30000);
        _listenUrl = $"http://127.0.0.1:{port}/push/";
        _listener = new HttpListener();
        _listener.Prefixes.Add(_listenUrl);
        _listener.Start();

        _serverCts = new CancellationTokenSource();
        _requestReceived = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        _serverTask = Task.Run(() => ServerLoop(_serverCts.Token));
    }

    private async Task ServerLoop(CancellationToken token) {
        while (!token.IsCancellationRequested) {
            try {
                var context = await _listener.GetContextAsync();
                var request = context.Request;

                using (var ms = new MemoryStream()) {
                    await request.InputStream.CopyToAsync(ms);
                    _lastReceivedBody = ms.ToArray();
                }
                _lastContentEncoding = request.Headers["Content-Encoding"];
                _requestReceived.TrySetResult(true);

                context.Response.StatusCode = 200;
                context.Response.Close();
            }
            catch (HttpListenerException) {
                // Listener stopped
                break;
            }
            catch {
                // Ignore
            }
        }
    }

    [Fact]
    public async Task TestMetricsPushFlow() {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        var tags = new Dictionary<string, string> { { "env", "test" } };

        using var pusher = new MetricsPusher(60, _listenUrl, tags, builder);

        var payload = Encoding.UTF8.GetBytes("metrics_push_count{env=\"test\"} 10\n");
        SeedExporter(pusher, payload);
        await InvokePushOnceAsync(pusher);
        await _requestReceived.Task.WaitAsync(TimeSpan.FromSeconds(2));

        // Assert
        Assert.NotNull(_lastReceivedBody);
        Assert.True(_lastReceivedBody!.Length > 0);
        Assert.Equal("zstd", _lastContentEncoding);

        // Decompress and verify
        using var decompressor = new Decompressor();
        var decompressed = decompressor.Unwrap(_lastReceivedBody.AsSpan()).ToArray();
        string text = Encoding.UTF8.GetString(decompressed);

        Assert.Contains("metrics_push_count", text);
        Assert.Contains("env=\"test\"", text);
    }

    public void Dispose() {
        _serverCts.Cancel();
        _listener.Stop();
        _listener.Close();
    }

    private static void SeedExporter(MetricsPusher pusher, byte[] payload) {
        var exporterField = typeof(MetricsPusher).GetField("_exporter", BindingFlags.NonPublic | BindingFlags.Instance);
        var exporter = (InProcessMetricsExporter)exporterField!.GetValue(pusher)!;

        var buffer = new Common.RentedBuffer {
            Data = payload,
            Length = payload.Length
        };

        var lockField = typeof(InProcessMetricsExporter).GetField("_lock", BindingFlags.NonPublic | BindingFlags.Instance);
        var gate = lockField!.GetValue(exporter)!;
        lock (gate) {
            typeof(InProcessMetricsExporter).GetField("_latest", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(exporter, buffer);
            typeof(InProcessMetricsExporter).GetField("_latestUsed", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(exporter, payload.Length);
        }
    }

    private static Task InvokePushOnceAsync(MetricsPusher pusher) {
        var method = typeof(MetricsPusher).GetMethod("PushOnceAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        return (Task)method!.Invoke(pusher, new object[] { CancellationToken.None })!;
    }
}
