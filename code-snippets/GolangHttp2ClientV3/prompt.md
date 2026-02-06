# 目标

基于 tcp 实现一个 golang 的 http2 协议的压测客户端，用于测试 http2 的服务器。

期望：
* 建立 N 个 tcp 连接
* 每个连接上并发 2 个协程，通过 http2 的 stream 机制来提升性能
  - 一个协程专门 send
  - 一个协程专门 recv

# 核心代码

参考下面代码的实现:

```go
// 仅展示结构：省略错误处理、ACK 逻辑、流控、CONTINUATION、HPACK 动态表等细节
conn, _ := net.Dial("tcp", "127.0.0.1:8081")

// 1) h2c prior-knowledge preface
conn.Write([]byte("PRI * HTTP/2.0\r\n\r\nSM\r\n\r\n"))

fr := http2.NewFramer(conn, conn)

// 2) 发送客户端 SETTINGS
fr.WriteSettings(
    http2.Setting{ID: http2.SettingEnablePush, Val: 0},
    // 你也可以声明初始窗口等
)

// 3) 写 N 个 stream：每个 stream = HEADERS(+DATA)
var encBuf bytes.Buffer
enc := hpack.NewEncoder(&encBuf)

writeReq := func(streamID uint32, path string) {
    encBuf.Reset()
    enc.WriteField(hpack.HeaderField{Name: ":method", Value: "GET"})
    enc.WriteField(hpack.HeaderField{Name: ":scheme", Value: "http"})
    enc.WriteField(hpack.HeaderField{Name: ":authority", Value: "127.0.0.1:8081"})
    enc.WriteField(hpack.HeaderField{Name: ":path", Value: path})

    fr.WriteHeaders(http2.HeadersFrameParam{
        StreamID:      streamID,
        BlockFragment: encBuf.Bytes(),
        EndHeaders:    true,
        EndStream:     true, // GET 无 body
    })
}

for i := 0; i < 80; i++ {
    sid := uint32(1 + i*2) // client-initiated 必须奇数
    writeReq(sid, "/echo")
}

// 4) 读循环：按 StreamID 分发
for {
    f, _ := fr.ReadFrame()
    sid := f.Header().StreamID
    switch ff := f.(type) {
    case *http2.HeadersFrame:
        _ = sid; _ = ff // 解析 headers block（需要 hpack.Decoder）
    case *http2.DataFrame:
        _ = sid; _ = ff.Data() // 这是对应 stream 的响应 body 分片
    }
}
```

# 命令行参数

- 命令行参数 `-thread.count=123`: 如果提供这个参数，设置 GOMAXPROCS
- 命令行参数 `-connection.count=123`: 允许的 tcp 连接数，默认是 GOMAXPROCS*2
- 命令行参数 `-target.url=http://xxx/`: 配置要访问的目标地址
- 命令行参数 `-duration.seconds=123`: 配置压测的时间
  - 运行达到 `-duration.seconds` 的秒数后，通知所有协程，退出程序
- 命令行参数 `-check.output=true/false`: 是否检查结果中的 "seq=${seq}"
- 命令行参数 `-max.concurrent.streams=123`: 配置每个连接上的最大 stream 数量

# 客户端发送逻辑

- 每一组生产者和消费者上，定义一个 atomic.Int64
  - 每发送一个请求，atomic.Int64 加 1
  - 这个值达到 `max.concurrent.streams` 设定的值时，调用 GOSched(), 让出协程调度。只有这个值小于 `max.concurrent.streams` 时，才能继续生产
- 每个生产者协程上存在一个 Int64，每次请求前获得一个位置的序列号。在 `-target.url` 后面加上 "seq=${seq}"

# 客户端接收逻辑

* 如果存在配置 `-check.output=true`
  - 收到结果后，检查文本中是否存在 "seq=${seq}"，如果没找到，立即 panic
* 每收到一个响应，对生产者上增加的 atomic.Int64 进行减 1 操作

# 错误

如果发生错误，使用 log.Println 打印详细信息

# 数据统计

- 按照 response 的 status_code 来汇总，分别显示每个状态码的 qps
- 记录时延的分布，分布的跨度按照 2 的 n 次幂来展示。从 10 微秒开始。运行完成后输出总的延迟分布，按照百分比展示。时间从少到多，显示积累的百分比，这样我就能看到 top n percent 的数据情况。

# 代码规范

* 每个函数不超过 100 行。
* 重用对象，尽可能做到 0 alloc。避免因为 gc 而影响性能。
