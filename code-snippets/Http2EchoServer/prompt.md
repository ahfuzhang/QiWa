本例子演示：提供一个 http 2 协议的 http 服务器, 原样返回请求的内容

To my dear AI:
* 目的：实现一个 echo 功能的 http 2 协议的服务器，用于测试 Kestrel 框架的性能。
* 命令行参数:
  - `-http2.port=8080`: 指定 http 2 协议的 http 服务器的端口
  - `-threadpool.max=1`: Set ThreadPool maximum worker threads.
* 规范:
  - 代码生成到当前文件目录的 ./src/ 目录下
  - 每个函数的长度不要超过 120 行
* 步骤:
  1. 使用 Option<T> 来解析命令行参数
  2. 当设置 `-threadpool.max=1` 时，设置 `ThreadPool.SetMaxThreads`。参考项目根目录下的代码: /code-snippets/SetMaxThreads/src/Program.cs
  3. 参数 `-http2.port=8081` 必须设置，否则报错。启动一个 http 2 的服务器，启动前要检查端口是否被占用。参考项目根目录下的代码: /code-snippets/MetricsPush/src/Program.cs
  4. 支持 graceful shutdown，参考项目根目录下的代码: /code-snippets/GracefulShutdown/src/Program.cs
  5. 提供接口 `/echo`，原样返回请求的内容，包括 Header，返回类似为 `GET /XXX HTTP/2\r\nHeaders: xx\r\n\r\n` 的内容。 打印请求日志，把 $method $host$path$querystring $status_code 这样的一行写到日志中。
  6. 日志输出到 stdout，使用 json 格式。日志的时间格式为 _time, 格式化为: `2025-11-25T06:28:33.684852008Z` 这样的格式。
