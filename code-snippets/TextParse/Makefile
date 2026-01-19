DOTNET ?= dotnet
PROJECT ?= TextParse.csproj
RUN_ARGS ?=

.PHONY: init build run

init:
	$(DOTNET) restore $(PROJECT)

build: init
	$(DOTNET) build $(PROJECT) -o ../../build/TextParse/

# make run RUN_ARGS="-input=./test/data/metrics.txt"
run: build
	$(DOTNET) run --project $(PROJECT) -- $(RUN_ARGS)

fmt:
	$(DOTNET) format

coverage:
	dotnet test /p:CollectCoverage=true /p:CoverletOutput=../../build/TestResults/ /p:CoverletOutputFormat=opencover

report:
	dotnet tool run reportgenerator \
	-reports:../../build/TestResults/coverage.opencover.xml \
	-targetdir:../../build/coveragereport \
	-reporttypes:Html

test:
	dotnet test

benchmark:
	dotnet run -c Release /p:StartupObject=Benchmarks.Program

.PHONY: test

