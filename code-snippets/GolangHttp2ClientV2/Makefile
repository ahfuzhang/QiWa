BINARY ?= http2-bench-client
BIN_DIR ?= bin

.PHONY: build clean

build:
	mkdir -p $(BIN_DIR)
	go build -o $(BIN_DIR)/$(BINARY) .

run:
	$(BIN_DIR)/$(BINARY) \
	    -thread.count=8 \
		-connection.count=16 \
		-coroutine.count.per.connection=40 \
		-target.url=http://localhost:9081/echo \
		-duration.seconds=10 \
		-check.output=false

clean:
	rm -rf $(BIN_DIR)
