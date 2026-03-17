ROOT_DIR := $(abspath ../..)
DOTNET ?= dotnet
PRJ=DemoProto
PROJECT ?= $(PRJ).csproj
OUTPUT_DIR := ../../build/code-snippets/$(PRJ)

.PHONY: init build run

init:
	$(DOTNET) restore $(PROJECT)

build: init
	$(DOTNET) build $(PROJECT) -o $(OUTPUT_DIR)

run: build
	$(DOTNET) run --project $(PROJECT)

fmt:
	$(DOTNET) format
