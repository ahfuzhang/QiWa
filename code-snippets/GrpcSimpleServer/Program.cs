using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;
using Grpc.AspNetCore.Server;         // Marshaller<T>、SerializationContext、DeserializationContext
using Grpc.AspNetCore.Server.Model;   // IServiceMethodProvider<T>、ServiceMethodProviderContext<T>
using Grpc.Core;                      // Method<TReq,TResp>、MethodType、ServerCallContext、RpcException、Status、StatusCode
using Microsoft.AspNetCore.Builder;   // WebApplication、WebApplicationBuilder
using Microsoft.AspNetCore.Hosting;   // IWebHostBuilder（ConfigureKestrel 扩展所在）
using Microsoft.AspNetCore.Server.Kestrel.Core; // HttpProtocols.Http2
using Microsoft.Extensions.DependencyInjection; // AddGrpc()、AddSingleton()

using Google.Protobuf;
using Greet;

namespace GrpcSimpleServer;

// Minimal wrapper: gRPC marshaller requires a reference type as the generic parameter.
// 这个类包装原始请求
// ──────────────────────────────────────────────────────────────────────────────
// 为什么需要这个类？
//   Grpc.AspNetCore 的 Marshaller<T> 要求 T 是引用类型（class），
//   而 ReadOnlySequence<byte> 本身是结构体（struct），所以必须用一个 class 包一层。
// ──────────────────────────────────────────────────────────────────────────────
internal sealed class RawPayload {
    // 实际的字节内容，ReadOnlySequence<byte> 是链表结构，可以零拷贝地引用多个内存块
    public ReadOnlySequence<byte> Bytes { get; }

    // 构造函数：直接存入字节序列，不做任何拷贝
    public RawPayload(ReadOnlySequence<byte> bytes) => Bytes = bytes;
}

// Single gRPC service: custom envelope decode → route dispatch → echo.
// ──────────────────────────────────────────────────────────────────────────────
// 这是唯一的 gRPC 服务类，承担以下三件事：
//   1. 尝试按自定义信封格式解析请求，提取 service/method 路由键
//   2. 按路由键在路由表中找到处理函数
//   3. 调用处理函数得到响应（echo 场景下即原样返回）
// ──────────────────────────────────────────────────────────────────────────────
internal sealed class EchoService {
    // 严格模式 UTF-8 编码器：遇到非法字节抛异常而不是静默替换，
    // 这样可以及早发现格式错误的信封，避免路由到错误的 handler。
    private static readonly UTF8Encoding StrictUtf8 = new(false, true);

    // 路由的 path
    /*
      Func 定义了函数
      ReadOnlySequence<byte> foo(ReadOnlySequence<byte> input){}
    */
    // 路由表：key = "service/method"，value = 处理该路由的函数
    // 使用 Ordinal 比较保证大小写敏感、无文化相关歧义
    private static readonly Dictionary<string, Func<ReadOnlySequence<byte>, ReadOnlySequence<byte>>> Routes =
        new(StringComparer.Ordinal) {
            // echo 路由：自定义信封协议下的测试入口
            ["echo/raw"] = static p => p,
            ["greet.Greeter/SayHello1"] = static p => p,  // echo 服务器，输入什么，也输出什么
        };

    // RawPayload request, 包装了原始请求
    // ──────────────────────────────────────────────────────────────────────────
    // 这是 gRPC 框架调用的入口方法（由 EchoServiceMethodProvider 注册）。
    // 参数：
    //   request  —— 框架已从 gRPC 帧中解出的原始字节，由 RawMarshaller 反序列化而来
    //   context  —— gRPC 调用上下文，包含 Method（完整路径如 /greet.Greeter/SayHello）等元数据
    // 返回值：RawPayload，框架会再由 RawMarshaller 序列化写回 gRPC 帧
    // ──────────────────────────────────────────────────────────────────────────
    public static Task<RawPayload> DispatchAsync(RawPayload request, ServerCallContext context) {
        ReadOnlySequence<byte> bytes = request.Bytes;
        string routeKey;
        ReadOnlySequence<byte> payload;

        // 先尝试按自定义二进制信封格式解析路由键和业务 payload
        if (TryDecodeEnvelope(bytes, out string envelopeRoute, out ReadOnlySequence<byte> envelopePayload)) {
            // grpc 协议头，在 body 里面加了 service 和 method
            routeKey = envelopeRoute;
            payload = envelopePayload;
        } else {
            // Fall back: derive route key from gRPC path "/service/method".
            // 信封解析失败时，退回到用 HTTP/2 的 :path 伪头（即 gRPC 方法路径）做路由。
            // context.Method 格式为 "/greet.Greeter/SayHello"，去掉前导 '/' 后与路由表 key 对齐。
            string normalized = context.Method.TrimStart('/');
            routeKey = normalized;
            payload = bytes; // 整个 gRPC payload 直接交给 handler
        }

        // 路由表查找；找不到则返回 gRPC 标准错误码 UNIMPLEMENTED（12）
        if (!Routes.TryGetValue(routeKey, out var handler))
            throw new RpcException(new Status(StatusCode.Unimplemented, $"No route: {routeKey}"));
        // handler 理解为业务处理函数
        // 调用 handler 得到响应 payload，包装成 RawPayload 返回给框架序列化
        return Task.FromResult(new RawPayload(handler(payload))); // 不会生成异步状态机
    }

