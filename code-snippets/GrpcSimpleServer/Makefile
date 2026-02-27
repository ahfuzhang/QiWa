DOTNET := ~/.dotnet/dotnet
PRJ := GrpcSimpleServer

build:
	$(DOTNET) build $(PRJ).csproj -c Release

run:
	$(DOTNET) run --project $(PRJ).csproj -- -http2.port=8090

.PHONY: build run
