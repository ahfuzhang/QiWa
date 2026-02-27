using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GrpcEchoServer;

/// <summary>
/// gRPC Echo 请求处理器，负责在单个 HTTP/2 回调中完成请求校验、解码和响应。
/// </summary>
internal sealed class GrpcEchoRequestHandler {
    /// <summary>
    /// 根据提示词意图：请求体允许的最大长度（10MB）。
    /// </summary>
    private const int MaxRequestBodyBytes = 10 * 1024 * 1024;

    /// <summary>
    /// 请求体超出长度限制时返回的错误消息。
    /// </summary>
    private const string BodyTooLargeErrorMessage = "request body exceeds 10MB limit.";

    /// <summary>
    /// Content-Length 缺失时返回的错误消息。
    /// </summary>
    private const string MissingContentLengthErrorMessage = "content-length header is required.";

    /// <summary>
    /// Content-Length 非法时返回的错误消息。
    /// </summary>
    private const string InvalidContentLengthErrorMessage = "content-length header is invalid.";

    /// <summary>
    /// 请求校验器。
    /// </summary>
    private readonly GrpcRequestValidator _validator;

    /// <summary>
    /// 初始化请求处理器。
    /// </summary>
    /// <param name="validator">请求校验器。</param>
    public GrpcEchoRequestHandler(GrpcRequestValidator validator) {
        _validator = validator;
    }

    /// <summary>
    /// 处理 HTTP/2 请求。
    /// </summary>
    /// <param name="context">HTTP 上下文。</param>
    public async Task HandleAsync(HttpContext context) {
        // 根据提示词意图：把 HTTP/2 上收到的 gRPC 请求解析流程集中到一个回调函数中，便于清晰展示完整过程。
        if (!_validator.TryValidate(context, out string errorMessage)) {
            await WriteHttp400Async(context, errorMessage);
            return;
        }

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = GrpcProtocolConstants.GrpcContentType;
        // 声明会有 Trailer 的消息
        GrpcTrailerWriter.DeclareGrpcTrailers(context.Response);
        if (!TryGetContentLength(context.Request.ContentLength, out int requestContentLength, out string contentLengthError)) {
            GrpcTrailerWriter.AppendError(context.Response, GrpcTrailerWriter.InternalErrorStatus, contentLengthError);
            return;
        }
        // 根据提示词意图：提前根据 Content-Length 拒绝超过 10MB 的请求体。
        if (IsBodyTooLargeByContentLength(requestContentLength)) {
            // todo: 出错后，没有把 body 读完，会不会出问题？
            //
            // 官方文档明确说明：HTTP/2 下发送响应前不必先把 request body 读完；HTTP/1.1 才需要考虑 drain body 以便复用连接。
            // 来源：https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/request-draining?view=aspnetcore-9.0。
            //
            GrpcTrailerWriter.AppendError(context.Response, GrpcTrailerWriter.ResourceExhaustedStatus, BodyTooLargeErrorMessage);
            return;
        }

        GrpcReadResult decodeResult = await GrpcUnaryFrameCodec.TryReadSingleMessageAsync(context.Request.Body, requestContentLength, context.RequestAborted);
        if (!decodeResult.IsSuccess) {
            GrpcTrailerWriter.AppendError(context.Response, GrpcTrailerWriter.InternalErrorStatus, decodeResult.ErrorMessage);
            return;
        }

        byte[] encoded = GrpcUnaryFrameCodec.EncodeSingleMessage(decodeResult.Payload);
        await context.Response.BodyWriter.WriteAsync(encoded, context.RequestAborted);
        GrpcTrailerWriter.AppendOk(context.Response);
    }

    /// <summary>
    /// 根据 Content-Length 判断请求体是否超限。
    /// </summary>
    /// <param name="contentLength">请求头中的 Content-Length。</param>
    /// <returns>是否超过最大允许长度。</returns>
    private static bool IsBodyTooLargeByContentLength(int contentLength) {
        return contentLength > MaxRequestBodyBytes;
    }

    /// <summary>
    /// 解析并校验 Content-Length。
    /// </summary>
    /// <param name="contentLength">请求头中的 Content-Length。</param>
    /// <param name="requestContentLength">解析后的请求体长度。</param>
    /// <param name="errorMessage">解析失败时的错误消息。</param>
    /// <returns>是否成功解析。</returns>
    private static bool TryGetContentLength(long? contentLength, out int requestContentLength, out string errorMessage) {
        if (!contentLength.HasValue) {
            requestContentLength = 0;
            errorMessage = MissingContentLengthErrorMessage;
            return false;
        }

        if (contentLength.Value < 0 || contentLength.Value > int.MaxValue) {
            requestContentLength = 0;
            errorMessage = InvalidContentLengthErrorMessage;
            return false;
        }

        requestContentLength = (int)contentLength.Value;
        errorMessage = string.Empty;
        return true;
    }

    /// <summary>
    /// 输出 400 错误响应。
    /// </summary>
    /// <param name="context">HTTP 上下文。</param>
    /// <param name="message">错误信息。</param>
    private static async Task WriteHttp400Async(HttpContext context, string message) {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "text/plain; charset=utf-8";
        await context.Response.WriteAsync(message);
    }
}
