using Microsoft.AspNetCore.Http;

namespace GrpcEchoServer;

/// <summary>
/// gRPC trailer 写入器，负责输出 grpc-status 与 grpc-message。
/// </summary>
internal static class GrpcTrailerWriter {
    /// <summary>
    /// gRPC 成功状态码。
    /// </summary>
    public const string OkStatus = "0";

    /// <summary>
    /// gRPC 内部错误状态码。
    /// </summary>
    public const string InternalErrorStatus = "13";

    /// <summary>
    /// 声明本次响应会返回 gRPC trailers。
    /// </summary>
    /// <param name="response">HTTP 响应对象。</param>
    public static void DeclareGrpcTrailers(HttpResponse response) {
        if (!response.SupportsTrailers()) {
            return;
        }

        response.DeclareTrailer(GrpcProtocolConstants.GrpcStatusTrailer);
        response.DeclareTrailer(GrpcProtocolConstants.GrpcMessageTrailer);
    }

    /// <summary>
    /// 追加成功 trailer。
    /// </summary>
    /// <param name="response">HTTP 响应对象。</param>
    public static void AppendOk(HttpResponse response) {
        AppendError(response, OkStatus, string.Empty);
    }

    /// <summary>
    /// 追加失败 trailer。
    /// </summary>
    /// <param name="response">HTTP 响应对象。</param>
    /// <param name="status">grpc-status 值。</param>
    /// <param name="message">grpc-message 值。</param>
    public static void AppendError(HttpResponse response, string status, string message) {
        if (response.SupportsTrailers()) {
            response.AppendTrailer(GrpcProtocolConstants.GrpcStatusTrailer, status);
            response.AppendTrailer(GrpcProtocolConstants.GrpcMessageTrailer, message);
            return;
        }

        response.Headers[GrpcProtocolConstants.GrpcStatusTrailer] = status;
        response.Headers[GrpcProtocolConstants.GrpcMessageTrailer] = message;
    }
}
