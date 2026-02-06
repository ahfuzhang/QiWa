
# 目标

通过 nginx.conf + docker 启动参数，搭建一个基于 nginx 的 http 1.1 协议的 echo server.

# 具体配置

访问 /echo 路径的时候，向客户端返回：

$method ${host}${path}${querystring} $version
$foreach ($headers)

# 生成要求

1. 生成 nginx.conf 配置文件
2. 生成 Makefile, 当执行 make start 的时候，启动 nginx 服务器

