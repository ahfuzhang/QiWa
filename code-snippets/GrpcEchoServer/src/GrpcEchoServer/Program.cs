using System.Threading.Tasks;

namespace GrpcEchoServer;

/// <summary>
/// 进程入口类型，负责以经典 Main() 形式启动命令行程序。
/// </summary>
internal static class Program {
    /// <summary>
    /// 程序同步入口，满足经典 Main() 入口形式要求。
    /// </summary>
    /// <param name="args">命令行参数。</param>
    /// <returns>进程退出码。</returns>
    public static int Main(string[] args) {
        return MainAsync(args).GetAwaiter().GetResult();
    }

    /// <summary>
    /// 程序异步入口，负责执行命令行解析和服务启动。
    /// </summary>
    /// <param name="args">命令行参数。</param>
    /// <returns>进程退出码。</returns>
    private static async Task<int> MainAsync(string[] args) {
        var application = new CliApplication(new GrpcServer(new GrpcEchoRequestHandler(new GrpcRequestValidator(new StreamIdTracker()))));
        return await application.RunAsync(args);
    }
}
