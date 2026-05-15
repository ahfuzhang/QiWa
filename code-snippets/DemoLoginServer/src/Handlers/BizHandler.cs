namespace DemoLoginServer.Handlers;

using System.Text.Json;
using Microsoft.AspNetCore.Http;
using DemoLoginServer.Redis;
using DemoLoginServer.Protos;

/// <summary>
/// 业务接口处理器，处理 POST /biz_logic 请求。
/// 提示词意图：验证 session_id 是否存在，鉴权通过后返回整个 request 结构作为业务输出。
/// </summary>
public static class BizHandler
{
    /// <summary>
    /// 处理业务接口请求的入口方法。
    /// </summary>
    public static async Task HandleAsync(HttpContext context)
    {
        var log = ConsoleLogger.Logger.Get();
        try
        {
            var req = await ParseRequestAsync(context);
            if (req == null)
            {
                return;
            }

            // 查询 redis 验证 session_id 是否存在
            var db = RedisManager.GetDatabase();
            var userIdStr = await db.StringGetAsync(req.SessionId);

            if (userIdStr.IsNullOrEmpty)
            {
                log.Info(
                    ConsoleLogger.Field.String("path"u8, "/biz_logic"),
                    ConsoleLogger.Field.String("session_id"u8, req.SessionId),
                    ConsoleLogger.Field.String("result"u8, "session_not_found"));
                context.Response.StatusCode = 401;
                await WriteResponseAsync(context, new BizResponse
                {
                    Code = 401,
                    Message = "Session not found, please login first"
                });
                return;
            }

            log.Info(
                Field.String("path"u8, "/biz_logic"),
                Field.String("session_id"u8, req.SessionId),
                Field.String("action"u8, req.Action),
                Field.String("user_id"u8, userIdStr.ToString()),
                Field.String("result"u8, "auth_success"));

            // 鉴权通过，返回整个 request 结构作为业务输出
            var dataJson = JsonSerializer.Serialize(req, ProtoJsonContext.Default.BizRequest);
            context.Response.StatusCode = 200;
            await WriteResponseAsync(context, new BizResponse
            {
                Code = 0,
                Message = "OK",
                Data = dataJson
            });
        }
        catch (Exception ex)
        {
            log.Info(
                ConsoleLogger.Field.String("path"u8, "/biz_logic"),
                ConsoleLogger.Field.String("error"u8, ex.Message));
            context.Response.StatusCode = 500;
            await WriteResponseAsync(context, new BizResponse { Code = 500, Message = "Internal error" });
        }
        finally
        {
            ConsoleLogger.Logger.Return(log);
        }
    }

    /// <summary>解析并验证请求体，验证失败则直接写响应并返回 null。</summary>
    private static async Task<BizRequest?> ParseRequestAsync(HttpContext context)
    {
        BizRequest? req;
        try
        {
            req = await JsonSerializer.DeserializeAsync(
                context.Request.Body,
                ProtoJsonContext.Default.BizRequest);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 400;
            await WriteResponseAsync(context, new BizResponse { Code = 400, Message = "Invalid JSON: " + ex.Message });
            return null;
        }

        if (req == null || string.IsNullOrEmpty(req.SessionId))
        {
            context.Response.StatusCode = 400;
            await WriteResponseAsync(context, new BizResponse { Code = 400, Message = "Missing session_id" });
            return null;
        }
        return req;
    }

    /// <summary>写 JSON 响应体到 HTTP 响应流。</summary>
    private static async Task WriteResponseAsync(HttpContext context, BizResponse resp)
    {
        context.Response.ContentType = "application/json";
        await JsonSerializer.SerializeAsync(
            context.Response.Body,
            resp,
            ProtoJsonContext.Default.BizResponse);
    }
}
