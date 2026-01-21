* 目标:
  - 定期采集 open telemetry 的 metrics 数据，然后以 push 的模式发送给类似 VictoriaMetrics 这样的 tsdb
  - 本项目为一个命令行程序。提供如下命令行参数：
    - `-http1.port=8081`: 设置 http 1.1 协议的 Kestrel 服务器的监听端口
    - `-push.interval.seconds=10`: 设置推送 metrics 数据的频率
    - `-push.addr=http://xxxx/`: 设置推送的目标地址
    - `-extraLabels=a=b&c=d&e=f`: 额外的公共标签，需要把 a=b c=d e=f 解析为键值对
* 约束:
  - 源码放到当前文件所在目录的 ./src/ 目录下
  - 部分功能函数，都生成对应的单元测试的函数，放到 ./Tests/ 目录
  - 使用 Option<> 类型来解析命令行
  - 使用库 `using OpenTelemetry.Metrics` 来收集 metrics 数据
  - 基于 utf-8 的字节流来处理，不要做 utf-8 <=> utf-16 的相互转换。
  - 生成 Makefile，提供 build / run / test 等命令。 build 的结果放到项目根目录下的 ./build/code-snippets/MetricsPush/ 下面
* 参考代码:
  - 1 自定义一个 exporter

```csharp
using OpenTelemetry;
using OpenTelemetry.Metrics;

public sealed class InProcessMetricsExporter : BaseExporter<Metric>
{
    private readonly object _lock = new();
    private List<Metric> _latest = new();

    public override ExportResult Export(in Batch<Metric> batch)
    {
        lock (_lock)
        {
            _latest = batch.ToList();
        }
        return ExportResult.Success;
    }

    public IReadOnlyList<Metric> GetSnapshot()
    {
        lock (_lock)
        {
            return _latest.ToList();
        }
    }
}
```

  - 2 注册到 OpenTelemetry

```csharp
var inProcessExporter = new InProcessMetricsExporter();

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics.AddMeter(Meter.Name);
        metrics.AddPrometheusExporter();

        metrics.AddReader(new PeriodicExportingMetricReader(inProcessExporter)
        {
            ExportIntervalMilliseconds = 5000  /*这里改为命令行参数的值*/
        });
    });
```


* 实现步骤
  - 1 解析命令行参数
  - 2 监听端口， 从 `-http1.port=` 获取端口号
  - 3 把 /metrics 这个路径进行注册，访问这个路径能够获得 metrics 的文本
  - 4 定义类 InProcessMetricsExporter， 并在 `builder.Services.AddOpenTelemetry()` 系列的 api 中注册 reader
  - 4.1 在指定的间隔进行 metrics push，间隔的时间来自命令行 `-push.interval.seconds=`
  - 5 InProcessMetricsExporter 读取所有的  metric
  - 6 在 metrics 中加入 extraLables，也就是命令行 `-extraLabels=a=b&c=d&e=f` 中设置的 label
  - 7 把 metrics 格式化为文本，格式为 `metricName{publicLabels,labels} value`，每行一条 metric 数据
  - 8 把文本压缩成 zstd 格式
  - 9 使用 http post 发送到 `-push.addr=` 指定的地址
  - 9.1 发送的时候: ContentType=text/plain, ContentEncoding=zstd
