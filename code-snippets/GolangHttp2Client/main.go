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
	threadCount     int
	coroutineCount  int
	connectionCount int
	maxStreams      int
	duration        time.Duration
	targetURL       string
	parsedURL       *url.URL
}

type h2Client struct {
	conn    *http2.ClientConn
	sem     chan struct{}
	batchMu sync.Mutex
}

type connPool struct {
	clients []*h2Client
	idx     uint64
}

func newConnPool(u *url.URL, count int, maxStreams int) (*connPool, error) {
	tr := &http2.Transport{AllowHTTP: true}
	clients := make([]*h2Client, 0, count)
	for i := 0; i < count; i++ {
		nc, err := dialConn(u)
		if err != nil {
			return nil, err
		}
		cc, err := tr.NewClientConn(nc)
		if err != nil {
			_ = nc.Close()
			return nil, err
		}
		clients = append(clients, &h2Client{
			conn: cc,
			sem:  make(chan struct{}, maxStreams),
		})
	}
	return &connPool{clients: clients}, nil
}

func (p *connPool) acquire() *h2Client {
	if len(p.clients) == 1 {
		c := p.clients[0]
		c.sem <- struct{}{}
		return c
	}
	start := int(atomic.AddUint64(&p.idx, 1))
	n := len(p.clients)
	for i := 0; i < n; i++ {
		c := p.clients[(start+i)%n]
		select {
		case c.sem <- struct{}{}:
			return c
		default:
		}
	}
	c := p.clients[start%n]
	c.sem <- struct{}{}
	return c
}

func (p *connPool) nextClient() *h2Client {
	if len(p.clients) == 1 {
		return p.clients[0]
	}
	idx := int(atomic.AddUint64(&p.idx, 1))
	return p.clients[idx%len(p.clients)]
}

func (p *connPool) acquireBatch(n int) *h2Client {
	c := p.nextClient()
	c.batchMu.Lock()
	for i := 0; i < n; i++ {
		c.sem <- struct{}{}
	}
	return c
}

func (p *connPool) closeAll() {
	for _, c := range p.clients {
		_ = c.conn.Close()
	}
}

type releaseBody struct {
	io.ReadCloser
	release func()
	once    sync.Once
}

