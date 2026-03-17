尝试实现一个有业务逻辑的服务器，以此验证框架的能力覆盖范围。

* 开多个端口
  - http1 端口
  - http2 端口
  - grpc 端口
* 使用自定义日志类
  - 可以通过命令行配置 remote log
* 完善的 metrics 能力
* graceful shutdown 能力
* k8s 环节需要的 /healz 等接口
* IDL
  - 存在一个目录，有编译好的 proto 对应的代码
* 统一回调
  - 封装三种协议的请求，然后走到统一的回调函数上去    
* 最大线程数限制能力
* mysql + prepared statement 的支持
* redis 的支持
* 写一个类似用户 login 的逻辑
* thread local 的注册能力
* 把 counter 对象写在 thread local 里面，然后全局的注册链里面读取这个链

## todo

* 读 .proto 文件，根据 service 生成对应的回调前的处理代码
* 为每个类型，生成 Reset 函数，便于用对象池重用对象
* 对象池能力的提供

