namespace DemoLoginServer.Redis;

using StackExchange.Redis;
using DemoLoginServer.Config;

/// <summary>
/// Redis 连接管理器，使用 StackExchange.Redis 的 ConnectionMultiplexer。
/// 提示词意图：使用 StackExchange.Redis，限制每个 redis client 的连接数，全程使用 async api。
/// </summary>
public static class RedisManager
{
    /// <summary>Redis 连接多路复用器，单例共享以限制连接数</summary>
    private static ConnectionMultiplexer? _multiplexer;

    /// <summary>每个端点的最大连接数限制</summary>
    private const int MaxConnectionsPerEndpoint = 5;

    /// <summary>
    /// 初始化 Redis 连接。应在应用启动时调用一次。
    /// </summary>
    public static void Initialize()
    {
        var configOptions = ConfigurationOptions.Parse(AppConfig.Instance.Redis);
        // 限制每个 redis client 的连接数，避免连接过多
        configOptions.SocketManager = new SocketManager(
            "DemoLoginServer",
            workerCount: MaxConnectionsPerEndpoint,
            options: SocketManager.SocketManagerOptions.None);
        configOptions.ConnectRetry = 3;
        configOptions.AbortOnConnectFail = false;
        _multiplexer = ConnectionMultiplexer.Connect(configOptions);
    }

    /// <summary>
    /// 获取 Redis 数据库操作接口。
    /// </summary>
    /// <returns>IDatabase 实例，支持所有 async 操作</returns>
    public static IDatabase GetDatabase()
    {
        if (_multiplexer == null)
            throw new InvalidOperationException("RedisManager not initialized. Call Initialize() first.");
        return _multiplexer.GetDatabase();
    }

    /// <summary>关闭 Redis 连接，应在应用关闭时调用。</summary>
    public static void Shutdown()
    {
        _multiplexer?.Close();
        _multiplexer?.Dispose();
        _multiplexer = null;
    }
}
