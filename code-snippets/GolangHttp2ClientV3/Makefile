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
		-max.concurrent.streams=87 \
		-target.url=http://localhost:9081/echo \
		-duration.seconds=20 \
		-check.output=false \
		-show.response=false


clean:
	rm -rf $(BIN_DIR)
