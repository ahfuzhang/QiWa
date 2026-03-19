
# 目标
实现一个测试程序。
基于 C# 的 Roslyn 库，读取 ../gen/demo.cs 文件。

# 步骤

* 遍历所有类型
* 找到 继承自 `global::ProtoBuf.IExtensible` 的 class
  - 遍历实现了 `[global::ProtoBuf.ProtoMember(x)]` 属性的成员
* 为类生成 Reset() 方法
  - 把存在 `[global::ProtoBuf.ProtoMember(x)]` 的成员，复制为其数据类型的空值
    - 整数类型: this.Property = 0
    - 浮点数类型：this.Property = 0.0
    - 布尔类型：this.Property = false
    - 字符串类型： this.Property = ""
    - 数组类型： this.Property = null
    - Dictionary 类型： this.Property.Clear()
* 把生成的代码，写回文件 ../gen/demo.cs

# 输出

* GenCode.csproj 文件
* Program.cs
* Makefile
  - make build 可以编译
  - make run 可以运行

# 其他要求
要读取项目根目录下的 AGENTS.md 作为系统提示词

