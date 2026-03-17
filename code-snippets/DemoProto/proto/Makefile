
build:
	protogen \
	  demo.proto \
	  --proto_path=./ \
	  --proto_path=/Users/ahfu/code/github.com/googleapis/googleapis/ \
	  --csharp_out=../gen/ --grpc=DemoService

generate-golang:
	mkdir -p ../gen
	buf generate

install:
	go install google.golang.org/protobuf/cmd/protoc-gen-go@latest
