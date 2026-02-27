using System.Threading.Tasks;

namespace GrpcGeneicServer;

/// <summary>
/// 进程入口类型，负责以经典 Main() 形式启动应用。
/// </summary>
internal static class Program {
    /// <summary>
    /// 程序同步入口，满足经典 Main() 形式的要求。
    /// </summary>
    /// <param name="args">命令行参数。</param>
    /// <returns>进程退出码。</returns>
    public static int Main(string[] args) {
        return MainAsync(args).GetAwaiter().GetResult();
    }

    /// <summary>
    /// 程序异步入口，串联命令行解析与 gRPC 服务运行。
    /// </summary>
    /// <param name="args">命令行参数。</param>
    /// <returns>进程退出码。</returns>
    private static async Task<int> MainAsync(string[] args) {
        var application = new CliApplication(new CliOptionsParser(), new GrpcServerHost());
        return await application.RunAsync(args).ConfigureAwait(false);
    }
}
