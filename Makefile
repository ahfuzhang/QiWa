
coverage:
	mkdir -p build/TestResults
	dotnet test QiWa.csproj /p:CollectCoverage=true /p:CoverletOutput=./build/TestResults/ /p:CoverletOutputFormat=opencover
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
	dotnet test QiWa.csproj

benchmark:
	dotnet run --project Benchmarks/Benchmarks.csproj -c Release

add-package:
	dotnet add package xunit
	dotnet add package xunit.runner.visualstudio
	dotnet add package Microsoft.NET.Test.Sdk
	dotnet add package coverlet.collector


.PHONY: test
