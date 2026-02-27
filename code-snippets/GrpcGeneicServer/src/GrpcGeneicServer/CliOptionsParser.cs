using System;
using System.Globalization;

namespace GrpcGeneicServer;

/// <summary>
/// 命令行参数解析器，负责解析提示词要求的 -http2.port 参数。
/// </summary>
internal sealed class CliOptionsParser {
    /// <summary>
    /// 端口参数前缀。
    /// </summary>
    private const string PortPrefix = "-http2.port=";

    /// <summary>
    /// 默认监听端口。
    /// </summary>
    private const int DefaultPort = 8090;

    /// <summary>
    /// 解析命令行参数为选项对象。
    /// </summary>
    /// <param name="args">命令行参数。</param>
    /// <param name="options">解析成功后的配置对象。</param>
    /// <param name="errorMessage">解析失败时的错误信息。</param>
    /// <returns>解析成功返回 true。</returns>
    public bool TryParse(string[] args, out CliOptions? options, out string? errorMessage) {
        int http2Port = DefaultPort;

        foreach (string arg in args) {
            if (!arg.StartsWith(PortPrefix, StringComparison.Ordinal)) {
                options = null;
                errorMessage = $"Unsupported argument: {arg}";
                return false;
            }

            string portText = arg[PortPrefix.Length..];
            if (!int.TryParse(portText, NumberStyles.None, CultureInfo.InvariantCulture, out http2Port)) {
                options = null;
                errorMessage = "The -http2.port value must be an integer.";
                return false;
            }

            if (http2Port is < 1 or > 65535) {
                options = null;
                errorMessage = "The -http2.port value must be between 1 and 65535.";
                return false;
            }
        }

        options = new CliOptions(http2Port);
        errorMessage = null;
        return true;
    }
}
