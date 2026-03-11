ifeq ($(shell uname), Darwin)
    DOTNET := dotnet
else
    DOTNET := ~/.dotnet/dotnet
endif
PRJ := Client

build:
	$(DOTNET) build $(PRJ).csproj -c Release

run:
	$(DOTNET) run --project $(PRJ).csproj -- -addr=http://127.0.0.1:8090

.PHONY: build run
