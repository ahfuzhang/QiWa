using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GrpcEchoServer;

/// <summary>
/// gRPC Echo 请求处理器，负责在单个 HTTP/2 回调中完成请求校验、解码和响应。
/// </summary>
internal sealed class GrpcEchoRequestHandler {
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
        GrpcTrailerWriter.DeclareGrpcTrailers(context.Response);

        GrpcReadResult decodeResult = await GrpcUnaryFrameCodec.TryReadSingleMessageAsync(context.Request.Body, context.RequestAborted);
        if (!decodeResult.IsSuccess) {
            GrpcTrailerWriter.AppendError(context.Response, GrpcTrailerWriter.InternalErrorStatus, decodeResult.ErrorMessage);
            return;
        }

        byte[] encoded = GrpcUnaryFrameCodec.EncodeSingleMessage(decodeResult.Payload);
        await context.Response.BodyWriter.WriteAsync(encoded, context.RequestAborted);
        GrpcTrailerWriter.AppendOk(context.Response);
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
