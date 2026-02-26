package main

import (
	"bytes"
	"context"
	"crypto/tls"
	"errors"
	"flag"
	"fmt"
	"io"
	"log"
	"math/bits"
	"net"
	"net/http"
	"net/url"
	"runtime"
	"sort"
	"strconv"
	"strings"
	"sync"
	"sync/atomic"
	"time"
	"unsafe"

	"golang.org/x/net/http2"
)

const latencyBase = 10 * time.Microsecond

type runConfig struct {
	threadCount       int           // 核数
	connectionCount   int           // 连接数
	coroutinesPerConn int           // 每个连接上的协程数
	duration          time.Duration // 压测持续时间
	targetURL         string        // 目标 url
	parsedURL         *url.URL
	seqPrefix         string
	addr              string // 解析后得到的 ip 和 端口
	checkOutput       bool   // 检查 echo 服务的返回，确保真的被服务器处理了
	singleConnection  bool   // 每个 http client 对象，限制只有一个 tcp 连接
	strictMaxStreams  bool   //???  *singleConnection && !effectiveStrict
	sendBufferBytes   int    // socket 的 send buffer
	recvBufferBytes   int    // socket 的 recv buffer
}

type workerStats struct {
	codeCounts     map[int]int64
	latencyBuckets []int64
	total          int64
	errors         int64
}

func newWorkerStats() *workerStats {
	return &workerStats{
		codeCounts:     make(map[int]int64),
		latencyBuckets: make([]int64, 32),
	}
}

var dailCount atomic.Int64

// 对延迟进行计数
func (ws *workerStats) addLatency(d time.Duration) {
	idx := latencyBucketIndex(d)
	if idx >= len(ws.latencyBuckets) {
		next := len(ws.latencyBuckets)
		if next == 0 {
			next = 1
		}
		for next <= idx {
			next *= 2
		}
		nb := make([]int64, next)
		copy(nb, ws.latencyBuckets)
		ws.latencyBuckets = nb
	}
	ws.latencyBuckets[idx]++
}

func latencyBucketIndex(d time.Duration) int {
	if d <= latencyBase {
		return 0
	}
	ratio := (d + latencyBase - 1) / latencyBase
	return bits.Len64(uint64(ratio - 1))
}

func parseConfig() (*runConfig, error) {
	threadCount := flag.Int("thread.count", 0, "set GOMAXPROCS")
	connectionCount := flag.Int("connection.count", 0, "number of TCP connections")
	coroutinesPerConn := flag.Int("coroutine.count.per.connection", 0, "goroutines per connection")
	targetURL := flag.String("target.url", "", "target URL")
	durationSeconds := flag.Int("duration.seconds", 10, "benchmark duration in seconds")
	checkOutput := flag.Bool("check.output", true, "check response body contains seq")
	singleConnection := flag.Bool("single.connection", false, "reuse a single TCP connection per http client")
	strictMaxStreams := flag.Bool("strict.max.concurrent.streams", false, "block on server stream limit instead of dialing extra connections")
	sendBufferBytes := flag.Int("send.buffer.bytes", 1<<20, "TCP send buffer size in bytes")
	recvBufferBytes := flag.Int("recv.buffer.bytes", 1<<20, "TCP recv buffer size in bytes and HTTP/2 receive window size")
	flag.Parse()

	if *threadCount > 0 {
		runtime.GOMAXPROCS(*threadCount)
	}
	gmp := runtime.GOMAXPROCS(0)
	connCount := *connectionCount
	if connCount <= 0 {
		connCount = gmp * 2
	}
	perConn := *coroutinesPerConn
	if perConn <= 0 {
		perConn = 80
	}
	if *durationSeconds <= 0 {
		return nil, fmt.Errorf("duration.seconds must be > 0")
	}
	if *sendBufferBytes < 0 {
		return nil, fmt.Errorf("send.buffer.bytes must be >= 0")
	}
	if *recvBufferBytes < 0 {
		return nil, fmt.Errorf("recv.buffer.bytes must be >= 0")
	}
	if *targetURL == "" {
		flag.Usage()
		return nil, fmt.Errorf("target.url is required")
	}

	parsedURL, err := url.Parse(*targetURL)
	if err != nil {
		return nil, fmt.Errorf("invalid target.url: %w", err)
	}
	if parsedURL.Scheme == "" || parsedURL.Host == "" {
		return nil, fmt.Errorf("target.url must include scheme and host")
	}

	addr := addrFromURL(parsedURL)
	effectiveStrict := *strictMaxStreams
	if *singleConnection && !effectiveStrict {
		effectiveStrict = true
	}
	return &runConfig{
		threadCount:       *threadCount,
		connectionCount:   connCount,
		coroutinesPerConn: perConn,
		duration:          time.Duration(*durationSeconds) * time.Second,
		targetURL:         *targetURL,
		parsedURL:         parsedURL,
		seqPrefix:         buildSeqPrefix(parsedURL),
		addr:              addr,
		checkOutput:       *checkOutput,
		singleConnection:  *singleConnection,
		strictMaxStreams:  effectiveStrict,
		sendBufferBytes:   *sendBufferBytes,
		recvBufferBytes:   *recvBufferBytes,
	}, nil
}

