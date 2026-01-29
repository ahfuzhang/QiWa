DOTNET ?= dotnet
PRJ=ConsoleLoggerCli
PROJECT ?= $(PRJ).csproj
RUN_ARGS ?=

.PHONY: init build run

init:
	$(DOTNET) restore $(PROJECT)

build: init
	$(DOTNET) build $(PROJECT) -o ../../build/code-snippets/$(PRJ)/

# make run RUN_ARGS="-threadpool.max=1"
run: build
	$(DOTNET) run --project $(PROJECT) -- $(RUN_ARGS)

install-debuger:
	curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l ~/.vsdbg

debug-run:
	~/.vsdbg/vsdbg \
		--interpreter=mi \
		-- \
		dotnet ../../build/code-snippets/$(PRJ)/$(PRJ) -- -threadpool.max=1

install-sos:
	dotnet tool install -g dotnet-sos
	dotnet-sos install

lldb:
	lldb -- ../../build/code-snippets/$(PRJ)/$(PRJ) -threadpool.max=1

fmt:
	$(DOTNET) format
