
# 目标

实现一个登录服务器的例子，一个基于 C# Kestrel 的后端服务，基于 mysql + redis 实现登录和 session 的管理。

# 实现细节

## 命令行参数

* `-log.level=`
  - 日志级别的配置
  - 可选: error, warn, info, debug
  - 默认为 warn
  - 对应 src/ConsoleLogger/Logger.cs 下 Init() 函数的 level 参数
* `-log.flush.interval.ms=1000`
  - 日志的 flush 时间间隔，单位为毫秒
  - 默认为 1000
  - 对应 src/ConsoleLogger/Logger.cs 下 Init() 函数的 flushIntervalMs 参数
* `-log.buffer.size=64k`
  - 日志的 buffer 的字节数
  - 支持以下简写的后缀: (不区分大小写)
    - `k`, `kb`
    - `m`, `mb`
    - `g`, `gb`
    - `t`, `tb`
    - `p`, `pb`
  - 默认为 64kb
  - 最大值为 1G  
  - 对应 src/ConsoleLogger/Logger.cs 下 Init() 函数的 logBufferSize 参数
* `-log.push.addr=`
  - 设置一个日志 post 的 http 地址  
  - 默认为空字符串
  - 对应 src/ConsoleLogger/Logger.cs 下 Init() 函数的 jsonlineUrl 参数
* `-log.global.tags="a=b&c=d"`
  - 设置日志的全局 tags
  - 以 url query string 的方式解析参数值，转换为 Dictionary<string, string>
    - 解析错误，抛出 Exception
  - 默认为空字符串
  - 对应 src/ConsoleLogger/Logger.cs 下 Init() 函数的 tags 参数  
* `-http1.port=`
  - http 1.1 的监听端口
  - 这个端口必须设置
* `-http2.port=`
  - http2 的端口
  - 可选
* `-grpc.port=`
  - grpc 协议的端口
  - 可选
* `-cores=1`
  - 设置线程池的最大线程数
  - 如果提供此参数，对线程池的最大值进行限制
    - ```csharp
      ThreadPool.SetMinThreads(n, n);
      ThreadPool.SetMaxThreads(n, n);
      ```

## Graceful shutdown

需要为服务提供 graceful shutdown 的能力。

进程结束前，需要调用：

```csharp
// src/ConsoleLogger/Logger.cs
Logger.Shutdown()
```

## metrics 上报

* 在 http1 的端口，注册 /metrics 路径。访问此路径时，返回 prometheus 格式的 metrics 数据
* 提供 Kestrel 内部的 http1/http2/grpc 的指标上报
* 提供 DotNet runtime 的上报
* 使用 OpenTelemetry 的库进行上报，如果某些指标来自 /proc/self/ ，则上报这些指标

## logs 输出

* 使用项目根目录下的 ./src/ConsoleLogger/ 库进行日志输出
* 在程序启动时，调用 Logger.Init() 来初始化日志库
  - 初始化的参数来自命令行参数
* 每个请求都输出日志
  - 使用 TaskLogger.Info() 来输出
  - 输出多个字段，而不是使用字符串格式化来把一堆信息糅合在一起

## 配置文件

* 程序启动时，读取当前目录下的 config.yaml

```yaml
mysql:
  dsn: "server=${server};user id=${user}i;password=$password{}$;persistsecurityinfo=True;port=3306;database=${db};Max Pool Size=10"  # 这里是 mysql 的host/user/password/db 等信息
redis: "${host}$:6379,password=${password}$"
```

* 根据 yaml 格式，定义对应的 struct
* 把解析后的数据，存储到对应的 struct
* 存储了配置信息的 struct 全局可见。可以在任意位置访问配置

## MysqlConnector

* 使用 MysqlConnector 来操作数据库
* 数据库使用 config.yaml 中的 mysql 配置
* 数据库连接对象，作为 thread local 对象来存储
* 全程都要使用 async api，避免阻塞

## StackExchange.Redis
* 使用 StackExchange.Redis 库来访问 redis
* redis 的连接信息来自 config.yaml
* redis 的连接对象，作为 thread local 对象来存储
* 限制每个 redis client 的连接数，避免连接过多
* 全程都要使用 async api，避免阻塞