func addrFromURL(u *url.URL) string {
	host := u.Hostname()
	port := u.Port()
	if port == "" {
		if strings.EqualFold(u.Scheme, "https") {
			port = "443"
		} else {
			port = "80"
		}
	}
	return net.JoinHostPort(host, port)
}

func buildSeqPrefix(u *url.URL) string {
	tmp := *u
	tmp.Fragment = ""
	base := tmp.String()
	if strings.Contains(base, "?") {
		if strings.HasSuffix(base, "?") || strings.HasSuffix(base, "&") {
			return base + "seq="
		}
		return base + "&seq="
	}
	return base + "?seq="
}

// 对 net.Conn 的包装
type trackedConn struct {
	net.Conn
	closed atomic.Bool
}

func (c *trackedConn) Close() error {
	c.closed.Store(true)
	return c.Conn.Close()
}

func (c *trackedConn) Read(p []byte) (int, error) {
	n, err := c.Conn.Read(p)
	if err != nil {
		c.closed.Store(true)
	}
	return n, err
}

func (c *trackedConn) Write(p []byte) (int, error) {
	n, err := c.Conn.Write(p)
	if err != nil {
		c.closed.Store(true)
	}
	return n, err
}

func (c *trackedConn) isClosed() bool {
	return c.closed.Load()
}

// 限制单个 tcp 连接
func newSingleConnDialer(dialFunc func(ctx context.Context, network, addr string) (net.Conn, error), addr string) func(ctx context.Context, network string) (net.Conn, error) {
	var (
		mu   sync.Mutex
		conn *trackedConn
	)
	return func(ctx context.Context, network string) (net.Conn, error) {
		mu.Lock()
		defer mu.Unlock()
		if conn != nil && !conn.isClosed() {
			// 如果闭包上已经有连接，就使用已有的连接
			return conn, nil
		}
		c, err := dialFunc(ctx, network, addr)
		if err != nil {
			return nil, err
		}
		conn = &trackedConn{Conn: c}
		return conn, nil
	}
}

// 设置 tcp 的发送和接收 buffer
func configureTCPBuffers(conn net.Conn, sendBytes, recvBytes int) error {
	tcpConn, ok := conn.(*net.TCPConn)
	if !ok {
		return nil
	}
	if sendBytes > 0 {
		if err := tcpConn.SetWriteBuffer(sendBytes); err != nil {
			return fmt.Errorf("set TCP send buffer: %w", err)
		}
	}
	if recvBytes > 0 {
		if err := tcpConn.SetReadBuffer(recvBytes); err != nil {
			return fmt.Errorf("set TCP recv buffer: %w", err)
		}
	}
	return nil
}

