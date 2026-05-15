namespace DemoLoginServer.Handlers;

using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using DemoLoginServer.Database;
using DemoLoginServer.Redis;
using DemoLoginServer.Protos;
using StackExchange.Redis;
using ProtoBuf;

/// <summary>
/// 登录接口处理器，处理 POST /login 请求。
/// 提示词意图：查询数据库验证用户名和密码，生成 session_id，写入 redis 并返回给客户端。
/// </summary>
public static class LoginHandler
{
    /// <summary>session 在 Redis 中的过期时间为 30 分钟</summary>
    private static readonly TimeSpan SessionTTL = TimeSpan.FromMinutes(30);

    /// <summary>
    /// 处理登录请求的入口方法。
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

            var user = await DemoLoginServer.Database.UserRepository.FindUserAsync(
                req.UserName, req.UserPasswordSha);

            if (user == null)
            {
                log.Info(
                    ConsoleLogger.Field.String("path"u8, "/login"),
                    ConsoleLogger.Field.String("user_name"u8, req.UserName),
                    ConsoleLogger.Field.String("result"u8, "login_failed"));
                context.Response.StatusCode = 401;
                await WriteResponseAsync(context, new LoginResponse { Code = 401, Message = "Invalid credentials" });
                return;
            }

            var (userId, userName) = user.Value;
            var sessionId = await CreateSessionAsync(userId, userName);

            log.Info(
                ConsoleLogger.Field.String("path"u8, "/login"),
                ConsoleLogger.Field.String("user_name"u8, userName),
                ConsoleLogger.Field.Int64("user_id"u8, userId),
                ConsoleLogger.Field.String("session_id"u8, sessionId),
                ConsoleLogger.Field.String("result"u8, "login_success"));

            context.Response.StatusCode = 200;
            await WriteResponseAsync(context, new LoginResponse { Code = 0, Message = "OK", SessionId = sessionId });
        }
        catch (Exception ex)
        {
            log.Info(
                ConsoleLogger.Field.String("path"u8, "/login"),
                ConsoleLogger.Field.String("error"u8, ex.Message));
            context.Response.StatusCode = 500;
            await WriteResponseAsync(context, new LoginResponse { Code = 500, Message = "Internal error" });
        }
        finally
        {
            ConsoleLogger.Logger.Return(log);
        }
    }

    /// <summary>解析并验证请求体，验证失败则直接写响应并返回 null。</summary>
    private static async Task<LoginRequest?> ParseRequestAsync(HttpContext context)
    {
        LoginRequest? req;
        try
        {
            req = await JsonSerializer.DeserializeAsync(
                context.Request.Body,
                ProtoJsonContext.Default.LoginRequest);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 400;
            await WriteResponseAsync(context, new LoginResponse { Code = 400, Message = "Invalid JSON: " + ex.Message });
            return null;
        }

        if (req == null || string.IsNullOrEmpty(req.UserName) || string.IsNullOrEmpty(req.UserPasswordSha))
        {
            context.Response.StatusCode = 400;
            await WriteResponseAsync(context, new LoginResponse { Code = 400, Message = "Missing required fields" });
            return null;
        }
        return req;
    }

    /// <summary>
    /// 在 Redis 中创建 session，写入两个 key：
    /// $user_id -> {user_name, session_id, last_login_time}
    /// $session_id -> $user_id
    /// </summary>
    private static async Task<string> CreateSessionAsync(long userId, string userName)
    {
        var sessionId = Guid.NewGuid().ToString("N");
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var cacheValue = new UserCacheValue
        {
            UserName = userName,
            SessionId = sessionId,
            LastLoginTime = now
        };

        using var ms = new MemoryStream();
        Serializer.Serialize(ms, cacheValue);
        var bytes = ms.ToArray();

        var db = RedisManager.GetDatabase();
        var tx = db.CreateTransaction();
        _ = tx.StringSetAsync(userId.ToString(System.Globalization.CultureInfo.InvariantCulture), bytes, SessionTTL);
        _ = tx.StringSetAsync(sessionId, userId.ToString(System.Globalization.CultureInfo.InvariantCulture), SessionTTL);
        await tx.ExecuteAsync();

        return sessionId;
    }

    /// <summary>写 JSON 响应体到 HTTP 响应流。</summary>
    private static async Task WriteResponseAsync(HttpContext context, LoginResponse resp)
    {
        context.Response.ContentType = "application/json";
        await JsonSerializer.SerializeAsync(
            context.Response.Body,
            resp,
            ProtoJsonContext.Default.LoginResponse);
    }
}
