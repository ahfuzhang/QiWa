DOTNET ?= dotnet
PRJ=ConsoleLoggerCli
PROJECT ?= $(PRJ).csproj
RUN_ARGS ?=

.PHONY: init build run

init:
	$(DOTNET) restore $(PROJECT)

build: init
	$(DOTNET) build $(PROJECT) -o ../../build/code-snippets/$(PRJ)/

build-debug:
	dotnet build -c Debug $(PROJECT) -o ../../build/code-snippets/$(PRJ)/

# make run RUN_ARGS="-threadpool.max=1"
run: build
	$(DOTNET) run -r linux-x64 -p:DefineConstants=UNIX -p:AllowUnsafeBlocks=true --project $(PROJECT) -- $(RUN_ARGS)

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

linux-install:
	dotnet tool install --global dotnet-dump

linux-debug-0:
	~/netcoredbg/netcoredbg \
		--interpreter=cli -- \
		../../build/code-snippets/$(PRJ)/$(PRJ) -threadpool.max=1

linux-debug:
	~/netcoredbg/netcoredbg \
        --interpreter=cli -- \
        dotnet ../../build/code-snippets/ConsoleLoggerCli/ConsoleLoggerCli.dll -threadpool.max=1

fmt:
	$(DOTNET) format
