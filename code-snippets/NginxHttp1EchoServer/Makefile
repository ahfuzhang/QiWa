NAME ?= nginx-http1-echo
IMAGE ?= openresty/openresty:1.25.3.1-0-alpine
PORT ?= 19081
CONF ?= $(abspath ./nginx.conf)

.PHONY: start stop restart logs request

start:
	docker run --rm -it \
	  --platform=linux/amd64 \
	  --cpuset-cpus="18" \
		-m 512m \
		--network host \
		--name $(NAME) \
		-v "$(CONF)":/usr/local/openresty/nginx/conf/nginx.conf:ro \
		$(IMAGE)

stop:
	-docker stop $(NAME)

restart: stop start

logs:
	docker logs -f $(NAME)

request:
	curl -G "http://127.0.0.1:$(PORT)/echo?abc=def" -v

stress-test-http1:
	docker run --rm -it --network host goodideal/nghttp2:latest \
		h2load -p http/1.1 -c 100 -t 8 -n 200000 \
		http://127.0.0.1:$(PORT)/echo?seq=9999
