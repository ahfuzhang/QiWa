using ConsoleLogger;
using Xunit;

namespace Tests.ConsoleLogger;

public class TaskLoggerTests : TestBase
{
    [Fact]
    public void WithFields_AppendsFieldsCorrectly()
    {
        var rootLogger = Logger.Get();
        Assert.NotNull(rootLogger);

        // 1. Initial WithFields call
        var logger = rootLogger.WithFields(Field.String("key1"u8, "value1"));
        Logger.Return(rootLogger);

        // 2. Chained WithFields call
        var nextLogger = logger.WithFields(Field.Int64("key2"u8, 2));
        Logger.Return(logger);
        logger = nextLogger;

        // Log a message to flush everything
        Logger.SetLevel(global::ConsoleLogger.LogLevel.Info);
        logger.Info(Field.String("msg"u8, "final"));

        Logger.Return(logger);

        // Output should look something like:
        // ..., "key1":"value1", "key2":2, "msg":"final", ...
        var output = GetCapturedOutput();
        Assert.Contains("\"key1\":\"value1\"", output);
        Assert.Contains("\"key2\":2", output);
        Assert.Contains("\"msg\":\"final\"", output);
    }

    [Fact]
    public void WithFields_MultipleOverloads_WorkCorrectly()
    {
        var rootLogger = Logger.Get();

        // Test a high-arity overload (e.g., 3 fields)
        var logger = rootLogger.WithFields(
            Field.String("f1"u8, "v1"),
            Field.String("f2"u8, "v2"),
            Field.String("f3"u8, "v3")
        );
        Logger.Return(rootLogger);

        Logger.SetLevel(global::ConsoleLogger.LogLevel.Info);
        logger.Info(Field.String("msg"u8, "done"));
        Logger.Return(logger);

        var output = GetCapturedOutput();
        Assert.Contains("\"f1\":\"v1\"", output);
        Assert.Contains("\"f2\":\"v2\"", output);
        Assert.Contains("\"f3\":\"v3\"", output);
    }

    [Fact]
    public void WithFields_AccumulatesPrefixProperly()
    {
        var rootLogger = Logger.Get();

        // First batch
        var logger = rootLogger.WithFields(Field.String("batch1"u8, "ok"));
        Logger.Return(rootLogger);

        // Second batch
        var nextLogger = logger.WithFields(Field.String("batch2"u8, "ok"));
        Logger.Return(logger);
        logger = nextLogger;

        Logger.SetLevel(global::ConsoleLogger.LogLevel.Info);
        logger.Info(Field.String("msg"u8, "test"));
        Logger.Return(logger);

        var output = GetCapturedOutput();
        // Just ensuring the comma separation logic works implicitly by invalid JSON check if failed
        // The format is usually "batch1":"ok","batch2":"ok"... within the JSON object
        Assert.Contains("\"batch1\":\"ok\",\"batch2\":\"ok\"", output.Replace(" ", ""));
    }

    // This test calls the large overload of WithFields(20 fields)
    [Fact]
    public void WithFields_MaxOverload_Works()
    {
        var rootLogger = Logger.Get();

        var logger = rootLogger.WithFields(
            Field.Int64("f1"u8, 1), Field.Int64("f2"u8, 2), Field.Int64("f3"u8, 3), Field.Int64("f4"u8, 4), Field.Int64("f5"u8, 5),
            Field.Int64("f6"u8, 6), Field.Int64("f7"u8, 7), Field.Int64("f8"u8, 8), Field.Int64("f9"u8, 9), Field.Int64("f10"u8, 10),
            Field.Int64("f11"u8, 11), Field.Int64("f12"u8, 12), Field.Int64("f13"u8, 13), Field.Int64("f14"u8, 14), Field.Int64("f15"u8, 15),
            Field.Int64("f16"u8, 16), Field.Int64("f17"u8, 17), Field.Int64("f18"u8, 18), Field.Int64("f19"u8, 19), Field.Int64("f20"u8, 20)
        );
        Logger.Return(rootLogger);

        Logger.SetLevel(global::ConsoleLogger.LogLevel.Info);
        logger.Info(Field.String("msg"u8, "max"));
        Logger.Return(logger);

        var output = GetCapturedOutput();
        for (int i = 1; i <= 20; i++)
        {
            Assert.Contains($"\"f{i}\":{i}", output);
        }
    }
}