// 构造一个 http client 对象
func newHTTPClient(cfg *runConfig) (*http.Client, error) {
	baseDialer := &net.Dialer{}
	dialFunc := func(ctx context.Context, network, addr string) (net.Conn, error) {
		conn, err := baseDialer.DialContext(ctx, network, addr)
		if err != nil {
			return nil, err
		}
		dailCount.Add(1)
		// 设置 socket 缓冲区
		if err := configureTCPBuffers(conn, cfg.sendBufferBytes, cfg.recvBufferBytes); err != nil {
			_ = conn.Close()
			return nil, err
		}
		return conn, nil
	}

	// 不考虑 https 的情况
	// var dialFunc func(ctx context.Context, network, addr string) (net.Conn, error)
	// if strings.EqualFold(cfg.parsedURL.Scheme, "https") {
	// 	tlsCfg := &tls.Config{
	// 		ServerName: cfg.parsedURL.Hostname(),
	// 		NextProtos: []string{"h2", "http/1.1"},
	// 	}
	// 	dialFunc = func(ctx context.Context, network, addr string) (net.Conn, error) {
	// 		rawConn, err := rawDialFunc(ctx, network, addr)
	// 		if err != nil {
	// 			return nil, err
	// 		}
	// 		tlsConn := tls.Client(rawConn, tlsCfg)
	// 		if err := tlsConn.HandshakeContext(ctx); err != nil {
	// 			_ = rawConn.Close()
	// 			return nil, err
	// 		}
	// 		return tlsConn, nil
	// 	}
	// } else {
	// 	dialFunc = rawDialFunc
	// }

	// http1Transport := &http.Transport{
	// 	HTTP2: &http.HTTP2Config{
	// 		MaxConcurrentStreams:          100,
	// 		MaxReceiveBufferPerConnection: cfg.recvBufferBytes,
	// 		MaxReceiveBufferPerStream:     cfg.recvBufferBytes,
	// 	},
	// }
	// // 得到一个 transport 对象
	// tr, err := http2.ConfigureTransports(http1Transport)
	// if err != nil {
	// 	return nil, fmt.Errorf("configure http2 transport: %w", err)
	// }
	// tr.AllowHTTP = true
	// // StrictMaxConcurrentStreams controls whether the server's
	// // SETTINGS_MAX_CONCURRENT_STREAMS should be respected
	// // globally. If false, new TCP connections are created to the
	// // server as needed to keep each under the per-connection
	// // SETTINGS_MAX_CONCURRENT_STREAMS limit. If true, the
	// // server's SETTINGS_MAX_CONCURRENT_STREAMS is interpreted as
	// // a global limit and callers of RoundTrip block when needed,
	// // waiting for their turn.
	// //tr.StrictMaxConcurrentStreams = cfg.strictMaxStreams
	// tr.StrictMaxConcurrentStreams = true // 当达到 SETTINGS_MAX_CONCURRENT_STREAMS 是，进行阻塞
	tr := &http2.Transport{
		AllowHTTP:                  true,
		StrictMaxConcurrentStreams: true,
	}

	if cfg.singleConnection {
		dialOnce := newSingleConnDialer(dialFunc /*传入 dail 函数*/, cfg.addr)
		tr.DialTLSContext = func(ctx context.Context, network, _ string, _ *tls.Config) (net.Conn, error) {
			return dialOnce(ctx, network)
		}
	} else {
		dialer := &net.Dialer{}
		tr.DialTLSContext = func(ctx context.Context, network, addr string, _ *tls.Config) (net.Conn, error) {
			dailCount.Add(1)
			return dialer.DialContext(ctx, network, addr)
		}
	}

	return &http.Client{
		Transport: tr,
		Timeout:   10 * time.Second,
	}, nil
}

func buildSeqURL(prefix string, seq int64, buf []byte) (string, []byte /*预防扩容的情况*/) {
	buf = buf[:0]
	buf = append(buf, prefix...)
	buf = strconv.AppendInt(buf, seq, 10)
	//next := buf[len(buf):]
	// 按照提示词意图：这里使用 unsafe 做 []byte -> string 的无拷贝转换，减少一次分配。
	return unsafe.String(unsafe.SliceData(buf), len(buf)), buf
}

func runWorker(ctx context.Context, client *http.Client, prefix string, seq *atomic.Int64, ws *workerStats, checkOutput bool) {
	urlBuf := make([]byte, 0, len(prefix)+20)
	var tokenBuf [64]byte
	var currentSeq int64
	for {
		if ctx.Err() != nil {
			return
		}
		//currentSeq := seq.Add(1)
		currentSeq++
		var urlStr string
		urlStr, urlBuf = buildSeqURL(prefix, currentSeq, urlBuf)
		req, err := http.NewRequestWithContext(ctx, http.MethodGet, urlStr, nil)
		if err != nil {
			log.Println(err.Error())
			ws.errors++
			continue
		}
		start := time.Now()
		resp, err := client.Do(req)
		if err != nil {
			if ctx.Err() != nil {
				return
			}
			if errors.Is(err, http2.ErrNoCachedConn) {
				// 忽略掉因为不能建立新连接而报的错误
				ws.errors++
				continue
			}
			log.Println(err.Error())
			ws.errors++
			continue
		}
		if checkOutput {
			body, readErr := io.ReadAll(resp.Body)
			_ = resp.Body.Close()
			if readErr != nil {
				if ctx.Err() != nil {
					return
				}
				log.Println(readErr.Error())
				ws.errors++
				continue
			}

			latency := time.Since(start)
			ws.total++
			ws.codeCounts[resp.StatusCode]++
			ws.addLatency(latency)

			seqToken := tokenBuf[:0]
			seqToken = append(seqToken, "seq="...)
			seqToken = strconv.AppendInt(seqToken, currentSeq, 10)
			if !bytes.Contains(body, seqToken) {
				panic(fmt.Sprintf("response missing %s", seqToken))
			}
			continue
		}

		_, readErr := io.Copy(io.Discard, resp.Body)
		_ = resp.Body.Close()
		if readErr != nil {
			if ctx.Err() != nil {
				return
			}
			log.Println(readErr.Error())
			ws.errors++
			continue
		}

		latency := time.Since(start)
		ws.total++
		ws.codeCounts[resp.StatusCode]++
		ws.addLatency(latency)
	}
}

