
# 目标

通过 nginx.conf + docker 启动参数，搭建一个基于 nginx 的 http 2 协议的 echo server.

# 具体配置

访问 /echo 路径的时候，向客户端返回：

$method ${host}${path}${querystring} $version
$foreach ($headers)

# 生成要求

1. 生成 nginx.conf 配置文件
2. 关闭 access log
3. max-concurrent-streams 配置为 500
4. 如果存在常见的优化 http2 的参数，尽量都加上
