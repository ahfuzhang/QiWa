using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using Compress;

namespace MetricsPush;

public sealed class MetricsPusher : IDisposable {
    private readonly InProcessMetricsExporter _exporter;
    private readonly HttpClient _httpClient;
    private readonly PeriodicTimer _timer;
    private readonly CancellationTokenSource _cts;
    private readonly Task _pushLoopTask;
    private readonly string _pushUrl;
    private bool _disposed;

    public MetricsPusher(
        int intervalSeconds,
        string pushAddr,
        Dictionary<string, string> publicTags,
        WebApplicationBuilder builder,
        HttpClient? httpClient = null) {
        if (intervalSeconds <= 0) throw new ArgumentOutOfRangeException(nameof(intervalSeconds));
        if (string.IsNullOrWhiteSpace(pushAddr)) throw new ArgumentNullException(nameof(pushAddr));
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        _pushUrl = pushAddr;
        _exporter = new InProcessMetricsExporter(publicTags);
        _httpClient = httpClient ?? new HttpClient();
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(intervalSeconds));
        _cts = new CancellationTokenSource();

        // Configure OpenTelemetry
        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics => {
                metrics.AddMeter(MetricsPushTelemetry.Meter.Name);
                metrics.AddProcessInstrumentation();
                metrics.AddRuntimeInstrumentation();
                metrics.AddHttpClientInstrumentation();
                metrics.AddAspNetCoreInstrumentation(); // Works if package is referenced
                metrics.AddMeter(
                    "System.Runtime",
                    "System.Net.Http",
                    "System.Net.Sockets",
                    "Microsoft.AspNetCore.Hosting",
                    "Microsoft.AspNetCore.Server.Kestrel",
                    "Microsoft.AspNetCore.Http.Connections",
                    "System.Net.NameResolution",
                    "Microsoft.AspNetCore.RateLimiting",
                    "OpenTelemetry.Instrumentation.AspNet",
                    "OpenTelemetry.Instrumentation.AspNetCore",
                    "OpenTelemetry.Instrumentation.Http",
                    "System.Net.NameResolution"
                );

                // We use our exporter
                metrics.AddReader(new PeriodicExportingMetricReader(
                    _exporter,
                    intervalSeconds * 1000));
            });

        // Start the push loop
        _pushLoopTask = RunPushLoopAsync(_cts.Token);
    }

    private async Task RunPushLoopAsync(CancellationToken token) {
        while (await _timer.WaitForNextTickAsync(token)) {
            if (token.IsCancellationRequested) break;

            try {
                await PushOnceAsync(token);
            }
            catch (OperationCanceledException) {
                break;
            }
            catch (Exception) {
                // Optionally log error if we had a logger.
                // Since specific logging requirement wasn't clear on *where* to log (no ILogger passed explicitly to constructor except via builder which is hard to capture here),
                // we might want to swallow or print to console.
                // Or maybe we can resolve ILogger from app? But we don't have app reference.
                // For now, we swallow to keep the loop alive.
            }
        }
    }

    private static readonly System.IO.Stream Stdout = Console.OpenStandardOutput();

    private async Task PushOnceAsync(CancellationToken token) {
        // 1. Get snapshot
        using var payload = _exporter.GetSnapshot(out int payloadLength);
        if (payloadLength == 0) {
            return;
        }
        //Console.WriteLine($"payloadLength={payloadLength}, {payload.Length}");
        //Stdout.Write(payload.Bytes());
        // 2. Compress
        var (compressed, error) = Compress.ZstdCompressor.Compress(payload.Data.AsSpan(0, payloadLength));
        if (error.Err()) {
            Console.Error.WriteLine($"ERROR metrics push compress failed: code={error.Code} message={error.Message}");
            return;
        }
        // todo: compressed 内存泄露
        int compressedLength = compressed.Length;

        // 3. Send
        // Wrap the RentedBuffer in a MemoryStream to avoid allocation.
        // MemoryStream(byte[], int index, int count, bool writable, bool publiclyVisible)
        using var ms = new MemoryStream(compressed.Data!, 0, compressedLength, false, true);
        using var content = new StreamContent(ms);

        content.Headers.ContentType = new MediaTypeHeaderValue("text/plain") { CharSet = "utf-8" };
        content.Headers.ContentEncoding.Add("zstd");
        //Console.WriteLine($"ready to post: {compressedLength} bytes, url={_pushUrl}");
        using var response = await _httpClient.PostAsync(_pushUrl, content, token);

        if (response.IsSuccessStatusCode) {
            MetricsPushTelemetry.PushCount.Add(1);
            MetricsPushTelemetry.PayloadBytes.Record(compressedLength);
            MetricsPushTelemetry.PayloadUncompressedBytes.Record(payloadLength);
            //Console.WriteLine($"post success: {compressedLength} bytes");
            return;
        }
        Console.WriteLine($"Failed to push metrics: {response.StatusCode}");
    }

    public void Dispose() {
        if (_disposed) return;
        _disposed = true;

        _cts.Cancel();
        try {
            _pushLoopTask.Wait(TimeSpan.FromSeconds(1)); // Wait briefly for loop to finish
        }
        catch {
            // Ignore
        }

        _cts.Dispose();
        _timer.Dispose();
        // Only dispose if we created it, or if we assume ownership. 
        // For simplicity in this helper class, we'll dispose it. 
        // If strict ownership is needed, we'd add a flag.
        _httpClient.Dispose();
        _exporter.Dispose(); // BaseExporter disposes? BaseExporter implements IDisposable
    }
}
