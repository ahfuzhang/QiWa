using System;
using ConsoleLogger;
using Xunit;

namespace Tests.ConsoleLogger
{
    public class TaskLogger_InfoTests : TestBase
    {
        [Fact]
        public void Info_AllOverloads_GenerateCorrectOutput()
        {
            var logger = Logger.Get();
            Logger.SetLevel(LogLevel.Info);

            // Test 1 field overload
            logger.Info(Field.String("f1"u8, "v1"));
            var output = GetCapturedOutput();
            Assert.Contains("\"f1\":\"v1\"", output);
            Assert.Contains("\"level\":\"info\"", output);
            ClearCapturedOutput();

            // Test 20 fields overload
            logger.Info(
                Field.Int64("f1"u8, 1), Field.Int64("f2"u8, 2), Field.Int64("f3"u8, 3), Field.Int64("f4"u8, 4), Field.Int64("f5"u8, 5),
                Field.Int64("f6"u8, 6), Field.Int64("f7"u8, 7), Field.Int64("f8"u8, 8), Field.Int64("f9"u8, 9), Field.Int64("f10"u8, 10),
                Field.Int64("f11"u8, 11), Field.Int64("f12"u8, 12), Field.Int64("f13"u8, 13), Field.Int64("f14"u8, 14), Field.Int64("f15"u8, 15),
                Field.Int64("f16"u8, 16), Field.Int64("f17"u8, 17), Field.Int64("f18"u8, 18), Field.Int64("f19"u8, 19), Field.Int64("f20"u8, 20)
            );
            output = GetCapturedOutput();
            for (int i = 1; i <= 20; i++)
            {
                Assert.Contains($"\"f{i}\":{i}", output);
            }
            ClearCapturedOutput();
            
            Logger.Return(logger);
        }

        [Fact]
        public void Info_RespectsLogLevel()
        {
            var logger = Logger.Get();
            Logger.SetLevel(LogLevel.Warn); // Higher than Info

            logger.Info(Field.String("msg"u8, "should not appear"));
            
            var output = GetCapturedOutput();
            Assert.Empty(output);
            
            Logger.Return(logger);
        }
    }
}