type benchResult struct {
	elapsed time.Duration
	stats   *workerStats
}

func runBenchmark(ctx context.Context, clients []*http.Client, prefix string, perConn int, checkOutput bool) benchResult {
	var seq atomic.Int64
	totalWorkers := len(clients) * perConn
	stats := make([]*workerStats, totalWorkers)
	startTime := time.Now()

	var wg sync.WaitGroup
	idx := 0
	for _, client := range clients {
		for i := 0; i < perConn; i++ {
			ws := newWorkerStats()
			stats[idx] = ws
			idx++
			wg.Add(1)
			go func(c *http.Client, s *workerStats) {
				defer wg.Done()
				runWorker(ctx, c, prefix, &seq, s, checkOutput)
			}(client, ws)
		}
	}
	wg.Wait()

	return benchResult{
		elapsed: time.Since(startTime),
		stats:   aggregateStats(stats),
	}
}

func aggregateStats(stats []*workerStats) *workerStats {
	aggregate := &workerStats{
		codeCounts:     make(map[int]int64),
		latencyBuckets: make([]int64, 0),
	}
	for _, ws := range stats {
		aggregate.total += ws.total
		aggregate.errors += ws.errors
		for code, count := range ws.codeCounts {
			aggregate.codeCounts[code] += count
		}
		if len(ws.latencyBuckets) > len(aggregate.latencyBuckets) {
			aggregate.latencyBuckets = append(aggregate.latencyBuckets, make([]int64, len(ws.latencyBuckets)-len(aggregate.latencyBuckets))...)
		}
		for i, v := range ws.latencyBuckets {
			aggregate.latencyBuckets[i] += v
		}
	}
	return aggregate
}

func printReport(elapsed time.Duration, stats *workerStats) {
	fmt.Printf("Duration: %s\n", elapsed.Truncate(time.Millisecond))
	fmt.Printf("Total requests: %d\n", stats.total)
	fmt.Printf("Errors: %d\n", stats.errors)
	fmt.Printf("Dail count: %d\n", dailCount.Load())

	if len(stats.codeCounts) > 0 {
		fmt.Println("Status QPS:")
		codes := make([]int, 0, len(stats.codeCounts))
		for code := range stats.codeCounts {
			codes = append(codes, code)
		}
		sort.Ints(codes)
		for _, code := range codes {
			count := stats.codeCounts[code]
			qps := float64(count) / elapsed.Seconds()
			fmt.Printf("  %d: %.2f\n", code, qps)
		}
	}

	var totalLat int64
	for _, v := range stats.latencyBuckets {
		totalLat += v
	}
	if totalLat > 0 {
		fmt.Println("Latency distribution (power-of-two, from 10us):")
		var cumulative int64
		for i, count := range stats.latencyBuckets {
			if count == 0 {
				continue
			}
			cumulative += count
			upper := latencyBase << i
			percent := float64(count) / float64(totalLat) * 100
			cumPercent := float64(cumulative) / float64(totalLat) * 100
			fmt.Printf("  <= %s: %.2f%% (cum %.2f%%) (%d)\n", upper, percent, cumPercent, count)
		}
	}
}

// 构造 http client
func buildClients(cfg *runConfig) ([]*http.Client, error) {
	clients := make([]*http.Client, cfg.connectionCount)
	for i := 0; i < cfg.connectionCount; i++ {
		client, err := newHTTPClient(cfg)
		if err != nil {
			return nil, err
		}
		clients[i] = client
	}
	return clients, nil
}

func main() {
	log.SetFlags(log.LstdFlags | log.Lshortfile)
	cfg, err := parseConfig()
	if err != nil {
		log.Fatalf("%v", err)
	}
	clients, err := buildClients(cfg)
	if err != nil {
		log.Fatalf("failed to build clients: %v", err)
	}

	ctx, cancel := context.WithTimeout(context.Background(), cfg.duration)
	defer cancel()

	result := runBenchmark(ctx, clients, cfg.seqPrefix, cfg.coroutinesPerConn, cfg.checkOutput)
	printReport(result.elapsed, result.stats)
}
