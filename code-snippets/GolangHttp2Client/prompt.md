* 目标：实现一个 golang 的 http2 协议的压测客户端，用于测试 http2 的服务器
* 步骤:
  - 命令行参数 `-thread.count=123`: 如果提供这个参数，设置 GOMAXPROCS
  - 命令行参数 `-coroutine.count=123`: 允许的协程数，默认是当前 GOMAXPROCS*10
  - 命令行参数 `-connection.count=123`: 允许的 tcp 连接数，默认是 GOMAXPROCS*2
  - 命令行参数 `-max.concurrent.streams=123`: 每个 tcp 连接上面的 stream 数量，默认是 80
  - 命令行参数 `-target.url=http://xxx/`: 配置要访问的目标地址
  - 命令行参数 `-duration.seconds=123`: 配置压测的时间
  - 创建 net/http 下面的 HttpClient, 使用 google 的库，让其支持 h2c 协议
  - 开启 `-coroutine.count` 中配置的协程，使用 HttpClient 来在循环中不断请求 `-target.url`:
    - 存在一个全局的 atomic.Int64，每次请求前获得一个位置的序列号。在 `-target.url` 后面加上 "seq=${seq}"
    - 收到结果后，检查文本中是否存在 "seq=${seq}"，如果没找到，立即 panic
  - 运行达到 `-duration.seconds` 的秒数后，通知所有协程，退出程序
  - 数据统计：
    - 按照 response 的 status_code 来汇总，分别显示每个状态码的 qps
    - 记录时延的分布，分布的跨度按照 2 的 n 次幂来展示。从 10 微秒开始。运行完成后输出总的延迟分布，按照百分比展示。
    