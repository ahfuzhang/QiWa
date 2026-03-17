using ConsoleLogger;
using Xunit;

namespace Tests.ConsoleLogger;

/// <summary>
/// Prompt intent: 为 TaskLogger.WithFields 的全部重载补全测试，只修改测试代码，并覆盖空前缀与已存在前缀两条分支。
/// </summary>
public class TaskLogger_WithFieldsTests : TestBase
{
    /// <summary>
    /// 提供 1 到 20 的字段数量，驱动所有 WithFields 重载覆盖。
    /// </summary>
    public static IEnumerable<object[]> FieldCounts()
    {
        foreach (var fieldCount in Enumerable.Range(1, 20))
        {
            yield return [fieldCount];
        }
    }

    /// <summary>
    /// 意图：覆盖当前文件在空前缀下的全部重载，验证会写入起始大括号、全部字段以及后续日志字段。
    /// </summary>
    [Theory]
    [MemberData(nameof(FieldCounts))]
    public void WithFields_AllOverloads_OnEmptyPrefix_GenerateExpectedPrefix(int fieldCount)
    {
        var rootLogger = Logger.Get();

        try
        {
            var scopedLogger = InvokeWithFields(rootLogger, fieldCount);

            try
            {
                Logger.SetLevel(global::ConsoleLogger.LogLevel.Info);
                scopedLogger.Info(Field.String("msg"u8, "empty-prefix"));

                var output = GetCapturedOutput();
                var normalizedOutput = NormalizeOutput(output);

                AssertContainsFields(output, fieldCount);
                Assert.Contains("\"msg\":\"empty-prefix\"", output);
                Assert.Contains("{\"f1\":1", normalizedOutput);
                Assert.Contains($"\"f{fieldCount}\":{fieldCount},\"msg\":\"empty-prefix\"", normalizedOutput);
            }
            finally
            {
                Logger.Return(scopedLogger);
            }
        }
        finally
        {
            Logger.Return(rootLogger);
        }
    }

    /// <summary>
    /// 意图：覆盖当前文件在已有前缀下的全部重载，验证新增字段会以逗号拼接在父级前缀后面。
    /// </summary>
    [Theory]
    [MemberData(nameof(FieldCounts))]
    public void WithFields_AllOverloads_OnExistingPrefix_AppendAfterParentPrefix(int fieldCount)
    {
        var rootLogger = Logger.Get();

        try
        {
            var parentLogger = rootLogger.WithFields(Field.String("seed"u8, "existing"));

            try
            {
                var scopedLogger = InvokeWithFields(parentLogger, fieldCount);

                try
                {
                    Logger.SetLevel(global::ConsoleLogger.LogLevel.Info);
                    scopedLogger.Info(Field.String("msg"u8, "existing-prefix"));

                    var output = GetCapturedOutput();
                    var normalizedOutput = NormalizeOutput(output);

                    Assert.Contains("\"seed\":\"existing\"", output);
                    AssertContainsFields(output, fieldCount);
                    Assert.Contains("\"msg\":\"existing-prefix\"", output);
                    Assert.Contains("{\"seed\":\"existing\",\"f1\":1", normalizedOutput);
                    Assert.Contains($"\"f{fieldCount}\":{fieldCount},\"msg\":\"existing-prefix\"", normalizedOutput);
                }
                finally
                {
                    Logger.Return(scopedLogger);
                }
            }
            finally
            {
                Logger.Return(parentLogger);
            }
        }
        finally
        {
            Logger.Return(rootLogger);
        }
    }

    /// <summary>
    /// 意图：验证 WithFields 返回新 logger，而不是把前缀直接写回原始 logger。
    /// </summary>
    [Fact]
    public void WithFields_DoesNotMutateOriginalLoggerPrefix()
    {
        var rootLogger = Logger.Get();
        ClearCapturedOutput();

        try
        {
            var scopedLogger = rootLogger.WithFields(Field.String("scope"u8, "child"));

            try
            {
                Logger.SetLevel(global::ConsoleLogger.LogLevel.Info);
                rootLogger.Info(Field.String("msg"u8, "root"));
                scopedLogger.Info(Field.String("msg"u8, "child"));

                var lines = GetCapturedLines();
                var rootLine = lines.Single(line => line.Contains("\"msg\":\"root\"", StringComparison.Ordinal));
                var childLine = lines.Single(line => line.Contains("\"msg\":\"child\"", StringComparison.Ordinal));

                Assert.DoesNotContain("\"scope\":\"child\"", rootLine, StringComparison.Ordinal);
                Assert.Contains("\"scope\":\"child\"", childLine, StringComparison.Ordinal);
            }
            finally
            {
                Logger.Return(scopedLogger);
            }
        }
        finally
        {
            Logger.Return(rootLogger);
        }
    }

