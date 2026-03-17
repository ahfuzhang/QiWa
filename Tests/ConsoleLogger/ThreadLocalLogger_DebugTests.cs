using ConsoleLogger;
using Xunit;

namespace Tests.ConsoleLogger;

/// <summary>
/// Prompt intent: 用参数化测试覆盖 ThreadLocalLogger.Debug 的全部重载和日志级别分支，提升当前文件的代码覆盖率。
/// </summary>
public class ThreadLocalLogger_DebugTests : TestBase
{
    /// <summary>
    /// 提供 1 到 20 个字段的重载数量，用于逐个覆盖 Debug 的全部重载。
    /// </summary>
    public static IEnumerable<object[]> DebugFieldCounts()
    {
        foreach (var fieldCount in Enumerable.Range(1, 20))
        {
            yield return [fieldCount];
        }
    }

    /// <summary>
    /// 验证每个 Debug 重载都会写出预期字段和调用位置信息。
    /// </summary>
    [Theory]
    [MemberData(nameof(DebugFieldCounts))]
    public void Debug_AllOverloads_GenerateCorrectOutput(int fieldCount)
    {
        var logger = ThreadLocalLogger.Current;
        const string expectedFile = "/tmp/threadlocal-debug-tests.cs";
        const string expectedMember = "Debug_AllOverloads_GenerateCorrectOutput";
        const int expectedLine = 123;

        Logger.SetLevel(global::ConsoleLogger.LogLevel.Debug);

        InvokeDebug(logger, fieldCount, expectedFile, expectedMember, expectedLine);

        var output = GetCapturedOutput();
        Assert.Contains("\"level\":\"debug\"", output);
        Assert.Contains($"\"_file\":\"{expectedFile}\"", output);
        Assert.Contains($"\"_member\":\"{expectedMember}\"", output);
        Assert.Contains($"\"_line\":{expectedLine}", output);
        AssertContainsFields(output, fieldCount);
    }

    /// <summary>
    /// 验证每个 Debug 重载在日志级别低于 Debug 时都会直接返回且不写出内容。
    /// </summary>
    [Theory]
    [MemberData(nameof(DebugFieldCounts))]
    public void Debug_AllOverloads_RespectLogLevel(int fieldCount)
    {
        var logger = ThreadLocalLogger.Current;
        const string expectedFile = "/tmp/threadlocal-debug-skip-tests.cs";
        const string expectedMember = "Debug_AllOverloads_RespectLogLevel";
        const int expectedLine = 456;

        Logger.SetLevel(global::ConsoleLogger.LogLevel.Info);

        InvokeDebug(logger, fieldCount, expectedFile, expectedMember, expectedLine);

        var output = GetCapturedOutput();
        Assert.Empty(output);
    }

    /// <summary>
    /// 根据字段数量显式调用对应的 Debug 重载，确保当前提示词关注的所有重载都进入测试覆盖。
    /// </summary>
    private static void InvokeDebug(ThreadLocalLogger logger, int fieldCount, string file, string member, int line)
    {
        var f1 = Field.Int64("f1"u8, 1);
        var f2 = Field.Int64("f2"u8, 2);
        var f3 = Field.Int64("f3"u8, 3);
        var f4 = Field.Int64("f4"u8, 4);
        var f5 = Field.Int64("f5"u8, 5);
        var f6 = Field.Int64("f6"u8, 6);
        var f7 = Field.Int64("f7"u8, 7);
        var f8 = Field.Int64("f8"u8, 8);
        var f9 = Field.Int64("f9"u8, 9);
        var f10 = Field.Int64("f10"u8, 10);
        var f11 = Field.Int64("f11"u8, 11);
        var f12 = Field.Int64("f12"u8, 12);
        var f13 = Field.Int64("f13"u8, 13);
        var f14 = Field.Int64("f14"u8, 14);
        var f15 = Field.Int64("f15"u8, 15);
        var f16 = Field.Int64("f16"u8, 16);
        var f17 = Field.Int64("f17"u8, 17);
        var f18 = Field.Int64("f18"u8, 18);
        var f19 = Field.Int64("f19"u8, 19);
        var f20 = Field.Int64("f20"u8, 20);

        switch (fieldCount)
        {
            case 1: logger.Debug(f1, file, member, line); return;
            case 2: logger.Debug(f1, f2, file, member, line); return;
            case 3: logger.Debug(f1, f2, f3, file, member, line); return;
            case 4: logger.Debug(f1, f2, f3, f4, file, member, line); return;
            case 5: logger.Debug(f1, f2, f3, f4, f5, file, member, line); return;
            case 6: logger.Debug(f1, f2, f3, f4, f5, f6, file, member, line); return;
            case 7: logger.Debug(f1, f2, f3, f4, f5, f6, f7, file, member, line); return;
            case 8: logger.Debug(f1, f2, f3, f4, f5, f6, f7, f8, file, member, line); return;
            case 9: logger.Debug(f1, f2, f3, f4, f5, f6, f7, f8, f9, file, member, line); return;
            case 10: logger.Debug(f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, file, member, line); return;
            case 11: logger.Debug(f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, file, member, line); return;
            case 12: logger.Debug(f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, file, member, line); return;
            case 13: logger.Debug(f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, f13, file, member, line); return;
            case 14: logger.Debug(f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, f13, f14, file, member, line); return;
            case 15: logger.Debug(f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, f13, f14, f15, file, member, line); return;
            case 16: logger.Debug(f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, f13, f14, f15, f16, file, member, line); return;
            case 17: logger.Debug(f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, f13, f14, f15, f16, f17, file, member, line); return;
            case 18: logger.Debug(f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, f13, f14, f15, f16, f17, f18, file, member, line); return;
            case 19: logger.Debug(f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, f13, f14, f15, f16, f17, f18, f19, file, member, line); return;
            case 20: logger.Debug(f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, f13, f14, f15, f16, f17, f18, f19, f20, file, member, line); return;
            default: throw new ArgumentOutOfRangeException(nameof(fieldCount), fieldCount, "fieldCount must be between 1 and 20.");
        }
    }

    /// <summary>
    /// 断言输出中包含当前重载应该写入的全部字段。
    /// </summary>
    private static void AssertContainsFields(string output, int fieldCount)
    {
        foreach (var index in Enumerable.Range(1, fieldCount))
        {
            Assert.Contains($"\"f{index}\":{index}", output);
        }
    }
}
