
# 目标
基于项目 https://github.com/grpc/grpc-dotnet/ 实现一个高性能的简洁的 grpc 服务。
使用这个框架的底层的 api 来做全局的更底层的功能封装，从而绕开以 proto 文件生成代码的模式。

# 约束

参考以下代码:

```csharp
// Program.cs (net8.0+). NuGet: Grpc.AspNetCore, Grpc.Core.Api
using System.Buffers;
using System.Text;
using Grpc.AspNetCore.Server;
using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();

builder.Services.AddSingleton<GlobalRouter>(); // your single “global callback”
var app = builder.Build();

app.MapGrpcService<RawBytesGrpcService>();     // only one service class
app.MapGet("/", () => "gRPC raw-bytes demo. Use a gRPC client.");

app.Run();

/// <summary>
/// One gRPC service endpoint; all methods route to GlobalRouter.
/// The request/response "message type" is ReadOnlySequence<byte> (raw payload bytes).
/// </summary>
sealed class RawBytesGrpcService : BindableService
{
    // Define methods with custom marshaller (no Protobuf decode/encode).
    static readonly Method<ReadOnlySequence<byte>, ReadOnlySequence<byte>> EchoMethod =
        new(MethodType.Unary, "demo.Router", "Echo", RawMarshaller, RawMarshaller);

    static readonly Method<ReadOnlySequence<byte>, ReadOnlySequence<byte>> UpperMethod =
        new(MethodType.Unary, "demo.Router", "Upper", RawMarshaller, RawMarshaller);

    public override void BindService(ServiceBinderBase binder)
    {
        binder.AddMethod(EchoMethod, Dispatch);
        binder.AddMethod(UpperMethod, Dispatch);
    }

    // One global dispatch callback for all methods.
    static async Task<ReadOnlySequence<byte>> Dispatch(
        ReadOnlySequence<byte> request, ServerCallContext ctx)
    {
        var router = ctx.GetHttpContext().RequestServices.GetRequiredService<GlobalRouter>();
        return await router.HandleAsync(request, ctx);
    }

    // Contextual marshaller: gives you raw payload bytes (post-gRPC-framing, pre-Protobuf).
    static readonly Marshaller<ReadOnlySequence<byte>> RawMarshaller =
        new(
            serializer: static (seq, sc) =>
            {
                var writer = sc.GetBufferWriter();
                foreach (var mem in seq)
                {
                    writer.Write(mem.Span);
                }
                sc.Complete();
            },
            deserializer: static dc =>
            {
                // This is the raw message payload bytes (no Protobuf decode).
                return dc.PayloadAsReadOnlySequence();
            });
}

/// <summary>
/// Your single “global callback” + routing based on ctx.Method.
/// ctx.Method is like "/demo.Router/Echo".
/// </summary>
sealed class GlobalRouter
{
    private readonly Dictionary<string, Func<ReadOnlySequence<byte>, ServerCallContext, ValueTask<ReadOnlySequence<byte>>>> _routes;

    public GlobalRouter()
    {
        _routes = new(StringComparer.Ordinal)
        {
            ["/demo.Router/Echo"]  = static (req, _) => new(req), // echo raw bytes
            ["/demo.Router/Upper"] = static (req, _) =>
            {
                // Example "business logic": treat payload as UTF-8 text, uppercase it, return UTF-8 bytes.
                var text = Utf8(req);
                var upper = text.ToUpperInvariant();
                var bytes = Encoding.UTF8.GetBytes(upper);
                return new(new ReadOnlySequence<byte>(bytes));
            }
        };
    }

    public ValueTask<ReadOnlySequence<byte>> HandleAsync(ReadOnlySequence<byte> request, ServerCallContext ctx)
    {
        if (_routes.TryGetValue(ctx.Method, out var fn))
            return fn(request, ctx);

        throw new RpcException(new Status(StatusCode.Unimplemented, $"No route for {ctx.Method}"));
    }

    static string Utf8(ReadOnlySequence<byte> seq)
    {
        if (seq.IsSingleSegment) return Encoding.UTF8.GetString(seq.First.Span);

        // Minimal copy for demo; for production, consider pooled buffers.
        var bytes = seq.ToArray();
        return Encoding.UTF8.GetString(bytes);
    }
}

```

1. 自己做路由：从 request 中读出 service / method 的信息，程序自己决定去调用哪些类；
2. 自己控制请求 body 的 decode
3. 自己控制 response body 的 encode
4. 不依赖于 proto 文件的代码生成机制

# 命令行参数
* `-http2.port=8090`: 设定监听的端口

# 输出
* 在 ./src/ 目录下实现一个简单的 echo 服务器，把请求 body 中未 decode 的 request 作为 encode 后的 response 返回
* 生成 .csproj 文件和 .sln 文件
* 生成 Makefile 文件，提供 build 和 run 两个命令
  - 代码生成后自动调用 make build，确保可以编译通过

