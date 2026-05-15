namespace Services;

using Common;
using Compress;
using Microsoft.AspNetCore.Http;
using System.Buffers;
using System.IO;
using System.Diagnostics;



public struct QueryUserRequest : IResettable
{
    // 请求用户    
    //todo: 提供序列化和反序列化的能力
    public void Reset()
    {
        // 重置请求数据，例如清空字段、重置状态等
    }

    public Error FromJSON(ReadOnlySpan<byte> json)
    {
        UserId = 123;  // todo: 从 json 中解析出 UserId 字段
        return default;
    }
    public int UserId;
}

public struct QueryUserResponse : IResettable
{
    // 响应用户信息
    public void Reset()
    {
        // 重置响应数据，例如清空字段、重置状态等
    }

    public string UserName;
    public int Age;
    public Error err;

    public void ToJSON(RentedBuffer buffer)
    {
        // todo: 将响应数据序列化为 JSON，写入 buffer 中
    }
}




// 1. 放到内存池中，减少 GC 压力
// 2. 提供 Reset 方法，重置对象状态，避免重复创建新对象
public class QueryUserContext
{
    public QueryUserRequest Request;
    public byte[]? RawRequest;  // 用于存储请求数据的缓冲区，避免每次都分配新的内存
    public int RawRequestLength; // 实际请求数据的长度
    public QueryUserResponse Response;  // Response
    public HttpContext? Http1Context;
    public HttpContext? Http2Context;
    TaskLogger? L;
    // 请求上下文信息，例如请求来源 IP、User-Agent 等
    public void Reset()
    {
        // 重置上下文数据，例如清空字段、重置状态等
        Request.Reset();
        Response.Reset();
        Logger.Return(L!);
        
        //L = null;  // 没必要设置为 null，放回对象池后，肯定不再使用
    }

    private Error Validate(HttpContext httpContext)
    {
        Debug.Assert(httpContext != null);
        Debug.Assert(httpContext.Request != null);
        // 验证请求数据是否合法，例如检查必填字段、字段格式等
        // if (httpContext.Request == null)
        // {
        //     httpContext.Response.StatusCode = 400;
        //     return new Error
        //     {
        //         Code = 400,
        //         Message = "HttpContext cannot be null"
        //     };
        // }
        if (httpContext.Request.Method != HttpMethods.Post)
        {
            httpContext.Response.StatusCode = 405;
            return new Error
            {
                Code = 405,
                Message = "Only POST method is allowed"
            };
        }
        if (httpContext.Request.ContentType != "application/json")
        {
            httpContext.Response.StatusCode = 400;
            return new Error
            {
                Code = 400,
                Message = "not support content type: " + httpContext.Request.ContentType
            };
        }
        if (httpContext.Request.ContentLength == null ||
            httpContext.Request.ContentLength == 0 ||
            httpContext.Request.ContentLength > 1024 * 1024)
        {
            httpContext.Response.StatusCode = 400;
            return new Error
            {
                Code = 400,
                Message = "Content-Length must be greater than 0 and less than 1MB"
            };
        }
        RawRequestLength = (int)httpContext.Request.ContentLength.Value;
        return default;
    }

    public async Task<Error> InitFromHttp1(HttpContext httpContext)
    {
        // 校验请求
        var err = Validate(httpContext);
        if (err.Err())
        {
            return err;
        }
        //
        Http1Context = httpContext;
        // 解析请求
        if (RawRequest == null)
        {
            RawRequest = ArrayPool<byte>.Shared.Rent(RawRequestLength);
        }
        if (RawRequest.Length < RawRequestLength)
        {
            ArrayPool<byte>.Shared.Return(RawRequest);
            RawRequest = ArrayPool<byte>.Shared.Rent(RawRequestLength);
        }
        try
        {
            await httpContext.Request.Body.ReadExactlyAsync(RawRequest, 0, RawRequestLength, httpContext.RequestAborted);
        }
        catch (EndOfStreamException ex)
        {
            httpContext.Response.StatusCode = 400;
            return new Error
            {
                Code = 400,
                Message = "Failed to read request body: " + ex.Message
            };
        }
        catch (OperationCanceledException)
        {
            httpContext.Response.StatusCode = 408;
            return new Error
            {
                Code = 408,
                Message = "Request was canceled"
            };
        }
        // 处理压缩: 读 Content-Encoding，支持 gzip 和 zstd 压缩格式
        var contentEncoding = httpContext.Request.Headers.ContentEncoding.ToString();
        ReadOnlySpan<byte> jsonBytes = RawRequest.AsSpan(0, RawRequestLength);
        RentedBuffer decompressedBuffer = default;
        if (contentEncoding.Equals("gzip", StringComparison.OrdinalIgnoreCase))
        {
            var (gzipBuf, gzipErr) = GzipCompressor.Uncompress(RawRequest.AsSpan(0, RawRequestLength));
            if (gzipErr.Err())
            {
                httpContext.Response.StatusCode = 400;
                return new Error { Code = 400, Message = "Failed to decompress gzip body: " + gzipErr.Message };
            }
            decompressedBuffer = gzipBuf;
            jsonBytes = decompressedBuffer.Bytes();
        }
        else if (contentEncoding.Equals("zstd", StringComparison.OrdinalIgnoreCase))
        {
            var (buf, decompErr) = ZstdCompressor.Uncompress(RawRequest.AsSpan(0, RawRequestLength));
            if (decompErr.Err())
            {
                httpContext.Response.StatusCode = 400;
                return new Error { Code = 400, Message = "Failed to decompress zstd body: " + decompErr.Message };
            }
            decompressedBuffer = buf;
            jsonBytes = decompressedBuffer.Bytes();
        }
        // 解析 JSON 数据到 Request 对象
        // todo: 未来判断是 json 还是 protobuf
        var parseErr = Request.FromJSON(jsonBytes);
        decompressedBuffer.Dispose();
        if (parseErr.Err())
        {
            httpContext.Response.StatusCode = 400;
            return new Error
            {
                Code = 400,
                Message = "Failed to parse JSON: " + parseErr.Message
            };
        }

        // 从 HTTP/1.1 请求中提取数据，初始化 Request 和 Logger
        var tempLogger = Logger.Get();
        L = tempLogger.WithFields(
            Field.String("path"u8, httpContext.Request.Path.Value ?? ""),
            Field.String("method"u8, httpContext.Request.Method),
            Field.String("protocol"u8, httpContext.Request.Protocol),
            Field.String(
                (httpContext.Request.HttpContext.Connection.RemoteIpAddress?.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    ? "client_ipv6"u8 : "client_ipv4"u8,
                httpContext.Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "")
            // todo: request id, client ip
        );
        Logger.Return(tempLogger);
        // todo: metrics 上报
        // todo: tracing 相关的工作
        return default;
    }
}

public class DemoService
{
    public static async ValueTask<Error> QueryUserAsync(QueryUserContext ctx)
    {
        // 处理查询用户的逻辑，例如从数据库获取用户信息
        ref var req = ref ctx.Request;
        ref var rsp = ref ctx.Response;
        // todo:
        if (req.UserId == 0)
        {
            rsp.err = new Error
            {
                Code = 20001,
                Message = "UserId cannot be 0"
            };
            // 业务错误，直接在 body 返回错误信息，HTTP 状态码仍然返回 200
            return default;
        }
        // 模拟查询用户信息的过程
        rsp.UserName = $"User{req.UserId}";
        rsp.Age = 20 + (req.UserId % 10);
        return default;
    }
}

