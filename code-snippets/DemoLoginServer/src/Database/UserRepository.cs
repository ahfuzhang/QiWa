namespace DemoLoginServer.Database;

using System.Collections.Concurrent;
using System.Data;
using MySqlConnector;
using DemoLoginServer.Config;

/// <summary>
/// 用户数据库访问层，使用 prepared statement 对象池避免重复编译 SQL 语句。
/// 提示词意图：使用 prepared statement 查询用户，建立对象池避免每次都预编译语句。
/// </summary>
public static class UserRepository
{
    /// <summary>预编译语句对象池</summary>
    private static readonly ConcurrentBag<PreparedEntry> _pool = new();

    /// <summary>查询用户的 SQL，使用参数化查询防止 SQL 注入</summary>
    private const string SelectUserSql =
        "SELECT user_id, user_name FROM users " +
        "WHERE user_name = @user_name AND user_password_sha = @user_password_sha LIMIT 1";

    /// <summary>
    /// 从对象池获取可用的预编译条目，如无可用条目则创建新的。
    /// </summary>
    private static async Task<PreparedEntry> GetEntryAsync()
    {
        while (_pool.TryTake(out var entry))
        {
            // 验证连接仍然有效
            if (entry.Connection.State == ConnectionState.Open)
            {
                return entry;
            }
            // 连接已关闭，丢弃此条目
            await entry.Connection.DisposeAsync();
        }

        // 创建新的连接和预编译命令
        var conn = new MySqlConnection(AppConfig.Instance.Mysql.Dsn);
        await conn.OpenAsync();
        var cmd = new MySqlCommand(SelectUserSql, conn);
        cmd.Parameters.Add("@user_name", MySqlDbType.VarChar, 64);
        cmd.Parameters.Add("@user_password_sha", MySqlDbType.VarChar, 64);
        await cmd.PrepareAsync();
        return new PreparedEntry { Connection = conn, SelectUserCommand = cmd };
    }

    /// <summary>
    /// 根据用户名和密码 SHA256 哈希查询用户。
    /// 全程使用 async api，避免阻塞线程。
    /// </summary>
    /// <param name="userName">用户名</param>
    /// <param name="passwordSha">密码的 SHA256 哈希</param>
    /// <returns>找到则返回 (userId, userName)，否则返回 null</returns>
    public static async Task<(long userId, string userName)?> FindUserAsync(string userName, string passwordSha)
    {
        var entry = await GetEntryAsync();
        try
        {
            entry.SelectUserCommand.Parameters["@user_name"].Value = userName;
            entry.SelectUserCommand.Parameters["@user_password_sha"].Value = passwordSha;

            await using var reader = await entry.SelectUserCommand.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return (reader.GetInt64(0), reader.GetString(1));
            }
            return null;
        }
        finally
        {
            // 将条目归还到对象池
            _pool.Add(entry);
        }
    }
}
