
docker-build:
	docker build --platform linux/amd64 -t dotnet-dbg-container:latest .

ROOT_DIR := $(abspath ../..)

run-dbg-container:
	docker run --rm -it \
	--platform linux/amd64 \
	-v "$(ROOT_DIR)":/src \
	-w /src/ \
	dotnet-dbg-container:latest \
	bash
