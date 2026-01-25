using Xunit;
using Log;
using System.Text;

public class LoggerTests {
    [Fact]
    public void SetLevel_SetFlushIntervalMs_Shutdown_CanBeCalledInOrder() {
        Logger.SetLevel(Log.LogLevel.Debug);
        Assert.Equal(Log.LogLevel.Debug, Logger.Instance.Level);
        Logger.SetFlushIntervalMs(5);
        Logger.SetFlushIntervalMs(1000);
        Assert.Equal(1000, Logger.Instance.flushIntervalMs);
        //
        TaskLogger logger = new TaskLogger();
        logger = logger.WithFields(Field.String("pod"u8, "qiwa-xxx-123456"));
        logger.Info(Field.String("xxx"u8, "abcdefg"));
        //
        Thread.Sleep(2000);
        //Logger.Shutdown();
    }

    [Fact]
    public void logger_with_tags() {
        Logger.SetLevel(Log.LogLevel.Debug);
        Assert.Equal(Log.LogLevel.Debug, Logger.Instance.Level);
        Logger.SetFlushIntervalMs(5);
        Logger.SetFlushIntervalMs(1000);
        Assert.Equal(1000, Logger.Instance.flushIntervalMs);
        Logger.SetGlobalTags(new Dictionary<string, string> { { "pod", "qiwa-12345678" }, { "namespace", "backend-team" } });
        Assert.Equal("{\"pod\":\"qiwa-12345678\",\"namespace\":\"backend-team\"", Encoding.UTF8.GetString(Logger.Instance.TagPrefix));
        //
        TaskLogger logger = new TaskLogger();
        logger = logger.WithFields(Field.String("biz_field1"u8, "qiwa-xxx-123456"));
        logger.Info(Field.String("xxx_field2"u8, "abcdefg"));
        // 测试协程
        Task t = Task.Run(async () => {
            await Task.Delay(1000);
            logger.Info(Field.String("task"u8, "task 1"));
        });
        //
        Thread.Sleep(2000);
        Task.WhenAll(t);
        //
        Logger.Shutdown();
    }
}
