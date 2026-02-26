namespace GrpcEchoServer;

/// <summary>
/// gRPC 协议常量集合，用于统一管理关键字符串。
/// </summary>
internal static class GrpcProtocolConstants {
    /// <summary>
    /// 实验服务路径。
    /// </summary>
    public const string ServicePath = "/my_service/my_method";

    /// <summary>
    /// gRPC 标准 content-type。
    /// </summary>
    public const string GrpcContentType = "application/grpc";

    /// <summary>
    /// gRPC 编码头名称。
    /// </summary>
    public const string GrpcEncodingHeader = "grpc-encoding";

    /// <summary>
    /// gRPC 状态 trailer 名称。
    /// </summary>
    public const string GrpcStatusTrailer = "grpc-status";

    /// <summary>
    /// gRPC 消息 trailer 名称。
    /// </summary>
    public const string GrpcMessageTrailer = "grpc-message";

    /// <summary>
    /// 身份编码名称，表示未压缩。
    /// </summary>
    public const string IdentityEncoding = "identity";
}
