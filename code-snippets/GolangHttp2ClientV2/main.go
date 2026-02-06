package main

import (
	"bytes"
	"context"
	"crypto/tls"
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

	"golang.org/x/net/http2"
)

const latencyBase = 10 * time.Microsecond

type runConfig struct {
	threadCount       int
	connectionCount   int
	coroutinesPerConn int
	duration          time.Duration
	targetURL         string
	parsedURL         *url.URL
	seqPrefix         string
	addr              string
	checkOutput       bool
	singleConnection  bool
	strictMaxStreams  bool
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

func newSingleConnDialer(dialFunc func(ctx context.Context, network, addr string) (net.Conn, error), addr string) func(ctx context.Context, network string) (net.Conn, error) {
	var (
		mu   sync.Mutex
		conn *trackedConn
	)
	return func(ctx context.Context, network string) (net.Conn, error) {
		mu.Lock()
		defer mu.Unlock()
		if conn != nil && !conn.isClosed() {
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

func newHTTPClient(cfg *runConfig) (*http.Client, error) {
	var dialFunc func(ctx context.Context, network, addr string) (net.Conn, error)
	if strings.EqualFold(cfg.parsedURL.Scheme, "https") {
		tlsCfg := &tls.Config{
			ServerName: cfg.parsedURL.Hostname(),
			NextProtos: []string{"h2", "http/1.1"},
		}
		dialer := &tls.Dialer{Config: tlsCfg}
		dialFunc = dialer.DialContext
	} else {
		dialer := &net.Dialer{}
		dialFunc = dialer.DialContext
	}

	tr := &http2.Transport{
		AllowHTTP:                  true,
		StrictMaxConcurrentStreams: cfg.strictMaxStreams,
	}
	if cfg.singleConnection {
		dialOnce := newSingleConnDialer(dialFunc, cfg.addr)
		tr.DialTLSContext = func(ctx context.Context, network, _ string, _ *tls.Config) (net.Conn, error) {
			return dialOnce(ctx, network)
		}
	} else {
		tr.DialTLSContext = func(ctx context.Context, network, addr string, _ *tls.Config) (net.Conn, error) {
			return dialFunc(ctx, network, addr)
		}
	}

	return &http.Client{
		Transport: tr,
		Timeout:   10 * time.Second,
	}, nil
}

func buildSeqURL(prefix string, seq int64, buf []byte) (string, []byte) {
	buf = buf[:0]
	buf = append(buf, prefix...)
	buf = strconv.AppendInt(buf, seq, 10)
	return string(buf), buf
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
		urlStr, nextBuf := buildSeqURL(prefix, currentSeq, urlBuf)
		urlBuf = nextBuf
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
