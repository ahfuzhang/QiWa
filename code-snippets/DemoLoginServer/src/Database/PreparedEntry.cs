namespace DemoLoginServer.Database;

using MySqlConnector;

/// <summary>
/// 对象池中的一个条目，包含预编译语句和其对应的数据库连接。
/// 提示词意图：建立 prepared statement 对象池，避免每次都预编译语句。
/// </summary>
internal sealed class PreparedEntry
{
    /// <summary>与此预编译语句绑定的数据库连接</summary>
    public MySqlConnection Connection { get; init; } = null!;

    /// <summary>查询用户的预编译命令</summary>
    public MySqlCommand SelectUserCommand { get; init; } = null!;
}
