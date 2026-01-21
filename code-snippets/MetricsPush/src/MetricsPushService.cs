using System.Net.Http.Headers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MetricsPush;

internal sealed class MetricsPushService : BackgroundService
{
    //private static long _payloadIndex;
    private readonly InProcessMetricsExporter _exporter;
    private readonly MetricsPushOptions _options;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<MetricsPushService> _logger;

    public MetricsPushService(
        InProcessMetricsExporter exporter,
        MetricsPushOptions options,
        IHttpClientFactory httpClientFactory,
        ILogger<MetricsPushService> logger)
    {
        _exporter = exporter;
        _options = options;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_options.PushIntervalSeconds));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await PushOnceAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Metrics push failed.");
            }
        }
    }

    private async Task PushOnceAsync(CancellationToken cancellationToken)
    {
        byte[] payload = _exporter.GetSnapshot();
        if (payload.Length == 0)
        {
            return;
        }

        // long index = Interlocked.Increment(ref _payloadIndex);
        // string path = Path.Combine(Directory.GetCurrentDirectory(), $"metrics_payload_{index}.txt");
        // try
        // {
        //     await File.WriteAllBytesAsync(path, payload, cancellationToken);
        // }
        // catch (Exception ex)
        // {
        //     _logger.LogWarning(ex, "Failed to write payload snapshot to {Path}.", path);
        // }

        byte[] compressed = ZstdCompressor.Compress(payload);
        using var content = new ByteArrayContent(compressed);
        content.Headers.ContentType = new MediaTypeHeaderValue("text/plain") { CharSet = "utf-8" };
        content.Headers.ContentEncoding.Add("zstd");

        var client = _httpClientFactory.CreateClient();
        using var response = await client.PostAsync(_options.PushAddress, content, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Metrics push failed with status {StatusCode}.", response.StatusCode);
            return;
        }

        MetricsPushTelemetry.PushCount.Add(1);
        MetricsPushTelemetry.PayloadBytes.Record(compressed.Length);
        MetricsPushTelemetry.PayloadUncompressedBytes.Record(payload.Length);
    }
}
