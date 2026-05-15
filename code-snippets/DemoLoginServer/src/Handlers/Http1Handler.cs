namespace DemoLoginServer.Handlers;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.ObjectPool;
using Services;
using Common;

/// <summary>
/// HTTP/1.1 兜底处理器，接收所有未匹配到具体路由的 HTTP/1.1 请求。
/// 提示词意图：所有 http 1.1 的请求，走到一个处理函数中去（兜底 fallback）。
/// </summary>
public static class Http1Handler
{

    // public sealed class QueryUserContextPoolObject
    // {
    //     public QueryUserContext Value;
    // }

    private sealed class QueryUserContextPoolPolicy : IPooledObjectPolicy<QueryUserContext>
    {
        public QueryUserContext Create() => new();
        public bool Return(QueryUserContext obj)
        {
            obj.Reset();
            return true;
        }
    }

    private static readonly ObjectPool<QueryUserContext> _queryUserContextPool =
        new DefaultObjectPool<QueryUserContext>(new QueryUserContextPoolPolicy());

    public static async Task<(QueryUserContext? pooled, Error err)> GetQueryUserContext(HttpContext httpContext)
    {
        var pooled = _queryUserContextPool.Get();
        var err = await pooled.InitFromHttp1(httpContext);
        if (err.Err())
        {
            _queryUserContextPool.Return(pooled);
            return (null, err);
        }
        return (pooled, default);
    }

    public static void PutQueryUserContext(QueryUserContext pooled)
    {
        _queryUserContextPool.Return(pooled);
    }

    /// <summary>
    /// 处理未匹配路由的 HTTP/1.1 请求，记录日志后返回 404。
    /// </summary>
    public static async Task HandleAsync(HttpContext context)
    {
        QueryUserContext? ctx = null;
        Error err;
        try
        {
            (ctx, err) = await GetQueryUserContext(context);
            if (err.Err())
            {
                // 打印日志
                var log = Logger.Get();
                log.Warn(Field.String("path"u8, context.Request.Path.Value ?? ""),
                    Field.String("method"u8, context.Request.Method),
                    Field.String("protocol"u8, context.Request.Protocol),
                    Field.String("error"u8, $"Failed to get QueryUserContext: {err.Message}"));
                Logger.Return(log);
                return;
            }
            // todo: 判断 path
            if (context.Request.Path == "/query_user")
            {
                err = await DemoService.QueryUserAsync(ctx!);
                if (err.Err())
                {
                    // 打印日志
                    var log = Logger.Get();
                    log.Warn(Field.String("path"u8, context.Request.Path.Value ?? ""),
                        Field.String("method"u8, context.Request.Method),
                        Field.String("protocol"u8, context.Request.Protocol),
                        Field.String("error"u8, $"Failed to run DemoService.QueryUserAsync: {err.Message}"));
                    Logger.Return(log);
                    return;
                }
                // 对 response 进行序列化
                // todo: 提供一个方法，对响应的长度进行预估
                RentedBuffer serializedResponse = new(1024);
                ctx!.Response.ToJSON(serializedResponse);
                // 进行压缩
                var (compressedResponse, compressErr) = Compress.ZstdCompressor.Compress(serializedResponse.Bytes());
                serializedResponse.Dispose();
                if (compressErr.Err())
                {
                    var log = Logger.Get();
                    log.Warn(Field.String("path"u8, context.Request.Path.Value ?? ""),
                        Field.String("error"u8, $"Failed to compress response: {compressErr.Message}"));
                    Logger.Return(log);
                    context.Response.StatusCode = 500;
                    return;
                }
                // 写响应
                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json";
                context.Response.Headers.ContentEncoding = "zstd";
                try
                {
                    await context.Response.Body.WriteAsync(compressedResponse.Data!.AsMemory(0, compressedResponse.Length), context.RequestAborted);
                }
                catch (OperationCanceledException ex)
                {
                    var log = Logger.Get();
                    log.Warn(Field.String("path"u8, context.Request.Path.Value ?? ""),
                        Field.String("error"u8, $"OperationCanceledException, Failed to write response: {ex.Message}"));
                    Logger.Return(log);
                    return;
                    //context.Response.StatusCode = 500;
                }
                finally
                {
                    compressedResponse.Dispose();
                }
                // await context.Response.Body.WriteAsync(compressedResponse.Data!.AsMemory(0, compressedResponse.Length), context.RequestAborted);
                // compressedResponse.Dispose();
            }
            else
            {
                context.Response.StatusCode = 404;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"code\":404,\"message\":\"Not Found\"}");
            }
        }
        catch (Exception ex)
        {
            // 打日志
            var log = Logger.Get();
            log.Warn(Field.String("path"u8, context.Request.Path.Value ?? ""),
                Field.String("method"u8, context.Request.Method),
                Field.String("protocol"u8, context.Request.Protocol),
                Field.String("error"u8, $"Exception: {ex.Message}"));
            Logger.Return(log);
            context.Response.StatusCode = 500;
            return;
        }
        finally
        {
            if (ctx != null)
            {
                PutQueryUserContext(ctx);
            }
        }

        // // todo: 代码生成工具，在这里填充对各个 path 的处理函数
        // var log = Logger.Get();
        // try
        // {
        //     log.Info(
        //         Field.String("path"u8, context.Request.Path.Value ?? ""),
        //         Field.String("method"u8, context.Request.Method),
        //         Field.String("protocol"u8, context.Request.Protocol),
        //         Field.String("result"u8, "not_found"));

        //     context.Response.StatusCode = 404;
        //     context.Response.ContentType = "application/json";
        //     await context.Response.WriteAsync("{\"code\":404,\"message\":\"Not Found\"}");
        // }
        // finally
        // {
        //     ConsoleLogger.Logger.Return(log);
        // }
    }
}
