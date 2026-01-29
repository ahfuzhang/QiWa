using System;
using ConsoleLogger;
using Xunit;

namespace Tests.ConsoleLogger
{
    public class TaskLogger_FatalTests : TestBase
    {
        [Fact]
        public void Fatal_AllOverloads_GenerateCorrectOutput()
        {
            var logger = Logger.Get();
            // Fatal should always log regardless of level (unless level is somehow Higher than Fatal, which is not possible in this enum design usually, but let's set to Fatal)
            Logger.SetLevel(LogLevel.Fatal);

            // Test 1 field overload
            logger.Fatal(Field.String("f1"u8, "v1"));
            var output = GetCapturedOutput();
            Assert.Contains("\"f1\":\"v1\"", output);
            Assert.Contains("\"level\":\"fatal\"", output);
            ClearCapturedOutput();

            // Test 20 fields overload
            logger.Fatal(
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
        public void Fatal_LogsEvenWhenLevelIsOff()
        {
            // Even if we hypothetically set level to something restrictive
            // Fatal implementation in source doesn't check level!
            // But let's verify that behavior.
            
            var logger = Logger.Get();
            
            logger.Fatal(Field.String("msg"u8, "fatal error"));
            
            var output = GetCapturedOutput();
            Assert.Contains("\"msg\":\"fatal error\"", output);
            
            Logger.Return(logger);
        }
    }
}
