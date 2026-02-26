namespace GrpcEchoServer;

/// <summary>
/// 命令行参数常量集合，用于统一维护参数名和默认值。
/// </summary>
internal static class CliOptions {
    /// <summary>
    /// HTTP/2 监听端口默认值。
    /// </summary>
    public const int DefaultHttp2Port = 8090;

    /// <summary>
    /// HTTP/2 监听端口短参数名。
    /// </summary>
    public const string Http2PortShortName = "-http2.port";

    /// <summary>
    /// HTTP/2 监听端口长参数名。
    /// </summary>
    public const string Http2PortLongName = "--http2.port";
}