    // Custom envelope format: [u16 serviceLen][service utf8][u16 methodLen][method utf8][payload].
    // 猜测是模拟 decode 请求包的过程
    // ──────────────────────────────────────────────────────────────────────────
    // 自定义二进制信封协议解析：
    //   字节布局：[2字节 service 名长度(大端)][service 名 UTF-8][2字节 method 名长度(大端)][method 名 UTF-8][业务 payload]
    // 返回值：
    //   true  —— 解析成功，routeKey = "service/method"，payload = 剩余业务字节
    //   false —— 格式不符，调用方应 fall back 到 gRPC path 路由
    // ───────────────────────────────── ─────────────────────────────────────────
    public static bool TryDecodeEnvelope(ReadOnlySequence<byte> data, out string routeKey, out ReadOnlySequence<byte> payload) {
        // grpc 协议有自己的封装格式
        routeKey = string.Empty;
        payload = default;
        // 链表块，包装成链表块的读对象
        // SequenceReader 是零拷贝的游标读取器，内部维护当前读取位置，
        // 支持跨多个内存块（ReadOnlySequence 的每个 Segment）连续读取
        var reader = new SequenceReader<byte>(data);
        // 依次读取 service 名和 method 名；任意一步失败则整体返回 false
        if (!TryReadToken(ref reader, out string service) || !TryReadToken(ref reader, out string method))
            return false;
        // 两个字段都不能为空或纯空白
        if (string.IsNullOrWhiteSpace(service) || string.IsNullOrWhiteSpace(method))
            return false;
        // 组合为路由表的 key 格式
        routeKey = $"{service}/{method}";
        // reader.Position 此时指向业务 payload 的起始位置，直接 Slice 零拷贝截取
        payload = data.Slice(reader.Position);   // ??? 确定不会拷贝吗?
        return true;
    }

