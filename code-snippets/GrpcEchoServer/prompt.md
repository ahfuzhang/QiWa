
# 目标
基于 Kestrel 的 http2 服务来实现一个简洁高效的 grpc 服务器端。
只支持请求/响应的模式，不支持 streaming 的模式。
1. 这个服务可以很好的展示基于 http2 协议来实现 grpc 服务器的全过程
2. 最终能够基于本次的实验，实现一个轻量级的且不依赖繁重的其他框架的 grpc 框架

# 实现原理
* 所有的 http2 的 callback 放到一个函数中
* 如果不是 post 请求，返回 400 错误
* if streamID%2 != 1 || streamID <= currentContext.maxStreamID then response 400
* 如果 http header 中没有 header: `content-type=application/grpc`，返回 400 错误
* 检查 path 是否是：/my_service/my_method
* 其他的关于压缩等处理
* 得到未 decode 的 protocol buffers 序列化后的 body
* 原样返回 request 的序列化的数据，实现一个 echo 服务
* 要考虑返回 http2 协议的 frame 的尾部数据，作为 grpc 协议的成功/失败的信息

# 限制
* 实现一个简洁的 grpc 服务，不要加入与这个主题无关的代码
* 基于 Kestrel 框架中 http2 相关的 api 来实现，不要基于 tcp 协议来做

# 命令行参数
* `-http2.port=8090`: 设定监听的端口

# 参考代码

目录 ref/http2_server.go 这个文件来自 grpc-go v1.78.0 的 internal/transport/http2_server.go
这个文件中的 `func (t *http2Server) operateHeaders` 实现了 grpc 解析的过程。
请仿照里面的逻辑，生成对应的 C# 代码。

# 目标文件

* 在 ./src/ 目录下组织所有 C# 代码
* 在当前目录生成 GrpcEchoServer.csproj 文件
* 在当前目录生成 Makefile， 提供 build 和 run 命令
  - 生成完代码后调用 make run，确保可以正常编译通过。
