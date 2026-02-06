BINARY ?= http2-bench-client
BIN_DIR ?= bin

.PHONY: build clean

build:
	mkdir -p $(BIN_DIR)
	go build -o $(BIN_DIR)/$(BINARY) .

run:
	$(BIN_DIR)/$(BINARY) \
	    -thread.count=8 \
		-coroutine.count=80 \
		-connection.count=16 \
		-max.concurrent.streams=80 \
		-target.url=http://localhost:9081/echo \
		-duration.seconds=10

clean:
	rm -rf $(BIN_DIR)
