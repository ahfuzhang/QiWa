ROOT_DIR := $(abspath ../..)
PRJ=Http1EchoServer
BUILD_DIR := $(ROOT_DIR)/build/code-snippets/$(PRJ)

build:
	dotnet build $(PRJ).csproj -c Release -o $(BUILD_DIR)

run:
	$(BUILD_DIR)/$(PRJ) \
	  -http1.port=8081 \
	  -metrics.push.interval.seconds=5 \
	  -metrics.push.addr=http://127.0.0.1:8480/insert/0/prometheus/api/v1/import/prometheus \
	  -metrics.push.extra.labels='customer_brand=pg&customer_env=stress-test&customer_cluster=mp-landing-ali-cn-hk-c&namespace=backend-team&job=qiwa&pod=qiwa-metrics-push-88d88b5c7-00009' \
	  -threadpool.max=1

wrk:
	wrk -c 100 -t 4 -d 60s http://127.0.0.1:8081/echo?abc=def --latency

build-linux-amd64:
	docker run --rm \
	--platform linux/amd64 \
	-v "$(ROOT_DIR)":/src \
	-w /src/code-snippets/$(PRJ) \
	mcr.microsoft.com/dotnet/sdk:10.0 \
	bash -lc ' \
		apt-get update && \
		apt-get install -y --no-install-recommends clang lld binutils zlib1g-dev && \
		dotnet publish $(PRJ).csproj \
		-c Release -r linux-x64 \
		-p:PublishAot=true \
		-p:StripSymbols=false \
		--self-contained true \
		-o /src/build/code-snippets/$(PRJ)/linux/amd64 \
		'

build-macos:
	dotnet publish $(PRJ).csproj -c Release -r osx-arm64 -p:PublishAot=true --self-contained true -o $(BUILD_DIR)/osx/arm64

build-linux-arm64:
	docker run --rm \
	--platform linux/arm64 \
	-v "$(ROOT_DIR)":/src \
	-w /src/code-snippets/$(PRJ) \
	mcr.microsoft.com/dotnet/sdk:10.0 \
	bash -lc ' \
		apt-get update && \
		apt-get install -y --no-install-recommends clang lld binutils zlib1g-dev && \
		dotnet publish $(PRJ).csproj \
		-c Release -r linux-arm64 \
		-p:PublishAot=true \
		-p:StripSymbols=false \
		--self-contained true \
		-o /src/build/code-snippets/$(PRJ)/linux/arm64 \
		'
docker-build-linux-arm64:
	mkdir -p ./bin/linux/arm64/
	cp $(BUILD_DIR)/linux/arm64/$(PRJ) ./bin/linux/arm64/
	docker build --build-arg BIN=./bin/linux/arm64/$(PRJ) --platform linux/arm64 -t http1-echo-server .

docker-build-linux-amd64:
	mkdir -p ./bin/linux/amd64/
	cp $(BUILD_DIR)/linux/amd64/$(PRJ) ./bin/linux/amd64/
	docker build --build-arg BIN=./bin/linux/amd64/$(PRJ) --platform linux/amd64 -t http1-echo-server .

run-in-docker-linux-arm64:
	docker run --rm \
	    --platform=linux/arm64 \
		-p 8081:8081 \
		--cpuset-cpus="3" \
		-m 256m \
		http1-echo-server:latest \
			-http1.port=8081 \
			-metrics.push.interval.seconds=1 \
			-metrics.push.addr=http://host.docker.internal:8480/insert/0/prometheus/api/v1/import/prometheus \
			-metrics.push.extra.labels='customer_brand=pg&customer_env=stress-test&customer_cluster=mp-landing-ali-cn-hk-c&namespace=backend-team&job=qiwa&pod=qiwa-metrics-push-88d88b5c7-0000a' \
			-threadpool.max=1 \
			-output.request.log=true

run-in-docker-linux-amd64:
	docker run --rm \
	    --platform=linux/amd64 \
		-p 8081:8081 \
		--cpuset-cpus="3" \
		-m 256m \
		http1-echo-server:latest \
			-http1.port=8081 \
			-metrics.push.interval.seconds=1 \
			-metrics.push.addr=http://host.docker.internal:8480/insert/0/prometheus/api/v1/import/prometheus \
			-metrics.push.extra.labels='customer_brand=pg&customer_env=stress-test&customer_cluster=mp-landing-ali-cn-hk-c&namespace=backend-team&job=qiwa&pod=qiwa-metrics-push-88d88b5c7-0000a' \
			-threadpool.max=1 \
			-output.request.log=true

request_metrics:
	curl -G "http://127.0.0.1:8081/metrics"

request_echo:
	curl -G "http://127.0.0.1:8081/echo?abc=def" -v


docker-push:
	docker tag http1-echo-server:latest ahfuzhang/http1-echo-server:latest
	docker push ahfuzhang/http1-echo-server:latest

.PHONY: build run test
