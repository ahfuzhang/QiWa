
# 目标
实现一个简单的 grpc 服务，以最简洁的例子代码实现。

本质上，我是在上一次提示词 ./code-snippets/GrpcGeneicServer/prompt.md 生成的结果的基础上，再去掉冗余部分，得到了一个更加精简的例子代码。

# 步骤
* 忽略项目根目录的 AGENTS.md
* 在当前文件夹下生成仅一个 Program.cs 文件
* 参考项目根目录开始的 ./code-snippets/GrpcGeneicServer/src/GrpcGeneicServer/ 下的逻辑
  - 尽可能精简的还原 “自己做路由“+“自己处理序列化“+echo 服务
  - 如果可以直接调一个函数，就别增加类型。类型越少越好
  - 代码尽量精简，把被参照的项目按照最精简的方式实现。

