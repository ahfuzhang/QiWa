using System;
using System.Threading;
using System.Threading.Tasks;

namespace GrpcGeneicServer;

/// <summary>
/// 命令行应用协调类型，负责把参数解析结果交给服务宿主执行。
/// </summary>
internal sealed class CliApplication {
    /// <summary>
    /// 命令行解析器，用于解析 -http2.port 参数。
    /// </summary>
    private readonly CliOptionsParser _optionsParser;

    /// <summary>
    /// gRPC 服务宿主，用于启动并托管 HTTP/2 服务。
    /// </summary>
    private readonly GrpcServerHost _serverHost;

    /// <summary>
    /// 初始化命令行应用实例。
    /// </summary>
    /// <param name="optionsParser">命令行解析器。</param>
    /// <param name="serverHost">服务宿主。</param>
    public CliApplication(CliOptionsParser optionsParser, GrpcServerHost serverHost) {
        _optionsParser = optionsParser;
        _serverHost = serverHost;
    }

    /// <summary>
    /// 执行应用主流程：解析参数并运行服务。
    /// </summary>
    /// <param name="args">命令行参数。</param>
    /// <param name="cancellationToken">取消信号。</param>
    /// <returns>进程退出码。</returns>
    public async Task<int> RunAsync(string[] args, CancellationToken cancellationToken = default) {
        if (HasHelpFlag(args)) {
            PrintUsage();
            return 0;
        }

        if (!_optionsParser.TryParse(args, out CliOptions? options, out string? errorMessage)) {
            Console.Error.WriteLine(errorMessage);
            PrintUsage();
            return 1;
        }

        return await _serverHost.RunAsync(options!, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 判断命令行中是否请求帮助信息。
    /// </summary>
    /// <param name="args">命令行参数。</param>
    /// <returns>存在帮助参数时返回 true。</returns>
    private static bool HasHelpFlag(string[] args) {
        foreach (string arg in args) {
            if (string.Equals(arg, "-h", StringComparison.Ordinal) || string.Equals(arg, "--help", StringComparison.Ordinal)) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 输出当前程序支持的参数格式。
    /// </summary>
    private static void PrintUsage() {
        Console.WriteLine("Usage: GrpcGeneicServer [-http2.port=8090]");
    }
}
