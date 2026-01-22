using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Xunit;
using MetricsPush;
using ZstdSharp;

namespace Tests.MetricsPush;

public class MetricsPusherTests : IDisposable
{
    private readonly HttpListener _listener;
    private readonly string _listenUrl;
    private readonly Task _serverTask;
    private readonly CancellationTokenSource _serverCts;
    private byte[] _lastReceivedBody;
    private string _lastContentEncoding;

    public MetricsPusherTests()
    {
        int port = new Random().Next(20000, 30000);
        _listenUrl = $"http://127.0.0.1:{port}/push/";
        _listener = new HttpListener();
        _listener.Prefixes.Add(_listenUrl);
        _listener.Start();
        
        _serverCts = new CancellationTokenSource();
        _serverTask = Task.Run(() => ServerLoop(_serverCts.Token));
    }

    private async Task ServerLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                var context = await _listener.GetContextAsync();
                var request = context.Request;
                
                using (var ms = new MemoryStream())
                {
                    await request.InputStream.CopyToAsync(ms);
                    _lastReceivedBody = ms.ToArray();
                }
                _lastContentEncoding = request.Headers["Content-Encoding"];

                context.Response.StatusCode = 200;
                context.Response.Close();
            }
            catch (HttpListenerException)
            {
                // Listener stopped
                break;
            }
            catch
            {
                // Ignore
            }
        }
    }

    [Fact]
    public async Task TestMetricsPushFlow()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        var tags = new Dictionary<string, string> { { "env", "test" } };
        
        // Use a short interval for testing
        // Note: PeriodicExportingMetricReader respects the interval.
        var pusher = new MetricsPusher(1, _listenUrl, tags, builder);
        
        using var app = builder.Build();
        await app.StartAsync(); // Start the host to activate OTel
        
        // Act
        // We wait for some time to allow the timer to trigger and push.
        
        // We also need to ensure some metrics are recorded.
        MetricsPushTelemetry.PushCount.Add(10);
        
        await Task.Delay(3500); // Wait for at least one or two pushes

        // Assert
        Assert.NotNull(_lastReceivedBody);
        Assert.True(_lastReceivedBody.Length > 0);
        Assert.Equal("zstd", _lastContentEncoding);

        // Decompress and verify
        using var decompressor = new Decompressor();
        var decompressed = decompressor.Unwrap(_lastReceivedBody.AsSpan()).ToArray();
        string text = Encoding.UTF8.GetString(decompressed);
        
        Assert.Contains("metrics_push_count", text);
        Assert.Contains("env=\"test\"", text);
        
        pusher.Dispose();
        await app.StopAsync();
    }

    public void Dispose()
    {
        _serverCts.Cancel();
        _listener.Stop();
        _listener.Close();
    }
}
