using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;

namespace GrpcEchoServer;

/// <summary>
/// gRPC 请求校验器，负责执行协议与业务路径相关的基础校验。
/// </summary>
internal sealed class GrpcRequestValidator {
    /// <summary>
    /// StreamId 跟踪器，用于校验 stream id 递增规则。
    /// </summary>
    private readonly StreamIdTracker _streamIdTracker;

    /// <summary>
    /// 初始化请求校验器。
    /// </summary>
    /// <param name="streamIdTracker">StreamId 跟踪器。</param>
    public GrpcRequestValidator(StreamIdTracker streamIdTracker) {
        _streamIdTracker = streamIdTracker;
    }

    /// <summary>
    /// 校验请求是否合法。
    /// </summary>
    /// <param name="context">HTTP 上下文。</param>
    /// <param name="errorMessage">校验失败时的错误信息。</param>
    /// <returns>是否合法。</returns>
    public bool TryValidate(HttpContext context, out string errorMessage) {
        HttpRequest request = context.Request;

        // 根据提示词意图：严格按 POST、stream id、content-type、path 的顺序进行 gRPC 请求前置校验。
        if (!string.Equals(request.Method, HttpMethods.Post, StringComparison.Ordinal)) {
            errorMessage = "gRPC request must use POST.";
            return false;
        }

        if (!TryValidateStreamId(context, out errorMessage)) {
            return false;
        }

        if (!HasGrpcContentType(request.ContentType)) {
            errorMessage = "content-type must be application/grpc.";
            return false;
        }

        if (!string.Equals(request.Path.Value, GrpcProtocolConstants.ServicePath, StringComparison.Ordinal)) {
            errorMessage = $"path must be {GrpcProtocolConstants.ServicePath}.";
            return false;
        }

        if (!IsSupportedGrpcEncoding(request.Headers[GrpcProtocolConstants.GrpcEncodingHeader])) {
            errorMessage = "grpc-encoding is unsupported, only identity is allowed.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

    /// <summary>
    /// 校验 HTTP/2 stream id 是否满足提示词要求。
    /// </summary>
    /// <param name="context">HTTP 上下文。</param>
    /// <param name="errorMessage">校验失败时的错误信息。</param>
    /// <returns>是否合法。</returns>
    private bool TryValidateStreamId(HttpContext context, out string errorMessage) {
        var streamIdFeature = context.Features.Get<IHttp2StreamIdFeature>();
        if (streamIdFeature is null) {
            errorMessage = "missing HTTP/2 stream id feature.";
            return false;
        }

        int streamId = streamIdFeature.StreamId;
        bool isValid = _streamIdTracker.ValidateAndUpdate(context.Connection.Id, streamId);
        if (!isValid) {
            errorMessage = "invalid stream id: it must be odd and strictly increasing per connection.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

    /// <summary>
    /// 判断 content-type 是否是 gRPC。
    /// </summary>
    /// <param name="contentType">content-type 头值。</param>
    /// <returns>是否为 gRPC content-type。</returns>
    private static bool HasGrpcContentType(string? contentType) {
        if (string.IsNullOrWhiteSpace(contentType)) {
            return false;
        }

        return contentType.StartsWith(GrpcProtocolConstants.GrpcContentType, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 判断 grpc-encoding 是否可接受。
    /// </summary>
    /// <param name="grpcEncoding">grpc-encoding 头值。</param>
    /// <returns>是否支持。</returns>
    private static bool IsSupportedGrpcEncoding(string? grpcEncoding) {
        if (string.IsNullOrWhiteSpace(grpcEncoding)) {
            return true;
        }

        return string.Equals(grpcEncoding, GrpcProtocolConstants.IdentityEncoding, StringComparison.OrdinalIgnoreCase);
    }
}