# 业务设计

## 用户表

* 定义一个用户数据库和用户表，生成对应的 init.sql，便于后续创建到数据库中
* 用户表
  - 包含以下主要字段: user_id, user_name, user_password_sha(使用 sha256 计算的密码)

## 用户 cache

* 用户的 session 信息缓存到 redis 中
* 用户基本信息：
  * key: $user_id
  * value:
    - 使用 protobuf 定义的结构，包含了 user_name, last_loging_time, session_id
* session 信息:
  * key: $session_id
  * value: $user_id

# 业务流程

* 用户通过 /login 接口登录
  - post {"user_name":"$user_name", "user_password_sha":"$sha256"}
  - 在数据库中查询是否存在 select * from users where user_name=$user_name and user_password_sha=$user_password_sha 的记录
  - 访问数据库，要使用 parepared statement
    - parepared statement 要建立对象池，避免每次都预编译语句
* 登录失败，返回错误信息
* 登录成功后:
  - 产生一个 uuid，作为 session_id
  - 在 redis 中写入两个 key
    - $user_id -> {user_name, session_id, last_loging_time}    
    - $session_id -> $user_id
    - key 的过期时间为 30 分钟
  - 向用户返回 $session_id
* 业务接口
  - `/biz_logic`
  - post 访问
  - 请求格式: {"action":"", "session_id":"xxx"}
  - 查询 redis 中对应的 session_id 是否存在
    - 如果不存在，返回信息告诉用户先登录
  - 鉴权通过后，返回整个 request 结构，作为业务输出

# 输出
* ./sql/*.sql 生成数据库对应的 sql
* ./proto/*.proto 定义配置文件、请求格式、响应格式等，都使用 proto 来定义
  - 使用 protobuf-net 来生成 .cs 文件，放到 ./gen/ 目录
* ./src/*.cs 生成 C# 代码
* Makefile
  - make build 可以编译
  - make run 可以运行


# 其他特性

## 全局异常捕获

参考以下代码，提供全局异常捕获的能力：

```csharp
using System;
using System.Threading;
using System.CommandLine;
using System.Threading.Tasks;

internal static class Program {
    private const int UnhandledExceptionExitCode = 99;
    private static int _hasPrintedUnhandledException;
    private static Timer? _unobservedTaskExceptionWatchdog;

    static Program()
    {
        ConfigureGlobalExceptionHandling();
    }

    public static async Task Main(string[] args) {
         // 这里引用 thread local
         // 并且在 thread local 对象中抛出未捕获的异常
    }

    private static void ConfigureGlobalExceptionHandling()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) =>
        {
            PrintUnhandledException("AppDomain.CurrentDomain.UnhandledException", eventArgs.ExceptionObject as Exception);
        };

        TaskScheduler.UnobservedTaskException += (_, eventArgs) =>
        {
            PrintUnhandledException("TaskScheduler.UnobservedTaskException", eventArgs.Exception);
        };

        StartUnobservedTaskExceptionWatchdog();
    }

    private static void StartUnobservedTaskExceptionWatchdog()
    {
        _unobservedTaskExceptionWatchdog = new Timer(_ =>
        {
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception ex)
            {
                PrintUnhandledException("UnobservedTaskExceptionWatchdog", ex);
            }
        }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
    }

    private static void PrintUnhandledException(string source, Exception? exception)
    {
        if (Interlocked.Exchange(ref _hasPrintedUnhandledException, 1) == 1)
        {
            return;
        }

        Console.Error.WriteLine($"[{DateTimeOffset.UtcNow:u}] Unhandled exception caught from {source}");
        if (exception is null)
        {
            Console.Error.WriteLine("Exception object was null.");
        }
        else
        {
            Console.Error.WriteLine(exception);
        }

        Console.Error.Flush();
        Environment.Exit(UnhandledExceptionExitCode);
    }

}


```


# 2026-04-04 增加 http2 的 MapFallback

提示词：增加 http2 的 Mapfallback 函数，callback 代码放在 Handlers/Http2.cs 中
相关文件：
- code-snippets/DemoLoginServer/src/KestrelInit.cs:107
- code-snippets/DemoLoginServer/src/Handlers/Http2Handler.cs（新增）
