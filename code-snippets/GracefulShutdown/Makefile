DOTNET ?= dotnet
PROJECT ?= GracefulShutdown.csproj
RUN_ARGS ?=

.PHONY: init build run

init:
	$(DOTNET) restore $(PROJECT)

build: init
	$(DOTNET) build $(PROJECT) -o ../../build/GracefulShutdown/

# make run RUN_ARGS="-threadpool.max=1"
run: build
	$(DOTNET) run --project $(PROJECT) -- $(RUN_ARGS)

fmt:
	$(DOTNET) format

pkill:
	pkill -TERM -x GracefulShutdown
