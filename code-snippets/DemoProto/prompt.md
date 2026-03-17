
# 目标

生成一个命令行测试程序，用于检查 proto 生成后的代码。

# 步骤

1 生成 DemoProto.csproj 文件
2 生成 Program.cs 文件
3 引入 ./gen/demo.cs 文件的代码
4 填充 AllTypesMessage 类型的每个字段
  4.1 序列化为 二进制，把序列化的结果报存到 ../../build/code-snippets/DemoProto/ 下面
  4.2 序列化为 json 格式，把序列化的结果报存到 ../../build/code-snippets/DemoProto/ 下面
  4.3 序列化为 yaml 格式，把序列化的结果报存到 ../../build/code-snippets/DemoProto/ 下面
5 在上面保存文件的基础上，另外提供一个反序列化的测试函数：
  5.1 protobuf 二进制反序列化
  5.2 json 反序列化
  5.3 yaml 反序列化
  5.4 对比上面的三次反序列化后的数据是否完全一致。不一致则报错.
6 生成 Makefile
  - make build 可以编译到 ../../build/code-snippets/DemoProto/
  - make run 可以执行
