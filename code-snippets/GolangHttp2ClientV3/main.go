package main

import (
	"bytes"
	"context"
	"flag"
	"fmt"
	"log"
	"math/bits"
	"net"
	"net/url"
	"runtime"
	"sort"
	"strconv"
	"strings"
	"sync"
	"sync/atomic"
	"time"

	"golang.org/x/net/http2"
	"golang.org/x/net/http2/hpack"
)

const (
	latencyBase           = 10 * time.Microsecond
	preface               = "PRI * HTTP/2.0\r\n\r\nSM\r\n\r\n"
	defaultWindowSize     = 65535
	initialWindowSize     = 1 << 20
	windowUpdateThreshold = 32 << 10
	spinSleep             = 100 * time.Microsecond
	maxPrintResponses     = 10
	maxPrintBody          = 4096
)

type runConfig struct {
	threadCount          int
	connectionCount      int
	duration             time.Duration
	targetURL            string
	parsedURL            *url.URL
	addr                 string
	scheme               string
	authority            string
	seqPathPrefix        string
	checkOutput          bool
	showResponse         bool
	maxConcurrentStreams int
	printBudget          atomic.Int64
}

type workerStats struct {
	codeCounts     map[int]int64
	latencyBuckets []int64
	total          int64
	errors         int64
}

type benchResult struct {
	elapsed time.Duration
	stats   *workerStats
}

type streamState struct {
	seq           int64
	start         time.Time
	status        int
	found         bool
	capture       bool
	bodyLen       int
	bodyTruncated bool
	windowUsed    int
	tokenLen      int
	tailLen       int
	tokenBuf      [64]byte
	tailBuf       [64]byte
	bodyBuf       [maxPrintBody]byte
}

type connWorker struct {
	id             int
	cfg            *runConfig
	conn           net.Conn
	fr             *http2.Framer
	enc            *hpack.Encoder
	dec            *hpack.Decoder
	encBuf         bytes.Buffer
	headerBuf      bytes.Buffer
	stats          *workerStats
	streams        map[uint32]*streamState
	mu             sync.Mutex
	writeMu        sync.Mutex
	outstanding    atomic.Int64
	serverMax      atomic.Int64
	errCount       atomic.Int64
	gotGoAway      atomic.Bool
	nextStreamID   uint32
	connWindowUsed int
}

var streamPool = sync.Pool{New: func() any { return &streamState{} }}
var printMu sync.Mutex

func main() {
	log.SetFlags(log.LstdFlags | log.Lshortfile)
	cfg, err := parseConfig()
	if err != nil {
		log.Fatalf("%v", err)
	}
	workers, err := buildWorkers(cfg)
	if err != nil {
		log.Fatalf("failed to build workers: %v", err)
	}

	ctx, cancel := context.WithTimeout(context.Background(), cfg.duration)
	defer cancel()

	result := runBenchmark(ctx, workers)
	printReport(result.elapsed, result.stats)
}

