#FROM mcr.microsoft.com/dotnet/aspnet:8.0
FROM mcr.microsoft.com/dotnet/runtime:10.0-alpine

ENV DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false

WORKDIR /app

# RUN apt-get update && \
#     apt-get install -y --no-install-recommends procps && \
#     rm -rf /var/lib/apt/lists/*

# build/code-snippets/Http1EchoServer/linux/amd64
COPY bin/Http1EchoServer /app/Http1EchoServer

RUN chmod +x /app/Http1EchoServer

#ENTRYPOINT ["/app/KestrelServer", "-addr=0.0.0.0:8081"]
#CMD ["/app/Http1EchoServer", "-addr=0.0.0.0:8081", "-threadpool.min=4", "-threadpool.max=4"]
