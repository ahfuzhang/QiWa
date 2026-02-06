# 目标

实现一个 golang 的 http2 协议的压测客户端，用于测试 http2 的服务器。

期望：
* 建立 N 个 tcp 连接
* 每个连接上并发 n 个协程，通过 http2 的 stream 机制来提升性能

# 命令行参数

- 命令行参数 `-thread.count=123`: 如果提供这个参数，设置 GOMAXPROCS
- 命令行参数 `-connection.count=123`: 允许的 tcp 连接数，默认是 GOMAXPROCS*2
  - 程序启动时，创建 `connection.count` 个数相当的 http2.Transport

http2.Transport 的代码参考如下:

```go
func xx(){
// 自己控制 Dial：这样可以确保所有请求都复用同一个 TCP 连接
	dialOnce := newSingleConnDialer(addr)

	tr := &http2.Transport{
		AllowHTTP: true, // 允许 h2c（明文）
		DialTLSContext: func(ctx context.Context, network, _ string, _ *tls.Config) (net.Conn, error) {
			return dialOnce(ctx, network)
		},
		// 你也可以调整一些参数，但一般不需要
		// MaxHeaderListSize: ...
	}
}

// 只拨一次号，后续重复使用同一个 net.Conn
func newSingleConnDialer(addr string) func(ctx context.Context, network string) (net.Conn, error) {
	var (
		once sync.Once
		c    net.Conn
		err  error
	)
	return func(ctx context.Context, network string) (net.Conn, error) {
		once.Do(func() {
			var d net.Dialer
			c, err = d.DialContext(ctx, network, addr)
		})
		return c, err
}  
```

  - 每个 http2.Transport 又包装到 http.Client 对象

http client 的对象参考如下:

```go
	c := &http.Client{
		Transport: tr,
		Timeout:   10 * time.Second,
	}
```

- 命令行参数 `-coroutine.count.per.connection=123`: 每个 tcp 连接上面的协程数
  - 对 `connection.count` 个 http.Client 对象 的每个对象，各自创建 `coroutine.count.per.connection` 个协程。也就是说 `coroutine.count.per.connection` 个协程共用一个 http.Client
- 命令行参数 `-target.url=http://xxx/`: 配置要访问的目标地址
- 命令行参数 `-duration.seconds=123`: 配置压测的时间
  - 运行达到 `-duration.seconds` 的秒数后，通知所有协程，退出程序

# http2 客户端对象

  - 参考下面的代码:

```go
package main

import (
	"context"
	"fmt"
	"io"
	"net"
	"net/http"
	"sync"
	"time"

	"golang.org/x/net/http2"
)

func main() {
	// 目标：h2c server，例如 127.0.0.1:8081
	addr := "127.0.0.1:8081"
	url := "http://" + addr + "/echo"

	// 自己控制 Dial：这样可以确保所有请求都复用同一个 TCP 连接
	dialOnce := newSingleConnDialer(addr)

	tr := &http2.Transport{
		AllowHTTP: true, // 允许 h2c（明文）
		DialTLSContext: func(ctx context.Context, network, _ string, _ *tls.Config) (net.Conn, error) {
			return dialOnce(ctx, network)
		},
		// 你也可以调整一些参数，但一般不需要
		// MaxHeaderListSize: ...
	}

	c := &http.Client{
		Transport: tr,
		Timeout:   10 * time.Second,
	}

	// 并发发多个请求 => 一个连接里开多个 stream
	n := 80 // 你想要的并发 stream 数
	var wg sync.WaitGroup
	wg.Add(n)

	for i := 0; i < n; i++ {
		go func(i int) {
			defer wg.Done()
			req, _ := http.NewRequest("GET", url, nil)
			req.Header.Set("X-Req", fmt.Sprintf("%d", i))

			resp, err := c.Do(req)
			if err != nil {
				fmt.Println("req", i, "err:", err)
				return
			}
			defer resp.Body.Close()
			b, _ := io.ReadAll(resp.Body)
			fmt.Println("req", i, "status:", resp.StatusCode, "len:", len(b))
		}(i)
	}

	wg.Wait()
}

// 只拨一次号，后续重复使用同一个 net.Conn
func newSingleConnDialer(addr string) func(ctx context.Context, network string) (net.Conn, error) {
	var (
		once sync.Once
		c    net.Conn
		err  error
	)
	return func(ctx context.Context, network string) (net.Conn, error) {
		once.Do(func() {
			var d net.Dialer
			c, err = d.DialContext(ctx, network, addr)
		})
		return c, err
}
```

* 客户端检查逻辑
  - 存在一个全局的 atomic.Int64，每次请求前获得一个位置的序列号。在 `-target.url` 后面加上 "seq=${seq}"
  - 收到结果后，检查文本中是否存在 "seq=${seq}"，如果没找到，立即 panic

# 数据统计

- 按照 response 的 status_code 来汇总，分别显示每个状态码的 qps
- 记录时延的分布，分布的跨度按照 2 的 n 次幂来展示。从 10 微秒开始。运行完成后输出总的延迟分布，按照百分比展示。时间从少到多，显示积累的百分比，这样我就能看到 top n percent 的数据情况。

# 代码规范

* 每个函数不超过 100 行。
* 重用对象，尽可能做到 0 alloc。避免因为 gc 而影响性能。
