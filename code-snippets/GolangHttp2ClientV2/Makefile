BINARY ?= http2-bench-client
BIN_DIR ?= bin

.PHONY: build clean

build:
	mkdir -p $(BIN_DIR)
	go build -o $(BIN_DIR)/$(BINARY) .

run:
	$(BIN_DIR)/$(BINARY) \
	    -thread.count=4 \
		-connection.count=8 \
		-coroutine.count.per.connection=40 \
		-target.url=http://localhost:9081/echo \
		-duration.seconds=20 \
		-check.output=false \
		-single.connection=false \
		-send.buffer.bytes=0 \
		-recv.buffer.bytes=0

clean:
	rm -rf $(BIN_DIR)