func (b *releaseBody) Close() error {
	err := b.ReadCloser.Close()
	b.once.Do(b.release)
	return err
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

func dialConn(u *url.URL) (net.Conn, error) {
	host := u.Hostname()
	port := u.Port()
	if port == "" {
		if strings.EqualFold(u.Scheme, "https") {
			port = "443"
		} else {
			port = "80"
		}
	}
	addr := net.JoinHostPort(host, port)
	if strings.EqualFold(u.Scheme, "https") {
		cfg := &tls.Config{
			ServerName: host,
			NextProtos: []string{"h2", "http/1.1"},
		}
		return tls.Dial("tcp", addr, cfg)
	}
	return net.Dial("tcp", addr)
}

func parseConfig() (*runConfig, error) {
	threadCount := flag.Int("thread.count", 0, "set GOMAXPROCS")
	coroutineCount := flag.Int("coroutine.count", 0, "number of goroutines")
	connectionCount := flag.Int("connection.count", 0, "number of TCP connections")
	maxStreams := flag.Int("max.concurrent.streams", 80, "max streams per connection")
	targetURL := flag.String("target.url", "", "target URL")
	durationSeconds := flag.Int("duration.seconds", 10, "benchmark duration in seconds")
	flag.Parse()

	if *threadCount > 0 {
		runtime.GOMAXPROCS(*threadCount)
	}
	gmp := runtime.GOMAXPROCS(0)
	coro := *coroutineCount
	if coro <= 0 {
		coro = gmp * 10
	}
	connCount := *connectionCount
	if connCount <= 0 {
		connCount = gmp * 2
	}
	streams := *maxStreams
	if streams <= 0 {
		streams = 80
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

	return &runConfig{
		threadCount:     *threadCount,
		coroutineCount:  coro,
		connectionCount: connCount,
		maxStreams:      streams,
		duration:        time.Duration(*durationSeconds) * time.Second,
		targetURL:       *targetURL,
		parsedURL:       parsedURL,
	}, nil
}

func buildSeqURL(base *url.URL, seq int64) string {
	reqURL := *base
	q := reqURL.Query()
	q.Set("seq", strconv.FormatInt(seq, 10))
	reqURL.RawQuery = q.Encode()
	return reqURL.String()
}

func runWorker(ctx context.Context, pool *connPool, baseURL *url.URL, seq *atomic.Int64, ws *workerStats) {
	for {
		if ctx.Err() != nil {
			return
		}
		currentSeq := seq.Add(1)
		req, err := http.NewRequestWithContext(ctx, http.MethodGet, buildSeqURL(baseURL, currentSeq), nil)
		if err != nil {
			ws.errors++
			continue
		}
		client := pool.acquire()
		start := time.Now()
		resp, err := client.conn.RoundTrip(req)
		if err != nil {
			<-client.sem
			ws.errors++
			continue
		}
		resp.Body = &releaseBody{
			ReadCloser: resp.Body,
			release: func() {
				<-client.sem
			},
		}
		body, readErr := io.ReadAll(resp.Body)
		_ = resp.Body.Close()
		if readErr != nil {
			ws.errors++
			continue
		}

		latency := time.Since(start)
		ws.total++
		ws.codeCounts[resp.StatusCode]++
		ws.addLatency(latency)

		var tmp [64]byte
		seqToken := tmp[:0]
		seqToken = append(seqToken, "seq="...)
		seqToken = strconv.AppendInt(seqToken, currentSeq, 10)
		//seqToken := []byte("seq=" + strconv.FormatInt(currentSeq, 10))
		if !bytes.Contains(body, seqToken) {
			panic(fmt.Sprintf("response missing %s", seqToken))
		}
	}
}

type reqResult struct {
	status  int
	latency time.Duration
	err     error
}

func doRequest(ctx context.Context, client *h2Client, url string, seq int64) reqResult {
	start := time.Now()
	req, err := http.NewRequestWithContext(ctx, http.MethodGet, url, nil)
	if err != nil {
		return reqResult{err: err}
	}
	resp, err := client.conn.RoundTrip(req)
	if err != nil {
		if resp != nil && resp.Body != nil {
			_ = resp.Body.Close()
		}
		return reqResult{err: err}
	}
	body, readErr := io.ReadAll(resp.Body)
	_ = resp.Body.Close()
	if readErr != nil {
		return reqResult{err: readErr}
	}

	var tmp [64]byte
	seqToken := tmp[:0]
	seqToken = append(seqToken, "seq="...)
	seqToken = strconv.AppendInt(seqToken, seq, 10)
	if !bytes.Contains(body, seqToken) {
		panic(fmt.Sprintf("response missing %s", seqToken))
	}
	return reqResult{
		status:  resp.StatusCode,
		latency: time.Since(start),
	}
}

func batchRunWorker(ctx context.Context, pool *connPool, baseURL *url.URL, seq *atomic.Int64, ws *workerStats, batchSize int) {
	if batchSize <= 0 {
		return
	}
	results := make(chan reqResult, batchSize)
	for {
		if ctx.Err() != nil {
			return
		}
		client := pool.acquireBatch(batchSize)
		if ctx.Err() != nil {
			for i := 0; i < batchSize; i++ {
				<-client.sem
			}
			client.batchMu.Unlock()
			return
		}

		for i := 0; i < batchSize; i++ {
			currentSeq := seq.Add(1)
			go func(s int64) {
				defer func() { <-client.sem }()
				results <- doRequest(ctx, client, buildSeqURL(baseURL, s), s)
			}(currentSeq)
		}

		for i := 0; i < batchSize; i++ {
			res := <-results
			if res.err != nil {
				ws.errors++
				continue
			}
			ws.total++
			ws.codeCounts[res.status]++
			ws.addLatency(res.latency)
		}
		client.batchMu.Unlock()
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

type benchResult struct {
	elapsed time.Duration
	stats   *workerStats
}

func runBenchmark(ctx context.Context, pool *connPool, cfg *runConfig) benchResult {
	var seq atomic.Int64
	stats := make([]*workerStats, cfg.coroutineCount)
	startTime := time.Now()

	var wg sync.WaitGroup
	wg.Add(cfg.coroutineCount)
	for i := 0; i < cfg.coroutineCount; i++ {
		ws := newWorkerStats()
		stats[i] = ws
		go func(ws *workerStats) {
			defer wg.Done()
			if cfg.maxStreams == 1 {
				runWorker(ctx, pool, cfg.parsedURL, &seq, ws)
				return
			}
			batchRunWorker(ctx, pool, cfg.parsedURL, &seq, ws, cfg.maxStreams)
		}(ws)
	}
	wg.Wait()

	return benchResult{
		elapsed: time.Since(startTime),
		stats:   aggregateStats(stats),
	}
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

func main() {
	cfg, err := parseConfig()
	if err != nil {
		log.Fatalf("%v", err)
	}

	pool, err := newConnPool(cfg.parsedURL, cfg.connectionCount, cfg.maxStreams)
	if err != nil {
		log.Fatalf("failed to create connection pool: %v", err)
	}
	defer pool.closeAll()

	ctx, cancel := context.WithTimeout(context.Background(), cfg.duration)
	defer cancel()

	result := runBenchmark(ctx, pool, cfg)
	printReport(result.elapsed, result.stats)
}