func parseConfig() (*runConfig, error) {
	threadCount := flag.Int("thread.count", 0, "set GOMAXPROCS")
	connectionCount := flag.Int("connection.count", 0, "number of TCP connections")
	targetURL := flag.String("target.url", "", "target URL")
	durationSeconds := flag.Int("duration.seconds", 10, "benchmark duration in seconds")
	checkOutput := flag.Bool("check.output", true, "check response body contains seq")
	showResponse := flag.Bool("show.response", false, "print first 10 responses")
	maxStreams := flag.Int("max.concurrent.streams", 0, "max concurrent streams per connection")
	flag.Parse()

	if *threadCount > 0 {
		runtime.GOMAXPROCS(*threadCount)
	}
	gmp := runtime.GOMAXPROCS(0)
	connCount := *connectionCount
	if connCount <= 0 {
		connCount = gmp * 2
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
	if !strings.EqualFold(parsedURL.Scheme, "http") {
		return nil, fmt.Errorf("only http (h2c prior-knowledge) is supported")
	}

	max := *maxStreams
	if max <= 0 {
		max = 80
	}

	cfg := &runConfig{
		threadCount:          *threadCount,
		connectionCount:      connCount,
		duration:             time.Duration(*durationSeconds) * time.Second,
		targetURL:            *targetURL,
		parsedURL:            parsedURL,
		addr:                 addrFromURL(parsedURL),
		scheme:               parsedURL.Scheme,
		authority:            parsedURL.Host,
		seqPathPrefix:        buildSeqPathPrefix(parsedURL),
		checkOutput:          *checkOutput,
		showResponse:         *showResponse,
		maxConcurrentStreams: max,
	}
	if cfg.showResponse {
		cfg.printBudget.Store(maxPrintResponses)
	}
	return cfg, nil
}

func addrFromURL(u *url.URL) string {
	host := u.Hostname()
	port := u.Port()
	if port == "" {
		port = "80"
	}
	return net.JoinHostPort(host, port)
}

func buildSeqPathPrefix(u *url.URL) string {
	path := u.EscapedPath()
	if path == "" {
		path = "/"
	}
	if u.RawQuery == "" {
		return path + "?seq="
	}
	if strings.HasSuffix(u.RawQuery, "&") || strings.HasSuffix(u.RawQuery, "=") {
		return path + "?" + u.RawQuery + "seq="
	}
	return path + "?" + u.RawQuery + "&seq="
}

func (cfg *runConfig) reservePrintSlot() bool {
	for {
		cur := cfg.printBudget.Load()
		if cur <= 0 {
			return false
		}
		if cfg.printBudget.CompareAndSwap(cur, cur-1) {
			return true
		}
	}
}

func (cfg *runConfig) releasePrintSlot() {
	cfg.printBudget.Add(1)
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

func buildWorkers(cfg *runConfig) ([]*connWorker, error) {
	workers := make([]*connWorker, 0, cfg.connectionCount)
	for i := 0; i < cfg.connectionCount; i++ {
		w, err := newConnWorker(i, cfg)
		if err != nil {
			for _, cw := range workers {
				_ = cw.conn.Close()
			}
			return nil, err
		}
		workers = append(workers, w)
	}
	return workers, nil
}

func newConnWorker(id int, cfg *runConfig) (*connWorker, error) {
	conn, err := (&net.Dialer{}).Dial("tcp", cfg.addr)
	if err != nil {
		return nil, err
	}

	cw := &connWorker{
		id:           id,
		cfg:          cfg,
		conn:         conn,
		fr:           http2.NewFramer(conn, conn),
		stats:        newWorkerStats(),
		streams:      make(map[uint32]*streamState, cfg.maxConcurrentStreams*2),
		nextStreamID: 1,
	}
	cw.enc = hpack.NewEncoder(&cw.encBuf)
	cw.dec = hpack.NewDecoder(4096, nil)

	if err := writeAll(conn, []byte(preface)); err != nil {
		_ = conn.Close()
		return nil, err
	}
	if err := cw.writeSettings(); err != nil {
		_ = conn.Close()
		return nil, err
	}
	if initialWindowSize > defaultWindowSize {
		if err := cw.writeWindowUpdate(0, initialWindowSize-defaultWindowSize); err != nil {
			_ = conn.Close()
			return nil, err
		}
	}
	return cw, nil
}

func writeAll(w net.Conn, b []byte) error {
	for len(b) > 0 {
		n, err := w.Write(b)
		if err != nil {
			return err
		}
		b = b[n:]
	}
	return nil
}

func (cw *connWorker) writeSettings() error {
	cw.writeMu.Lock()
	defer cw.writeMu.Unlock()
	return cw.fr.WriteSettings(
		http2.Setting{ID: http2.SettingEnablePush, Val: 0},
		http2.Setting{ID: http2.SettingInitialWindowSize, Val: uint32(initialWindowSize)},
	)
}

func (cw *connWorker) ackSettings() error {
	cw.writeMu.Lock()
	defer cw.writeMu.Unlock()
	return cw.fr.WriteSettingsAck()
}

func (cw *connWorker) writeWindowUpdate(streamID uint32, incr int) error {
	if incr <= 0 {
		return nil
	}
	cw.writeMu.Lock()
	defer cw.writeMu.Unlock()
	return cw.fr.WriteWindowUpdate(streamID, uint32(incr))
}

func runBenchmark(ctx context.Context, workers []*connWorker) benchResult {
	start := time.Now()
	var wg sync.WaitGroup
	for _, w := range workers {
		wg.Add(2)
		go w.sendLoop(ctx, &wg)
		go w.recvLoop(ctx, &wg)
	}
	wg.Wait()

	stats := aggregateWorkerStats(workers)
	return benchResult{elapsed: time.Since(start), stats: stats}
}

func (cw *connWorker) sendLoop(ctx context.Context, wg *sync.WaitGroup) {
	defer wg.Done()
	defer func() { _ = cw.conn.Close() }()

	var seq int64
	pathBuf := make([]byte, 0, len(cw.cfg.seqPathPrefix)+20)
	for {
		if ctx.Err() != nil || cw.gotGoAway.Load() {
			return
		}
		limit := cw.currentLimit()
		for limit > 0 && cw.outstanding.Load() >= limit {
			runtime.Gosched()
			time.Sleep(spinSleep)
			if ctx.Err() != nil || cw.gotGoAway.Load() {
				return
			}
			limit = cw.currentLimit()
		}

		seq++
		pathStr, nextBuf := buildSeqPath(cw.cfg.seqPathPrefix, seq, pathBuf)
		pathBuf = nextBuf

		streamID := cw.nextStreamID
		cw.nextStreamID += 2
		state := streamPool.Get().(*streamState)
		capture := false
		if cw.cfg.showResponse {
			capture = cw.cfg.reservePrintSlot()
		}
		state.reset(seq, time.Now(), cw.cfg.checkOutput, capture)
		cw.addStream(streamID, state)

		if err := cw.writeHeaders(streamID, pathStr); err != nil {
			cw.recordError(err)
			if capture {
				cw.cfg.releasePrintSlot()
			}
			cw.removeStream(streamID)
			streamPool.Put(state)
			continue
		}
		cw.outstanding.Add(1)
	}
}

func (cw *connWorker) recvLoop(ctx context.Context, wg *sync.WaitGroup) {
	defer wg.Done()
	defer func() { _ = cw.conn.Close() }()

	var drainDeadline time.Time
	for {
		if ctx.Err() != nil && drainDeadline.IsZero() {
			drainDeadline = time.Now().Add(2 * time.Second)
		}
		if !drainDeadline.IsZero() {
			if cw.outstanding.Load() == 0 || time.Now().After(drainDeadline) {
				return
			}
		}

		_ = cw.conn.SetReadDeadline(time.Now().Add(500 * time.Millisecond))
		f, err := cw.fr.ReadFrame()
		if err != nil {
			if ne, ok := err.(net.Error); ok && ne.Timeout() {
				continue
			}
			if ctx.Err() != nil {
				return
			}
			cw.recordError(err)
			return
		}
		cw.handleFrame(f)
	}
}

func (cw *connWorker) currentLimit() int64 {
	max := int64(cw.cfg.maxConcurrentStreams)
	server := cw.serverMax.Load()
	if server > 0 && server < max {
		return server
	}
	return max
}

func (cw *connWorker) buildHeaderBlock(path string) []byte {
	cw.encBuf.Reset()
	_ = cw.enc.WriteField(hpack.HeaderField{Name: ":method", Value: "GET"})
	_ = cw.enc.WriteField(hpack.HeaderField{Name: ":scheme", Value: cw.cfg.scheme})
	_ = cw.enc.WriteField(hpack.HeaderField{Name: ":authority", Value: cw.cfg.authority})
	_ = cw.enc.WriteField(hpack.HeaderField{Name: ":path", Value: path})
	return cw.encBuf.Bytes()
}

func (cw *connWorker) writeHeaders(streamID uint32, path string) error {
	block := cw.buildHeaderBlock(path)
	cw.writeMu.Lock()
	defer cw.writeMu.Unlock()
	return cw.fr.WriteHeaders(http2.HeadersFrameParam{
		StreamID:      streamID,
		BlockFragment: block,
		EndHeaders:    true,
		EndStream:     true,
	})
}

func (cw *connWorker) handleFrame(f http2.Frame) {
	switch ff := f.(type) {
	case *http2.SettingsFrame:
		cw.handleSettings(ff)
	case *http2.HeadersFrame:
		cw.handleHeaders(ff)
	case *http2.ContinuationFrame:
		// should be handled via collectHeaderBlock; ignore stray continuation
	case *http2.DataFrame:
		cw.handleData(ff)
	case *http2.PingFrame:
		cw.handlePing(ff)
	case *http2.RSTStreamFrame:
		cw.handleRST(ff)
	case *http2.GoAwayFrame:
		cw.handleGoAway(ff)
	case *http2.WindowUpdateFrame:
		// ignore
	default:
		// ignore other frames
	}
}

func (cw *connWorker) handleSettings(f *http2.SettingsFrame) {
	if f.IsAck() {
		return
	}
	_ = f.ForeachSetting(func(s http2.Setting) error {
		if s.ID == http2.SettingMaxConcurrentStreams {
			cw.serverMax.Store(int64(s.Val))
		}
		return nil
	})
	if err := cw.ackSettings(); err != nil {
		cw.recordError(err)
	}
}

func (cw *connWorker) handlePing(f *http2.PingFrame) {
	if f.IsAck() {
		return
	}
	cw.writeMu.Lock()
	defer cw.writeMu.Unlock()
	if err := cw.fr.WritePing(true, f.Data); err != nil {
		cw.recordError(err)
	}
}

func (cw *connWorker) handleGoAway(f *http2.GoAwayFrame) {
	cw.gotGoAway.Store(true)
	cw.recordError(fmt.Errorf("goaway: %v", f.ErrCode))
}

func (cw *connWorker) handleRST(f *http2.RSTStreamFrame) {
	if st := cw.removeStream(f.StreamID); st != nil {
		streamPool.Put(st)
		cw.outstanding.Add(-1)
	}
	cw.recordError(fmt.Errorf("rst_stream: %v", f.ErrCode))
}

func (cw *connWorker) handleHeaders(f *http2.HeadersFrame) {
	block, err := cw.collectHeaderBlock(f)
	if err != nil {
		cw.recordError(err)
		return
	}
	status, err := cw.decodeStatus(block)
	if err != nil {
		cw.recordError(err)
		return
	}
	st := cw.getStream(f.StreamID)
	if st != nil && status != 0 {
		st.status = status
	}
	if f.StreamEnded() {
		cw.finishStream(f.StreamID)
	}
}

func (cw *connWorker) handleData(f *http2.DataFrame) {
	st := cw.getStream(f.StreamID)
	if st == nil {
		return
	}
	if len(f.Data()) > 0 {
		st.scanData(f.Data())
		cw.consumeWindow(f.StreamID, len(f.Data()), st)
	}
	if f.StreamEnded() {
		cw.finishStream(f.StreamID)
	}
}

func (cw *connWorker) collectHeaderBlock(f *http2.HeadersFrame) ([]byte, error) {
	if f.HeadersEnded() {
		return f.HeaderBlockFragment(), nil
	}
	cw.headerBuf.Reset()
	_, _ = cw.headerBuf.Write(f.HeaderBlockFragment())
	for {
		_ = cw.conn.SetReadDeadline(time.Now().Add(500 * time.Millisecond))
		nf, err := cw.fr.ReadFrame()
		if err != nil {
			return nil, err
		}
		cont, ok := nf.(*http2.ContinuationFrame)
		if !ok {
			return cw.headerBuf.Bytes(), nil
		}
		if cont.StreamID != f.StreamID {
			return cw.headerBuf.Bytes(), nil
		}
		_, _ = cw.headerBuf.Write(cont.HeaderBlockFragment())
		if cont.HeadersEnded() {
			return cw.headerBuf.Bytes(), nil
		}
	}
}

func (cw *connWorker) decodeStatus(block []byte) (int, error) {
	var status int
	cw.dec.SetEmitFunc(func(f hpack.HeaderField) {
		if f.Name == ":status" {
			if code, err := strconv.Atoi(f.Value); err == nil {
				status = code
			}
		}
	})
	_, err := cw.dec.Write(block)
	return status, err
}

func (cw *connWorker) addStream(streamID uint32, st *streamState) {
	cw.mu.Lock()
	cw.streams[streamID] = st
	cw.mu.Unlock()
}

func (cw *connWorker) getStream(streamID uint32) *streamState {
	cw.mu.Lock()
	st := cw.streams[streamID]
	cw.mu.Unlock()
	return st
}

func (cw *connWorker) removeStream(streamID uint32) *streamState {
	cw.mu.Lock()
	st := cw.streams[streamID]
	delete(cw.streams, streamID)
	cw.mu.Unlock()
	return st
}

func (cw *connWorker) finishStream(streamID uint32) {
	st := cw.removeStream(streamID)
	if st == nil {
		return
	}
	cw.printResponse(st)
	if cw.cfg.checkOutput && !st.found {
		panic(fmt.Sprintf("response missing seq=%d", st.seq))
	}
	latency := time.Since(st.start)
	cw.stats.total++
	if st.status != 0 {
		cw.stats.codeCounts[st.status]++
	}
	cw.stats.addLatency(latency)
	cw.outstanding.Add(-1)
	streamPool.Put(st)
}

func (cw *connWorker) recordError(err error) {
	cw.errCount.Add(1)
	log.Println(err.Error())
}

func (cw *connWorker) consumeWindow(streamID uint32, size int, st *streamState) {
	if size <= 0 {
		return
	}
	cw.connWindowUsed += size
	if cw.connWindowUsed >= windowUpdateThreshold {
		if err := cw.writeWindowUpdate(0, cw.connWindowUsed); err != nil {
			cw.recordError(err)
		}
		cw.connWindowUsed = 0
	}
	if st == nil {
		return
	}
	st.windowUsed += size
	if st.windowUsed >= windowUpdateThreshold {
		if err := cw.writeWindowUpdate(streamID, st.windowUsed); err != nil {
			cw.recordError(err)
		}
		st.windowUsed = 0
	}
}

func (cw *connWorker) printResponse(st *streamState) {
	if !st.capture {
		return
	}
	printMu.Lock()
	defer printMu.Unlock()
	fmt.Printf("Response seq=%d status=%d len=%d\n", st.seq, st.status, st.bodyLen)
	if st.bodyLen == 0 {
		fmt.Println("<empty body>")
	} else {
		fmt.Printf("%s\n", st.bodyBuf[:st.bodyLen])
	}
	if st.bodyTruncated {
		fmt.Println("[truncated]")
	}
	fmt.Println("----")
}

func (s *streamState) reset(seq int64, start time.Time, check bool, capture bool) {
	s.seq = seq
	s.start = start
	s.status = 0
	s.found = !check
	s.capture = capture
	s.bodyLen = 0
	s.bodyTruncated = false
	s.windowUsed = 0
	s.tokenLen = 0
	s.tailLen = 0
	if check {
		b := s.tokenBuf[:0]
		b = append(b, "seq="...)
		b = strconv.AppendInt(b, seq, 10)
		s.tokenLen = len(b)
		copy(s.tokenBuf[:], b)
	}
}

func (s *streamState) scanData(data []byte) {
	s.appendBody(data)
	if s.found || s.tokenLen == 0 {
		s.updateTail(data)
		return
	}
	token := s.tokenBuf[:s.tokenLen]
	if bytes.Contains(data, token) {
		s.found = true
		s.updateTail(data)
		return
	}
	if s.tailLen > 0 && boundaryMatch(s.tailBuf[:s.tailLen], data, token) {
		s.found = true
	}
	s.updateTail(data)
}

func (s *streamState) appendBody(data []byte) {
	if !s.capture || len(data) == 0 {
		return
	}
	if s.bodyLen >= maxPrintBody {
		s.bodyTruncated = true
		return
	}
	space := maxPrintBody - s.bodyLen
	if len(data) > space {
		data = data[:space]
		s.bodyTruncated = true
	}
	copy(s.bodyBuf[s.bodyLen:], data)
	s.bodyLen += len(data)
}

func boundaryMatch(tail, data, token []byte) bool {
	maxOverlap := len(tail)
	if maxOverlap > len(token)-1 {
		maxOverlap = len(token) - 1
	}
	if maxOverlap > len(data) {
		maxOverlap = len(data)
	}
	for i := 1; i <= maxOverlap; i++ {
		if !bytes.Equal(tail[len(tail)-i:], token[:i]) {
			continue
		}
		need := len(token) - i
		if need > len(data) {
			continue
		}
		if bytes.Equal(data[:need], token[i:]) {
			return true
		}
	}
	return false
}

func (s *streamState) updateTail(data []byte) {
	if s.tokenLen == 0 {
		return
	}
	max := s.tokenLen - 1
	if max <= 0 {
		s.tailLen = 0
		return
	}
	total := s.tailLen + len(data)
	if total <= 0 {
		s.tailLen = 0
		return
	}
	need := max
	if total < need {
		need = total
	}
	if len(data) >= need {
		copy(s.tailBuf[:need], data[len(data)-need:])
		s.tailLen = need
		return
	}
	suffixFromTail := need - len(data)
	if suffixFromTail > s.tailLen {
		suffixFromTail = s.tailLen
	}
	start := s.tailLen - suffixFromTail
	copy(s.tailBuf[:suffixFromTail], s.tailBuf[start:s.tailLen])
	copy(s.tailBuf[suffixFromTail:need], data)
	s.tailLen = need
}

func buildSeqPath(prefix string, seq int64, buf []byte) (string, []byte) {
	buf = buf[:0]
	buf = append(buf, prefix...)
	buf = strconv.AppendInt(buf, seq, 10)
	return string(buf), buf
}

func aggregateWorkerStats(workers []*connWorker) *workerStats {
	aggregate := &workerStats{
		codeCounts:     make(map[int]int64),
		latencyBuckets: make([]int64, 0),
	}
	for _, w := range workers {
		w.stats.errors = w.errCount.Load()
		aggregate.total += w.stats.total
		aggregate.errors += w.stats.errors
		for code, count := range w.stats.codeCounts {
			aggregate.codeCounts[code] += count
		}
		if len(w.stats.latencyBuckets) > len(aggregate.latencyBuckets) {
			aggregate.latencyBuckets = append(aggregate.latencyBuckets, make([]int64, len(w.stats.latencyBuckets)-len(aggregate.latencyBuckets))...)
		}
		for i, v := range w.stats.latencyBuckets {
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
