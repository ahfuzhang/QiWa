using ConsoleLogger;
using Xunit;

namespace Tests.ConsoleLogger;

/// <summary>
/// Prompt intent: 用测试覆盖 TaskLogger.Error 的全部重载和调用者信息，避免当前文件没有测试导致覆盖率缺失。
/// </summary>
public class TaskLogger_ErrorTests : TestBase
{
    /// <summary>
    /// 提供 1 到 20 个字段数量，用于逐个覆盖 Error 的全部重载。
    /// </summary>
    public static IEnumerable<object[]> ErrorFieldCounts()
    {
        foreach (var fieldCount in Enumerable.Range(1, 20))
        {
            yield return [fieldCount];
        }
    }

    /// <summary>
    /// 验证每个 Error 重载都会输出对应字段和调用位置信息。
    /// </summary>
    [Theory]
    [MemberData(nameof(ErrorFieldCounts))]
    public void Error_AllOverloads_GenerateCorrectOutput(int fieldCount)
    {
        var logger = Logger.Get();
        const string expectedFile = "/tmp/tasklogger-error-tests.cs";
        const string expectedMember = "Error_AllOverloads_GenerateCorrectOutput";
        const int expectedLine = 789;

        try
        {
            Logger.SetLevel(global::ConsoleLogger.LogLevel.Error);

            InvokeError(logger, fieldCount, expectedFile, expectedMember, expectedLine);

            var output = GetCapturedOutput();
            Assert.Contains("\"level\":\"error\"", output);
            Assert.Contains($"\"_file\":\"{expectedFile}\"", output);
            Assert.Contains($"\"_member\":\"{expectedMember}\"", output);
            Assert.Contains($"\"_line\":{expectedLine}", output);
            AssertContainsFields(output, fieldCount);
        }
        finally
        {
            Logger.Return(logger);
        }
    }

    /// <summary>
    /// 验证每个 Error 重载在日志级别高于 Error 时都会直接返回且不输出内容。
    /// </summary>
    [Theory]
    [MemberData(nameof(ErrorFieldCounts))]
    public void Error_AllOverloads_RespectLogLevel(int fieldCount)
    {
        var logger = Logger.Get();
        const string expectedFile = "/tmp/tasklogger-error-skip-tests.cs";
        const string expectedMember = "Error_AllOverloads_RespectLogLevel";
        const int expectedLine = 456;

        try
        {
            Logger.SetLevel(global::ConsoleLogger.LogLevel.Fatal);

            InvokeError(logger, fieldCount, expectedFile, expectedMember, expectedLine);

            var output = GetCapturedOutput();
            Assert.Empty(output);
        }
        finally
        {
            Logger.Return(logger);
        }
    }

    /// <summary>
    /// 验证未显式传入参数时，Error 会保留编译器生成的调用者信息。
    /// </summary>
    [Fact]
    public void Error_UsesCompilerProvidedCallerInfo()
    {
        var logger = Logger.Get();
        var expectedFile = GetCurrentSourceFile();
        const string expectedMessage = "error message";

        try
        {
            Logger.SetLevel(global::ConsoleLogger.LogLevel.Error);
            var expectedLine = GetNextSourceLine();
            logger.Error(Field.String("msg"u8, expectedMessage));

            var output = GetCapturedOutput();
            Assert.Contains($"\"_file\":\"{expectedFile}\"", output);
            Assert.Contains($"\"_member\":\"{nameof(Error_UsesCompilerProvidedCallerInfo)}\"", output);
            Assert.Contains($"\"_line\":{expectedLine}", output);
            Assert.Contains($"\"msg\":\"{expectedMessage}\"", output);
            Assert.Contains("\"level\":\"error\"", output);
        }
        finally
        {
            Logger.Return(logger);
        }
    }

    /// <summary>
    /// 根据字段数量显式调用对应的 Error 重载，确保当前提示词关注的所有重载都进入测试覆盖。
    /// </summary>
    private static void InvokeError(TaskLogger logger, int fieldCount, string file, string member, int line)
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
            case 1: logger.Error(f1, file, member, line); return;
            case 2: logger.Error(f1, f2, file, member, line); return;
            case 3: logger.Error(f1, f2, f3, file, member, line); return;
            case 4: logger.Error(f1, f2, f3, f4, file, member, line); return;
            case 5: logger.Error(f1, f2, f3, f4, f5, file, member, line); return;
            case 6: logger.Error(f1, f2, f3, f4, f5, f6, file, member, line); return;
            case 7: logger.Error(f1, f2, f3, f4, f5, f6, f7, file, member, line); return;
            case 8: logger.Error(f1, f2, f3, f4, f5, f6, f7, f8, file, member, line); return;
            case 9: logger.Error(f1, f2, f3, f4, f5, f6, f7, f8, f9, file, member, line); return;
            case 10: logger.Error(f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, file, member, line); return;
            case 11: logger.Error(f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, file, member, line); return;
            case 12: logger.Error(f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, file, member, line); return;
            case 13: logger.Error(f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, f13, file, member, line); return;
            case 14: logger.Error(f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, f13, f14, file, member, line); return;
            case 15: logger.Error(f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, f13, f14, f15, file, member, line); return;
            case 16: logger.Error(f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, f13, f14, f15, f16, file, member, line); return;
            case 17: logger.Error(f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, f13, f14, f15, f16, f17, file, member, line); return;
            case 18: logger.Error(f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, f13, f14, f15, f16, f17, f18, file, member, line); return;
            case 19: logger.Error(f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, f13, f14, f15, f16, f17, f18, f19, file, member, line); return;
            case 20: logger.Error(f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, f13, f14, f15, f16, f17, f18, f19, f20, file, member, line); return;
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

    /// <summary>
    /// 返回当前源码文件路径，便于断言 CallerFilePath 的结果。
    /// </summary>
    private static string GetCurrentSourceFile([System.Runtime.CompilerServices.CallerFilePath] string file = "")
    {
        return file;
    }

    /// <summary>
    /// 返回下一行源码行号，便于断言紧随其后的日志调用写入了正确的 CallerLineNumber。
    /// </summary>
    private static int GetNextSourceLine([System.Runtime.CompilerServices.CallerLineNumber] int line = 0)
    {
        return line + 1;
    }
}
