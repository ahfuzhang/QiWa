namespace GrpcGeneicServer;

/// <summary>
/// 命令行选项模型，承载 HTTP/2 监听端口配置。
/// </summary>
internal sealed class CliOptions {
    /// <summary>
    /// HTTP/2 服务监听端口。
    /// </summary>
    public int Http2Port { get; }

    /// <summary>
    /// 初始化命令行选项对象。
    /// </summary>
    /// <param name="http2Port">HTTP/2 服务监听端口。</param>
    public CliOptions(int http2Port) {
        Http2Port = http2Port;
    }
}