    // ──────────────────────────────────────────────────────────────────────────
    // 将字节序列以 hex dump 格式打印到 Console，格式：
    //   每行 16 字节，十六进制两位大写，空格分隔；
    //   行尾追加 ASCII 可打印字符（0x20–0x7E），不可打印字符显示为 '.'。
    // ──────────────────────────────────────────────────────────────────────────
    public static void DumpHex(ReadOnlySequence<byte> data) {
        // 将链表结构展平，方便按偏移随机访问
        byte[] buf = data.ToArray();
        var sb = new StringBuilder(80);
        for (int i = 0; i < buf.Length; i += 16) {
            sb.Clear();
            int lineLen = Math.Min(16, buf.Length - i);
            // 十六进制部分
            for (int j = 0; j < lineLen; j++) {
                if (j > 0) sb.Append(' ');
                sb.Append(buf[i + j].ToString("X2"));
            }
            // 不足 16 字节时用空格补齐（每字节占 3 字符，末尾少一个空格）
            int padding = (16 - lineLen) * 3;
            if (lineLen < 16) sb.Append(' ', padding);
            sb.Append("  ");
            // ASCII 可打印字符部分
            for (int j = 0; j < lineLen; j++) {
                byte b = buf[i + j];
                sb.Append(b >= 0x20 && b <= 0x7E ? (char)b : '.');
            }
            Console.WriteLine(sb.ToString());
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // 从 SequenceReader 中读取一个"长度前缀 UTF-8 字段"：
    //   先读 2 字节大端 uint16 作为字符串字节长度，再读对应字节数解码为字符串。
    // 使用 ref 传递 reader 是为了让调用方感知到游标已前进（SequenceReader 是 struct）。
    // ──────────────────────────────────────────────────────────────────────────
    private static bool TryReadToken(ref SequenceReader<byte> reader, out string token) {
        token = string.Empty;
        // stackalloc 在栈上分配 2 字节缓冲，避免堆分配
        Span<byte> lenBuf = stackalloc byte[2];
        // TryCopyTo 将接下来的 2 字节复制到 lenBuf，但不移动游标
        if (!reader.TryCopyTo(lenBuf)) return false;
        reader.Advance(2); // 手动移动游标 2 字节
        // 大端解读为无符号 16 位整数，即字符串的字节长度
        ushort len = BinaryPrimitives.ReadUInt16BigEndian(lenBuf);
        // 剩余字节不足则格式错误
        if (reader.Remaining < len) return false;
        // 从当前游标位置截取 len 个字节（零拷贝）
        ReadOnlySequence<byte> slice = reader.Sequence.Slice(reader.Position, len);
        try {
            // IsSingleSegment 时直接用 Span 解码，避免调用 ToArray() 产生堆分配
            token = slice.IsSingleSegment
                ? StrictUtf8.GetString(slice.First.Span)
                : StrictUtf8.GetString(slice.ToArray()); // 跨多段时必须先合并
            // 我觉得不用 utf-8 解码，也是可以的    
        } catch (DecoderFallbackException) { return false; } // 非法 UTF-8 字节，信封格式不合法
        reader.Advance(len); // 游标前进字符串字节数，指向下一个字段
        return true;
    }
}

// Registers the gRPC method explicitly to avoid framework reflection fallback.
// ──────────────────────────────────────────────────────────────────────────────
// 为什么需要这个类？
//   Grpc.AspNetCore 默认通过反射寻找继承自 proto 生成基类的服务方法。
//   本项目没有 proto 生成代码，因此必须实现 IServiceMethodProvider<T> 手动告知框架：
//   "EchoService 有一个 Unary 方法，路径是 /greet.Greeter/SayHello，
//    用 RawMarshaller 做编解码，调用 EchoService.DispatchAsync 处理请求。"
// ──────────────────────────────────────────────────────────────────────────────
internal sealed class EchoServiceMethodProvider : IServiceMethodProvider<EchoService> {
    // ──────────────────────────────────────────────────────────────────────────
    // 自定义 Marshaller：负责在 gRPC 帧的原始字节 ↔ RawPayload 对象之间转换。
    // Marshaller<T> 构造函数签名：
    //   Action<T, SerializationContext>   serializer   （对象 → 字节，写响应时调用）
    //   Func<DeserializationContext, T>   deserializer （字节 → 对象，读请求时调用）
    // ──────────────────────────────────────────────────────────────────────────
    private static readonly Marshaller<RawPayload> RawMarshaller = new(
        // 序列化（响应路径）：把 RawPayload 里的字节序列逐段写入框架提供的 IBufferWriter<byte>
        static (p, ctx) => {
            var writer = ctx.GetBufferWriter(); // 获取框架管理的输出缓冲，零拷贝写入
            foreach (ReadOnlyMemory<byte> seg in p.Bytes) writer.Write(seg.Span); // 逐段写，不合并
            ctx.Complete(); // 通知框架序列化结束，可以发送
        },
        // 反序列化（请求路径）：从框架缓冲中取出请求字节，包装成 RawPayload
        // PayloadAsNewBuffer() 会分配新数组并拷贝，确保生命周期独立于框架内部缓冲
        static ctx => new RawPayload(new ReadOnlySequence<byte>(ctx.PayloadAsNewBuffer())));

    // ──────────────────────────────────────────────────────────────────────────
    // gRPC 方法描述符：定义了传输层的路由信息和编解码器。
    // Method<TReq, TResp> 参数：
    //   MethodType.Unary        —— 一请求一响应（非流式）
    //   "greet.Greeter"         —— proto service 名（决定 HTTP/2 路径的第一段）
    //   "SayHello"              —— proto method 名（决定 HTTP/2 路径的第二段）
    //   RawMarshaller（×2）     —— 请求和响应共用同一个原始字节编解码器
    // 最终 gRPC 路径为 /greet.Greeter/SayHello，与标准 Greeter proto 兼容，
    // 可以直接用 grpcurl 或标准 Greeter 客户端测试。
    // ──────────────────────────────────────────────────────────────────────────
    private static readonly Method<RawPayload, RawPayload> UnaryMethod = new(
        MethodType.Unary, "greet.Greeter", "SayHello1", RawMarshaller, RawMarshaller);

    // ──────────────────────────────────────────────────────────────────────────
    // 框架在启动时调用此方法发现服务方法。
    // AddUnaryMethod 参数：
    //   UnaryMethod                      —— 上面定义的方法描述符
    //   Array.Empty<object>()            —— 元数据（此处为空，可用于鉴权策略等）
    //   lambda (svc, req, ctx) => ...    —— 实际调用委托，将框架调用转发到 EchoService.DispatchAsync
    // ──────────────────────────────────────────────────────────────────────────
    public void OnServiceMethodDiscovery(ServiceMethodProviderContext<EchoService> context) {
        context.AddUnaryMethod(UnaryMethod, Array.Empty<object>(),
            static (svc, req, ctx) => EchoService.DispatchAsync(req, ctx));
        // 在这里加一条自己的方法
        Method<RawPayload, RawPayload> myMethod = new Method<RawPayload, RawPayload>(
            MethodType.Unary,
            "greet.Greeter",
            "SayHello",
            RawMarshaller,
            RawMarshaller
        );
        context.AddUnaryMethod(
            myMethod,
            Array.Empty<object>(),
            static (svc, req, ctx) =>
            {
                Console.WriteLine("context.AddUnaryMethod");
                // 这里是业务逻辑
                ReadOnlySequence<byte> bytes = req.Bytes;
                string envelopeRoute;
                ReadOnlySequence<byte> envelopePayload;
                if (!EchoService.TryDecodeEnvelope(bytes, out envelopeRoute, out envelopePayload))
                {
                    envelopeRoute = ctx.Method.TrimStart('/');
                    envelopePayload = bytes;
                } 

                // if (!EchoService.TryDecodeEnvelope(bytes, out string envelopeRoute, out ReadOnlySequence<byte> envelopePayload)) {
                //     // 设置 gRPC 状态码，不 throw
                //     Console.WriteLine($"[DumpHex] invalid envelope, raw bytes ({bytes.Length}):");
                //     EchoService.DumpHex(bytes);
                //     ctx.Status = new Status(StatusCode.InvalidArgument, "invalid envelope format");
                //     return Task.FromResult<RawPayload>(new RawPayload(bytes));
                // }
                if (envelopeRoute != "greet.Greeter/SayHello")
                {
                    ctx.Status = new Status(StatusCode.NotFound, "invalid path:"+envelopeRoute);
                    return Task.FromResult<RawPayload>(new RawPayload(bytes));
                }
                // 尝试反序列化
                HelloRequest r = HelloRequest.Parser.ParseFrom(bytes);
                Console.WriteLine("name:" + r.Name);
                HelloReply rsp = new HelloReply();
                rsp.Message = "you said:" + r.Name;
                byte[] rspBytes = rsp.ToByteArray();
                return Task.FromResult(new RawPayload(new ReadOnlySequence<byte>(rspBytes)));
            }
        );
        Console.WriteLine("add method complete");
    }
}

// ──────────────────────────────────────────────────────────────────────────────
// 程序入口：构建 ASP.NET Core 宿主，配置 Kestrel 监听 HTTP/2，注册 gRPC 服务并启动。
// ──────────────────────────────────────────────────────────────────────────────
internal static class Program {
    public static async Task Main(string[] args) {
        // 自行解析 -http2.port=<n>，避免把自定义参数传给 CreateBuilder。
        // ASP.NET Core 配置系统要求单横杠 short switch 必须预先注册映射，否则抛 FormatException。
        int port = 5000;
        foreach (string arg in args) {
            if (arg.StartsWith("-http2.port=", StringComparison.Ordinal) &&
                int.TryParse(arg["-http2.port=".Length..], out int p))
                port = p;
        }

        // 不传 args，避免框架尝试解析不认识的自定义参数
        var builder = WebApplication.CreateBuilder();

        // 配置 Kestrel 只监听 HTTP/2（gRPC 要求 HTTP/2，且通常不与 HTTP/1.1 混用）
        builder.WebHost.ConfigureKestrel(k => k.ListenAnyIP(port, o => o.Protocols = HttpProtocols.Http2));

        // 注册 gRPC 核心服务（拦截器、编解码器管线等）
        builder.Services.AddGrpc();

        // 注册自定义方法提供器，替代框架默认的 proto 反射发现机制。
        // 框架启动时会找到所有 IServiceMethodProvider<EchoService> 实现并调用 OnServiceMethodDiscovery。
        builder.Services.AddSingleton<IServiceMethodProvider<EchoService>, EchoServiceMethodProvider>();

        var app = builder.Build();

        // 将 EchoService 映射为 gRPC 端点；框架会通过上面注册的 provider 找到具体方法
        app.MapGrpcService<EchoService>();

        Console.WriteLine("Listening on 0.0.0.0:5000 (HTTP/2 gRPC).");
        Console.WriteLine("Envelope wire format: [u16 serviceLen][service utf8][u16 methodLen][method utf8][payload].");

        // RunAsync 启动宿主并阻塞到进程退出（Ctrl+C 或 SIGTERM）
        await app.RunAsync().ConfigureAwait(false);
    }
}
