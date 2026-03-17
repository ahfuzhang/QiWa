using System;
using System.Collections.Generic;
using System.Text;
using Log;
using Xunit;
using LogLevel = global::Log.LogLevel;

namespace Tests.Log;

/// <summary>
/// 意图：补上 Logger.Init 中 tags 条件分支的测试覆盖，避免选中代码缺少用例。
/// </summary>
[Collection("LoggerTests")]
public class LoggerInitTagsTests : IDisposable
{
    /// <summary>
    /// 在每个测试开始前清理 Logger 单例，避免全局状态互相污染。
    /// </summary>
    public LoggerInitTagsTests()
    {
        EnsureLoggerStopped();
    }

    /// <summary>
    /// 在每个测试结束后释放 Logger 资源，保持后续测试环境干净。
    /// </summary>
    public void Dispose()
    {
        EnsureLoggerStopped();
    }

    /// <summary>
    /// 意图：验证 tags 为 null 时不会进入 SetGlobalTags 分支。
    /// </summary>
    [Fact]
    public void Init_WithNullTags_LeavesTagPrefixEmpty()
    {
        Logger.Init(level: LogLevel.Info, tags: null, overload: OverloadPolicy.Direct);

        Assert.NotNull(Logger.Instance);
        Assert.Empty(Logger.Instance.TagPrefix);
    }

    /// <summary>
    /// 意图：验证 tags 为空字典时不会进入 SetGlobalTags 分支。
    /// </summary>
    [Fact]
    public void Init_WithEmptyTags_LeavesTagPrefixEmpty()
    {
        Logger.Init(
            level: LogLevel.Info,
            tags: new Dictionary<string, string>(),
            overload: OverloadPolicy.Direct);

        Assert.NotNull(Logger.Instance);
        Assert.Empty(Logger.Instance.TagPrefix);
    }

    /// <summary>
    /// 意图：验证 tags 非空时会序列化全局标签前缀，覆盖选中的条件分支。
    /// </summary>
    [Fact]
    public void Init_WithNonEmptyTags_SerializesTagPrefix()
    {
        var tags = new Dictionary<string, string>
        {
            ["service"] = "qiwa",
            ["env"] = "test",
        };

        Logger.Init(level: LogLevel.Info, tags: tags, overload: OverloadPolicy.Direct);

        Assert.NotNull(Logger.Instance);
        Assert.NotEmpty(Logger.Instance.TagPrefix);

        var tagPrefix = Encoding.UTF8.GetString(Logger.Instance.TagPrefix);
        Assert.StartsWith("{", tagPrefix, StringComparison.Ordinal);
        Assert.Contains("\"service\":\"qiwa\"", tagPrefix, StringComparison.Ordinal);
        Assert.Contains("\"env\":\"test\"", tagPrefix, StringComparison.Ordinal);
    }

    /// <summary>
    /// 关闭当前 Logger 单例，避免全局资源影响其他测试。
    /// </summary>
    private static void EnsureLoggerStopped()
    {
        if (Logger.Instance == null)
        {
            return;
        }

        try
        {
            Logger.Shutdown();
        }
        catch (ObjectDisposedException)
        {
        }
    }
}
