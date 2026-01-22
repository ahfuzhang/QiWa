# Using runtime-deps since we have a self-contained AOT binary
# Removing -chiseled to allow shell access (/bin/sh) and standard utilities
FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-noble
ARG BIN=./bin/linux/arm64/Http1EchoServer
ENV DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false

WORKDIR /app

# build/code-snippets/Http1EchoServer/linux/amd64
# Use --chmod to set execution permissions directly during copy
COPY --chmod=0755 ${BIN} /app/Http1EchoServer

ENTRYPOINT ["/app/Http1EchoServer"]
#CMD ["/app/Http1EchoServer", "-addr=0.0.0.0:8081", "-threadpool.min=4", "-threadpool.max=4"]