    /// <summary>
    /// 根据字段数量显式调用对应的 WithFields 重载，确保当前文件的全部方法都进入覆盖。
    /// </summary>
    private static TaskLogger InvokeWithFields(TaskLogger logger, int fieldCount)
    {
        return fieldCount switch
        {
            1 => logger.WithFields(Field.Int64("f1"u8, 1)),
            2 => logger.WithFields(Field.Int64("f1"u8, 1), Field.Int64("f2"u8, 2)),
            3 => logger.WithFields(Field.Int64("f1"u8, 1), Field.Int64("f2"u8, 2), Field.Int64("f3"u8, 3)),
            4 => logger.WithFields(Field.Int64("f1"u8, 1), Field.Int64("f2"u8, 2), Field.Int64("f3"u8, 3), Field.Int64("f4"u8, 4)),
            5 => logger.WithFields(Field.Int64("f1"u8, 1), Field.Int64("f2"u8, 2), Field.Int64("f3"u8, 3), Field.Int64("f4"u8, 4), Field.Int64("f5"u8, 5)),
            6 => logger.WithFields(Field.Int64("f1"u8, 1), Field.Int64("f2"u8, 2), Field.Int64("f3"u8, 3), Field.Int64("f4"u8, 4), Field.Int64("f5"u8, 5), Field.Int64("f6"u8, 6)),
            7 => logger.WithFields(Field.Int64("f1"u8, 1), Field.Int64("f2"u8, 2), Field.Int64("f3"u8, 3), Field.Int64("f4"u8, 4), Field.Int64("f5"u8, 5), Field.Int64("f6"u8, 6), Field.Int64("f7"u8, 7)),
            8 => logger.WithFields(Field.Int64("f1"u8, 1), Field.Int64("f2"u8, 2), Field.Int64("f3"u8, 3), Field.Int64("f4"u8, 4), Field.Int64("f5"u8, 5), Field.Int64("f6"u8, 6), Field.Int64("f7"u8, 7), Field.Int64("f8"u8, 8)),
            9 => logger.WithFields(Field.Int64("f1"u8, 1), Field.Int64("f2"u8, 2), Field.Int64("f3"u8, 3), Field.Int64("f4"u8, 4), Field.Int64("f5"u8, 5), Field.Int64("f6"u8, 6), Field.Int64("f7"u8, 7), Field.Int64("f8"u8, 8), Field.Int64("f9"u8, 9)),
            10 => logger.WithFields(Field.Int64("f1"u8, 1), Field.Int64("f2"u8, 2), Field.Int64("f3"u8, 3), Field.Int64("f4"u8, 4), Field.Int64("f5"u8, 5), Field.Int64("f6"u8, 6), Field.Int64("f7"u8, 7), Field.Int64("f8"u8, 8), Field.Int64("f9"u8, 9), Field.Int64("f10"u8, 10)),
            11 => logger.WithFields(Field.Int64("f1"u8, 1), Field.Int64("f2"u8, 2), Field.Int64("f3"u8, 3), Field.Int64("f4"u8, 4), Field.Int64("f5"u8, 5), Field.Int64("f6"u8, 6), Field.Int64("f7"u8, 7), Field.Int64("f8"u8, 8), Field.Int64("f9"u8, 9), Field.Int64("f10"u8, 10), Field.Int64("f11"u8, 11)),
            12 => logger.WithFields(Field.Int64("f1"u8, 1), Field.Int64("f2"u8, 2), Field.Int64("f3"u8, 3), Field.Int64("f4"u8, 4), Field.Int64("f5"u8, 5), Field.Int64("f6"u8, 6), Field.Int64("f7"u8, 7), Field.Int64("f8"u8, 8), Field.Int64("f9"u8, 9), Field.Int64("f10"u8, 10), Field.Int64("f11"u8, 11), Field.Int64("f12"u8, 12)),
            13 => logger.WithFields(Field.Int64("f1"u8, 1), Field.Int64("f2"u8, 2), Field.Int64("f3"u8, 3), Field.Int64("f4"u8, 4), Field.Int64("f5"u8, 5), Field.Int64("f6"u8, 6), Field.Int64("f7"u8, 7), Field.Int64("f8"u8, 8), Field.Int64("f9"u8, 9), Field.Int64("f10"u8, 10), Field.Int64("f11"u8, 11), Field.Int64("f12"u8, 12), Field.Int64("f13"u8, 13)),
            14 => logger.WithFields(Field.Int64("f1"u8, 1), Field.Int64("f2"u8, 2), Field.Int64("f3"u8, 3), Field.Int64("f4"u8, 4), Field.Int64("f5"u8, 5), Field.Int64("f6"u8, 6), Field.Int64("f7"u8, 7), Field.Int64("f8"u8, 8), Field.Int64("f9"u8, 9), Field.Int64("f10"u8, 10), Field.Int64("f11"u8, 11), Field.Int64("f12"u8, 12), Field.Int64("f13"u8, 13), Field.Int64("f14"u8, 14)),
            15 => logger.WithFields(Field.Int64("f1"u8, 1), Field.Int64("f2"u8, 2), Field.Int64("f3"u8, 3), Field.Int64("f4"u8, 4), Field.Int64("f5"u8, 5), Field.Int64("f6"u8, 6), Field.Int64("f7"u8, 7), Field.Int64("f8"u8, 8), Field.Int64("f9"u8, 9), Field.Int64("f10"u8, 10), Field.Int64("f11"u8, 11), Field.Int64("f12"u8, 12), Field.Int64("f13"u8, 13), Field.Int64("f14"u8, 14), Field.Int64("f15"u8, 15)),
            16 => logger.WithFields(Field.Int64("f1"u8, 1), Field.Int64("f2"u8, 2), Field.Int64("f3"u8, 3), Field.Int64("f4"u8, 4), Field.Int64("f5"u8, 5), Field.Int64("f6"u8, 6), Field.Int64("f7"u8, 7), Field.Int64("f8"u8, 8), Field.Int64("f9"u8, 9), Field.Int64("f10"u8, 10), Field.Int64("f11"u8, 11), Field.Int64("f12"u8, 12), Field.Int64("f13"u8, 13), Field.Int64("f14"u8, 14), Field.Int64("f15"u8, 15), Field.Int64("f16"u8, 16)),
            17 => logger.WithFields(Field.Int64("f1"u8, 1), Field.Int64("f2"u8, 2), Field.Int64("f3"u8, 3), Field.Int64("f4"u8, 4), Field.Int64("f5"u8, 5), Field.Int64("f6"u8, 6), Field.Int64("f7"u8, 7), Field.Int64("f8"u8, 8), Field.Int64("f9"u8, 9), Field.Int64("f10"u8, 10), Field.Int64("f11"u8, 11), Field.Int64("f12"u8, 12), Field.Int64("f13"u8, 13), Field.Int64("f14"u8, 14), Field.Int64("f15"u8, 15), Field.Int64("f16"u8, 16), Field.Int64("f17"u8, 17)),
            18 => logger.WithFields(Field.Int64("f1"u8, 1), Field.Int64("f2"u8, 2), Field.Int64("f3"u8, 3), Field.Int64("f4"u8, 4), Field.Int64("f5"u8, 5), Field.Int64("f6"u8, 6), Field.Int64("f7"u8, 7), Field.Int64("f8"u8, 8), Field.Int64("f9"u8, 9), Field.Int64("f10"u8, 10), Field.Int64("f11"u8, 11), Field.Int64("f12"u8, 12), Field.Int64("f13"u8, 13), Field.Int64("f14"u8, 14), Field.Int64("f15"u8, 15), Field.Int64("f16"u8, 16), Field.Int64("f17"u8, 17), Field.Int64("f18"u8, 18)),
            19 => logger.WithFields(Field.Int64("f1"u8, 1), Field.Int64("f2"u8, 2), Field.Int64("f3"u8, 3), Field.Int64("f4"u8, 4), Field.Int64("f5"u8, 5), Field.Int64("f6"u8, 6), Field.Int64("f7"u8, 7), Field.Int64("f8"u8, 8), Field.Int64("f9"u8, 9), Field.Int64("f10"u8, 10), Field.Int64("f11"u8, 11), Field.Int64("f12"u8, 12), Field.Int64("f13"u8, 13), Field.Int64("f14"u8, 14), Field.Int64("f15"u8, 15), Field.Int64("f16"u8, 16), Field.Int64("f17"u8, 17), Field.Int64("f18"u8, 18), Field.Int64("f19"u8, 19)),
            20 => logger.WithFields(Field.Int64("f1"u8, 1), Field.Int64("f2"u8, 2), Field.Int64("f3"u8, 3), Field.Int64("f4"u8, 4), Field.Int64("f5"u8, 5), Field.Int64("f6"u8, 6), Field.Int64("f7"u8, 7), Field.Int64("f8"u8, 8), Field.Int64("f9"u8, 9), Field.Int64("f10"u8, 10), Field.Int64("f11"u8, 11), Field.Int64("f12"u8, 12), Field.Int64("f13"u8, 13), Field.Int64("f14"u8, 14), Field.Int64("f15"u8, 15), Field.Int64("f16"u8, 16), Field.Int64("f17"u8, 17), Field.Int64("f18"u8, 18), Field.Int64("f19"u8, 19), Field.Int64("f20"u8, 20)),
            _ => throw new ArgumentOutOfRangeException(nameof(fieldCount), fieldCount, "fieldCount must be between 1 and 20."),
        };
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
    /// 移除输出中的空白字符，便于断言前缀拼接顺序。
    /// </summary>
    private static string NormalizeOutput(string output)
    {
        return output.Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace("\r", string.Empty, StringComparison.Ordinal)
            .Replace("\n", string.Empty, StringComparison.Ordinal);
    }
}
