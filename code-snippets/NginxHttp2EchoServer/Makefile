NAME ?= nginx-http2-echo
IMAGE ?= openresty/openresty:1.25.3.1-0-alpine
PORT ?= 19082
CONF ?= $(abspath ./nginx.conf)

.PHONY: start stop restart logs request

start:
	docker run --rm -it \
	  --platform=linux/amd64 \
	  --cpuset-cpus="17" \
		-m 512m \
		--name $(NAME) \
		--network host \
		-v "$(CONF)":/usr/local/openresty/nginx/conf/nginx.conf:ro \
		$(IMAGE)

stop:
	-docker stop $(NAME)

restart: stop start

logs:
	docker logs -f $(NAME)

req:
	curl -v --http2-prior-knowledge \
		-H 'content-type: application/json' \
		-d '{"hello":"world"}' \
		http://127.0.0.1:$(PORT)/echo?abc=def


stress-test-http2:
	docker run --rm -it --network host goodideal/nghttp2:latest \
		h2load -p h2c -c 16 -t 8 --max-concurrent-streams 60 -n 1000000 \
		http://127.0.0.1:$(PORT)/echo?seq=8888

