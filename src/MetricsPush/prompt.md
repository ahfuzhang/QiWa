* 目的：提供一个类，可以把 OpenTelemetry 库的各种 metrics 数据，先构造为 prometheus 格式的文本，然后做 zstd 压缩，然后使用 http post 发送到 VictoriaMetrics 的 tsdb 上。
* 参考：目前已经在项目的根目录下的 ./code-snippets/MetricsPush 下实现了一个， 已经经过了充分的测试。需要把其中的逻辑抽取出来形成可以重复使用的类。
* 规格要求:
  - 类的构造函数参数如下:
    - int intervalSeconds: push 的时间间隔
    - string pushAddr: push 的 url
    - Dictionary<string, string> publicTags: 每个 metrics 中的公共 tag
    - 传入 WebApplication.CreateBuilder() 后的对象，便于在对象上配置 OpenTelemetry 的各种选项。
      - 也就是希望在类里面封装繁琐的 OpenTelemetry 的初始化过程。可以参考下面的代码:

      ```csharp
        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddMeter(MetricsPushTelemetry.Meter.Name);
                metrics.AddProcessInstrumentation();
                metrics.AddRuntimeInstrumentation();
                metrics.AddHttpClientInstrumentation();
                metrics.AddAspNetCoreInstrumentation();
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
      ```

  - 内部提供定时器，在每个 intervalSeconds 的时间间隔中，获取 OpenTelemetry 的 metrics 数据，构造为 prometheus 格式的文本，然后做 zstd 压缩，然后使用 http post 发送到 VictoriaMetrics 的 tsdb 上。
  - 提供一个 Dispose() 方法，用于停止定时器，并清理资源
  - 不要使用 string 类型来处理字符串，基于 utf-8 的字节流来处理
  - 避免内存分配，尽量使用 Common.RentedBuffer 来进行显式的内存分配和释放，从而降低 gc 的压力。
* 输出:
  - 代码: 在 ./src/MetricsPush/ 目录下新增类文件
  - 测试用例： 在 ./Tests/MetricsPush/ 目录下生成测试用例
    - 生成后运行测试用例，确保测试通过
