MAKEFILE_DIR := $(abspath $(dir $(lastword $(MAKEFILE_LIST))))
ROOT_DIR := $(abspath $(MAKEFILE_DIR)/../..)
ROOT_UID := $(shell stat -c %u "$(ROOT_DIR)")
ROOT_GID := $(shell stat -c %g "$(ROOT_DIR)")
CONFIG_DIR := $(ROOT_DIR)/.code-server
WORKSPACE_BASE := /config/workspace
HOST_DIR := $(abspath $(CURDIR))
REL_DIR := $(patsubst $(ROOT_DIR)%,%,$(HOST_DIR))
ifeq ($(REL_DIR),$(HOST_DIR))
WORKSPACE_DIR := $(WORKSPACE_BASE)
else
WORKSPACE_DIR := $(WORKSPACE_BASE)$(REL_DIR)
endif

run1:
	mkdir -p "$(CONFIG_DIR)"
	docker run -it --rm --name=code_server \
		-e PUID=$(ROOT_UID) \
		-e PGID=$(ROOT_GID) \
		-e TZ=Asia/Singapore \
		-e AUTH=none \
		-e DEFAULT_WORKSPACE=/workspace \
		-p 8443:8443 \
		-v "$(CONFIG_DIR)":/config \
		-v "$(ROOT_DIR)":/workspace \
		-w $(ROOT_DIR) \
		linuxserver/code-server:latest

# 		-v "$(ROOT_DIR)":/config/workspace 

docker-build:
	docker build --platform linux/amd64 -t dotnet-code-server:latest .

docker-build-dotnet6:
	docker build --platform linux/amd64 -t dotnet-code-server:dotnet6 -f Dockerfile_dotnet6 .

run:
	mkdir -p "$(CONFIG_DIR)"
	docker run -it --rm --name=code_server \
		-e PUID=$(ROOT_UID) \
		-e PGID=$(ROOT_GID) \
		-e TZ=Asia/Singapore \
		-e AUTH=none \
		-e DEFAULT_WORKSPACE=/workspace \
		-p 8443:8443 \
		-v "$(CONFIG_DIR)":/config \
		-v "$(ROOT_DIR)":/workspace \
		-w $(ROOT_DIR) \
		dotnet-code-server:latest
