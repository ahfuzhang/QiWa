本例子演示：提供一个 http 1.1 协议的 http 服务器, 原样返回请求的内容

To my dear AI:
* 目的：实现一个 echo 功能的 http 1.1 协议的服务器，用于测试 Kestrel 框架的性能。
* 命令行参数:
  - `-http1.port=8080`: 指定 http 1.1 协议的 http 服务器的端口
  - `-metrics.push.interval.seconds=10`: 指定指标推送的间隔时间，单位为秒。默认值为 15
  - `-metrics.push.addr=http://127.0.0.1:8080`: 指定指标推送的地址
  - `-metrics.push.extra.labels=a=b&c=d`: Extra labels, e.g. a=b&c=d.
  - `-threadpool.max=1`: Set ThreadPool maximum worker threads.
* 规范:
  - 代码生成到当前文件目录的 ./src/ 目录下
  - 每个函数的长度不要超过 120 行
* 步骤:
  1. 使用 Option<T> 来解析命令行参数
  2. 当设置 `-threadpool.max=1` 时，设置 `ThreadPool.SetMaxThreads`。参考项目根目录下的代码: /code-snippets/SetMaxThreads/src/Program.cs
  3. 参数 `-http1.port=8080` 必须设置，否则报错。启动一个 http 1.1 的服务器，启动前要检查端口是否被占用。参考项目根目录下的代码: /code-snippets/MetricsPush/src/Program.cs
  4. 支持 graceful shutdown，参考项目根目录下的代码: /code-snippets/GracefulShutdown/src/Program.cs
  5.  当存在参数 `-metrics.push.addr` 时，启动指标推送器。使用已经写好的类:  /src/MetricsPush/MetricsPusher.cs
  6. 当通过 http 端口访问 `/metrics` 时，返回指标数据。
  7. 提供接口 `/echo`，原样返回请求的内容，包括 Header，返回类似为 `GET /XXX HTTP/1.1\r\nHeaders: xx\r\n\r\n` 的内容。 打印请求日志，把 $method $host$path$querystring $status_code 这样的一行写到日志中。
  8.  日志输出到 stdout，使用 json 格式。日志的时间格式为 _time, 格式化为: `2025-11-25T06:28:33.684852008Z` 这样的格式。

----

* metrics:
  - rate(http_request_total{pod=~"qiwa-metrics-push-88d88b5c7-00008"}[1m])
  - 46.3k / s, 无日志输出
  
