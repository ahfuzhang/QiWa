ROOT_DIR := $(abspath ../..)
PRJ=DemoLoginServer
BUILD_DIR := $(ROOT_DIR)/build/code-snippets/$(PRJ)

build:
	dotnet build $(PRJ).csproj -r linux-x64 -p:AllowUnsafeBlocks=true -c Release -o $(BUILD_DIR)

run:
	$(BUILD_DIR)/$(PRJ) \
	  -http1.port=8082 \
	  -log.level=info \
	  -cores=4

.PHONY: build run
