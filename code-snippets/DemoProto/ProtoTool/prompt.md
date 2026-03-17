
# 目标

用 golang 实现一个 .proto 文件的解析的命令行工具。

# 命令行参数

* `-proto=xxx.proto`
  - 指定 .proto 文件的路径

# 步骤

* 从命令行参数 `-proto=xxx.proto` 读取输入的 .proto 文件
* 遍历所有的 service
  - 遍历所有的 method
  - 输出 method 对应的 request 类型和 response 类型
  

