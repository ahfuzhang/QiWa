DOTNET ?= dotnet
PROJECT ?= TextParse.csproj
RUN_ARGS ?=

.PHONY: init build run

init:
	$(DOTNET) restore $(PROJECT)

build: init
	$(DOTNET) build $(PROJECT) -o ../../build/code-snippets/TextParse/

# make run RUN_ARGS="-input=../../Tests/data/metrics.txt"
run:
	dotnet run \
		--project $(PROJECT) -- $(RUN_ARGS)

fmt:
	$(DOTNET) format

test:
	dotnet test

# coverage:
# 	dotnet test /p:CollectCoverage=true /p:CoverletOutput=../../build/TestResults/ /p:CoverletOutputFormat=opencover

# report:
# 	dotnet tool run reportgenerator \
# 	-reports:../../build/TestResults/coverage.opencover.xml \
# 	-targetdir:../../build/coveragereport \
# 	-reporttypes:Html

# test:
# 	dotnet test

# benchmark:
# 	dotnet run -c Release /p:StartupObject=Benchmarks.Program



add-package:
	dotnet add package xunit
	dotnet add package xunit.runner.visualstudio
	dotnet add package Microsoft.NET.Test.Sdk
	dotnet add package coverlet.collector


.PHONY: test

