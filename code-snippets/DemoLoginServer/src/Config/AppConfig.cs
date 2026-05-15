namespace DemoLoginServer.Config;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

/// <summary>
/// 应用程序配置，对应当前目录下的 config.yaml。
/// 提示词意图：程序启动时读取 config.yaml，存储到全局可见的 struct。
/// </summary>
public class AppConfig
{
    /// <summary>全局单例配置实例，可在任意位置访问</summary>
    public static AppConfig Instance { get; private set; } = new AppConfig();

    /// <summary>MySQL 数据库配置</summary>
    public MySqlConfig Mysql { get; set; } = new MySqlConfig();

    /// <summary>Redis 连接字符串</summary>
    public string Redis { get; set; } = "";

    /// <summary>
    /// 从指定路径加载 YAML 配置文件，并更新全局单例。
    /// 解析失败时返回非零 <see cref="Common.Error"/>，不抛出异常。
    /// </summary>
    /// <param name="path">配置文件路径，默认为当前目录下的 config.yaml</param>
    /// <returns>成功时返回零值 Error；失败时返回含错误信息的 Error。</returns>
    public static Common.Error Load(string path = "config.yaml")
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(LowerCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
        using var reader = new StreamReader(path);
        var config = deserializer.Deserialize<AppConfig>(reader);
        if (config == null)
            return Common.Error.WithLoc(1, $"Failed to parse config file: {path}");
        Instance = config;
        return default;
    }
}

/// <summary>MySQL 数据库配置项</summary>
public class MySqlConfig
{
    /// <summary>MySQL 连接字符串 (DSN)，包含 server/user/password/database 等信息</summary>
    public string Dsn { get; set; } = "";
}
