ifeq ($(shell uname), Darwin)
    DOTNET := dotnet
else
    DOTNET := ~/.dotnet/dotnet
endif
PRJ := GrpcSimpleServer

build:
	$(DOTNET) build $(PRJ).csproj -c Release

run:
	$(DOTNET) run --project $(PRJ).csproj -- -http2.port=8090

.PHONY: build run

req:
	# gRPC unary 请求体格式: [1-byte compressed flag][4-byte message length][message bytes]
	# message bytes(本服务约定): [2-byte service length][service utf8][2-byte method length][method utf8][payload]
	printf '\000\000\000\000\013\000\004echo\000\003raw' | curl --http2-prior-knowledge \
		-X POST \
		-H 'Content-Type: application/grpc' \
		--data-binary @- \
		http://127.0.0.1:8090/greet.Greeter/SayHello --output - -v

req1:
	# gRPC unary 请求体格式: [1-byte compressed flag][4-byte message length][message bytes]
	# message bytes(本服务约定): [2-byte service length][service utf8][2-byte method length][method utf8][payload]
	printf '\000\000\000\000\013\000\004echo\000\003raw' | curl --http2-prior-knowledge \
		-X POST \
		-H 'Content-Type: application/grpc' \
		--data-binary @- \
		http://127.0.0.1:8090/greet.Greeter/Said --output - -v

req_notfound:
	# gRPC unary 请求体格式: [1-byte compressed flag][4-byte message length][message bytes]
	# message bytes(本服务约定): [2-byte service length][service utf8][2-byte method length][method utf8][payload]
	printf '\000\000\000\000\013\000\004echo\000\003raw' | curl --http2-prior-knowledge \
		-X POST \
		-H 'Content-Type: application/grpc' \
		--data-binary @- \
		http://127.0.0.1:8090/greet.Greeter/Said1 --output - -v

# * upload completely sent off: 16 bytes
# < HTTP/2 200 
# < content-type: application/grpc
# < date: Wed, 11 Mar 2026 03:16:58 GMT
# < server: Kestrel
# < content-length: 0
# < grpc-status: 12
# < grpc-message: Method is unimplemented.

