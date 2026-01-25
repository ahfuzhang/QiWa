
coverage:
	mkdir -p build/TestResults
	dotnet test QiWa.csproj /p:CollectCoverage=true /p:CoverletOutput=./build/TestResults/ /p:CoverletOutputFormat=opencover  -r osx-arm64
	dotnet tool run reportgenerator \
	-reports:./build/TestResults/coverage.opencover.xml \
	-targetdir:./build/coveragereport \
	-reporttypes:Html

report:
	dotnet tool run reportgenerator \
	-reports:./build/TestResults/coverage.opencover.xml \
	-targetdir:./build/coveragereport \
	-reporttypes:Html

test:
	dotnet test QiWa.csproj --logger "console;verbosity=detailed" -r osx-arm64

benchmark:
	dotnet run --project Benchmarks/Benchmarks.csproj -c Release

benchmark-all:
	dotnet run --project Benchmarks/Benchmarks.csproj -c Release -- --filter *

flamegraph:
	dotnet-trace convert --format Speedscope \
	--output build/benchmarks/trace.speedscope.json \
	build/benchmarks/Benchmarks.ParseTextUtf8Bench.ParseUtf16-20260120-145256.nettrace

add-package:
	dotnet add package xunit
	dotnet add package xunit.runner.visualstudio
	dotnet add package Microsoft.NET.Test.Sdk
	dotnet add package coverlet.collector
	dotnet tool install -g dotnet-trace

fmt:
	dotnet format QiWa.csproj

.PHONY: test fmt
