using System;
using ConsoleLogger;
using Xunit;

namespace Tests.ConsoleLogger
{
    public class TaskLogger_DebugTests : TestBase
    {
        [Fact]
        public void Debug_AllOverloads_GenerateCorrectOutput()
        {
            var logger = Logger.Get();
            Logger.SetLevel(LogLevel.Debug);

            // Test 1 field overload
            logger.Debug(Field.String("f1"u8, "v1"));
            var output = GetCapturedOutput();
            Assert.Contains("\"f1\":\"v1\"", output);
            ClearCapturedOutput();

            // Test 20 fields overload (boundary)
            logger.Debug(
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
            Assert.Contains("\"level\":\"debug\"", output);
            ClearCapturedOutput();
            
            // Should verify some intermediate overloads too to ensure wiring is correct?
            // Test 5 fields
            logger.Debug(
                Field.Int64("f1"u8, 1), Field.Int64("f2"u8, 2), Field.Int64("f3"u8, 3), Field.Int64("f4"u8, 4), Field.Int64("f5"u8, 5)
            );
            output = GetCapturedOutput();
            Assert.Contains("\"f5\":5", output);
            ClearCapturedOutput();
            
            Logger.Return(logger);
        }

        [Fact]
        public void Debug_RespectsLogLevel()
        {
            var logger = Logger.Get();
            Logger.SetLevel(LogLevel.Info); // Higher than Debug

            logger.Debug(Field.String("msg"u8, "shoud not appear"));
            
            var output = GetCapturedOutput();
            Assert.Empty(output);
            
            Logger.Return(logger);
        }
    }
}
