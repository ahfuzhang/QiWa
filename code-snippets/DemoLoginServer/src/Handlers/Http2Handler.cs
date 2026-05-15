namespace DemoLoginServer.Handlers;

using Microsoft.AspNetCore.Http;

/// <summary>
/// HTTP/2 兜底处理器，接收所有未匹配到具体路由的 HTTP/2 请求。
/// 提示词意图：所有 http 2 的请求，走到一个处理函数中去（兜底 fallback）。
/// </summary>
public static class Http2Handler
{
    /// <summary>
    /// 处理未匹配路由的 HTTP/2 请求，记录日志后返回 404。
    /// </summary>
    public static async Task HandleAsync(HttpContext context)
    {
        var log = ConsoleLogger.Logger.Get();
        try
        {
            log.Info(
                ConsoleLogger.Field.String("path"u8, context.Request.Path.Value ?? ""),
                ConsoleLogger.Field.String("method"u8, context.Request.Method),
                ConsoleLogger.Field.String("protocol"u8, context.Request.Protocol),
                ConsoleLogger.Field.String("result"u8, "not_found"));

            context.Response.StatusCode = 404;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"code\":404,\"message\":\"Not Found\"}");
        }
        finally
        {
            ConsoleLogger.Logger.Return(log);
        }
    }
}
