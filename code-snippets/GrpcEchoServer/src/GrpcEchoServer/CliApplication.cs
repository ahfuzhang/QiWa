using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace GrpcEchoServer;

/// <summary>
/// 命令行应用封装，负责解析参数并启动 gRPC echo 服务。
/// </summary>
internal sealed class CliApplication {
    /// <summary>
    /// gRPC 服务运行器。
    /// </summary>
    private readonly GrpcServer _grpcServer;

    /// <summary>
    /// HTTP/2 监听端口选项。
    /// </summary>
    private readonly Option<int> _http2PortOption;

    /// <summary>
    /// 初始化命令行应用。
    /// </summary>
    /// <param name="grpcServer">gRPC 服务运行器。</param>
    public CliApplication(GrpcServer grpcServer) {
        _grpcServer = grpcServer;
        _http2PortOption = CreateHttp2PortOption();
    }

    /// <summary>
    /// 执行命令行入口逻辑。
    /// </summary>
    /// <param name="args">命令行参数。</param>
    /// <returns>进程退出码。</returns>
    public async Task<int> RunAsync(string[] args) {
        var root = new RootCommand("GrpcEchoServer");
        root.AddOption(_http2PortOption);
        root.SetHandler(async (InvocationContext context) => {
            int port = context.ParseResult.GetValueForOption(_http2PortOption);
            await _grpcServer.RunAsync(port);
        });
        return await root.InvokeAsync(args);
    }

    /// <summary>
    /// 创建 HTTP/2 端口参数定义。
    /// </summary>
    /// <returns>端口参数对象。</returns>
    private static Option<int> CreateHttp2PortOption() {
        var option = new Option<int>(CliOptions.Http2PortShortName, () => CliOptions.DefaultHttp2Port, "HTTP/2 listen port.");
        option.AddAlias(CliOptions.Http2PortLongName);
        option.AddValidator(result => {
            if (result.GetValueOrDefault<int>() <= 0) {
                result.ErrorMessage = "http2.port must be greater than 0.";
            }
        });
        return option;
    }
}
