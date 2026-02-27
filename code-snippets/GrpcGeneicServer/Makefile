DOTNET := ~/.dotnet/dotnet
PRJ := GrpcGeneicServer

build:
	$(DOTNET) build $(PRJ).csproj -c Release

run:
	$(DOTNET) run --project $(PRJ).csproj -- -http2.port=8090

.PHONY: build run req


# 根据提示词意图：用 curl 访问 h2c gRPC 接口，并发送合法 gRPC 帧与业务 envelope，避免 Incomplete message。
# < grpc-status: 0
req:
	# gRPC unary 请求体格式: [1-byte compressed flag][4-byte message length][message bytes]
	# message bytes(本服务约定): [2-byte service length][service utf8][2-byte method length][method utf8][payload]
	printf '\000\000\000\000\013\000\004echo\000\003raw' | curl --http2-prior-knowledge \
		-X POST \
		-H 'Content-Type: application/grpc' \
		--data-binary @- \
		http://127.0.0.1:8090/greet.Greeter/SayHello --output - -v

# < grpc-status: 12
# < grpc-message: Method is unimplemented.
req1:
	# gRPC unary 请求体格式: [1-byte compressed flag][4-byte message length][message bytes]
	# message bytes(本服务约定): [2-byte service length][service utf8][2-byte method length][method utf8][payload]
	printf '\000\000\000\000\013\000\004echo\000\003raw' | curl --http2-prior-knowledge \
		-X POST \
		-H 'Content-Type: application/grpc' \
		--data-binary @- \
		http://127.0.0.1:8090/greet.Greeter/SayHello1 --output - -v
