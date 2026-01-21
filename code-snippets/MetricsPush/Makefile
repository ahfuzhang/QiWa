ROOT_DIR := $(abspath ../..)
BUILD_DIR := $(ROOT_DIR)/build/code-snippets/MetricsPush

build:
	dotnet build MetricsPush.csproj -c Release -o $(BUILD_DIR)

# run:
# 	dotnet run --project MetricsPush.csproj -- $(ARGS)

run:
	$(BUILD_DIR)/MetricsPush \
	  -http1.port=8081 \
	  -push.interval.seconds=10 \
	  -push.addr=http://127.0.0.1:8480/insert/0/prometheus/api/v1/import/prometheus \
	  -extraLabels='namespace=system-team&job=qiwa&pod=qiwa-metrics-push-88d88b5c7-00006'

test:
	dotnet test MetricsPush.Tests.csproj

.PHONY: build run test run1

