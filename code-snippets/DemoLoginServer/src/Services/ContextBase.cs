namespace Services;

using Common;
using Compress;
using Microsoft.AspNetCore.Http;
using System.Buffers;
using System.IO;
using System.Diagnostics;

public class ServerConfig
{
    public const int MaxRequestSize = 1024 * 1024; // 1MB
}

// 实现一个 context 的基类
public abstract class ContextBase<TRequest, TResponse>
                            where TRequest : struct, IResettable
                            where TResponse : struct, IResettable
{
    public TRequest Request;
    public TResponse Response;
    public byte[]? RawRequest;  // 用于存储请求数据的缓冲区，避免每次都分配新的内存
    public int RawRequestLength; // 实际请求数据的长度
    public HttpContext? HttpContext;
    TaskLogger? L;

    public void Reset()
    {
        // 重置上下文数据，例如清空字段、重置状态等
        Request.Reset();
        Response.Reset();
        if (L != null)
        {
            Logger.Return(L!);
        }
        L = null;  // 没必要设置为 null，放回对象池后，肯定不再使用
    }

    public Error Validate(HttpContext httpContext)
    {
        Debug.Assert(httpContext != null);
        Debug.Assert(httpContext.Request != null);
        // 验证请求数据是否合法，例如检查必填字段、字段格式等
        if (httpContext.Request.Method != HttpMethods.Post)  // todo: 某些接口可能允许 GET 请求
        {
            // 这一版只支持 post
            httpContext.Response.StatusCode = 405;  // Method Not Allowed
            return Error.WithLoc(code: 405, message: "Only POST method is allowed");
        }
        // todo: 支持更多的 Content-Type，例如 application/protobuf
        if (httpContext.Request.ContentType != "application/json" && httpContext.Request.ContentType != "application/protobuf")
        {
            httpContext.Response.StatusCode = 400;
            return Error.WithLoc(code: 400, message:"not support content type: " + httpContext.Request.ContentType);
        }
        if (httpContext.Request.ContentLength == null ||
            httpContext.Request.ContentLength == 0 ||
            httpContext.Request.ContentLength > ServerConfig.MaxRequestSize)
        {
            httpContext.Response.StatusCode = 400;
            return Error.WithLoc(code: 400, message: $"Content-Length must be greater than 0 and less than {ServerConfig.MaxRequestSize} bytes");
        }
        RawRequestLength = (int)httpContext.Request.ContentLength.Value;
        return default;
    }

    public async Task<Error> InitFromHttp(HttpContext httpContext)
    {
        // 校验请求
        var err = Validate(httpContext);
        if (err.Err())
        {
            return err;
        }
        //
        this.HttpContext = httpContext;
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
            return Error.WithLoc(code: 400, message: "Failed to read request body: " + ex.Message);
        }
        catch (OperationCanceledException)
        {
            httpContext.Response.StatusCode = 408;
            return Error.WithLoc(code: 408, message: "Request was canceled");
        }
        // 处理压缩: 读 Content-Encoding，支持 gzip 和 zstd 压缩格式
        var contentEncoding = httpContext.Request.Headers.ContentEncoding.ToString();
        ReadOnlySpan<byte> reqBytes = RawRequest.AsSpan(0, RawRequestLength);
        RentedBuffer decompressedBuffer = default;
        if (contentEncoding.Contains("gzip", StringComparison.CurrentCulture))
        {
            var (gzipBuf, gzipErr) = GzipCompressor.Uncompress(RawRequest.AsSpan(0, RawRequestLength));
            if (gzipErr.Err())
            {
                httpContext.Response.StatusCode = 400;
                return Error.WithLoc(code: 400, message: "Failed to decompress gzip body: " + gzipErr.Message);
            }
            decompressedBuffer = gzipBuf;
            reqBytes = decompressedBuffer.AsSpan();
        }
        else if (contentEncoding.Contains("zstd", StringComparison.CurrentCulture))
        {
            var (buf, decompErr) = ZstdCompressor.Uncompress(RawRequest.AsSpan(0, RawRequestLength));
            if (decompErr.Err())
            {
                httpContext.Response.StatusCode = 400;
                return Error.WithLoc(code: 400, message: "Failed to decompress zstd body: " + decompErr.Message);
            }
            decompressedBuffer = buf;
            reqBytes = decompressedBuffer.AsSpan();
        }
        // 解析 JSON 数据到 Request 对象
        // todo: 未来判断是 json 还是 protobuf
        var parseErr = Request.FromJSON(reqBytes);
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
