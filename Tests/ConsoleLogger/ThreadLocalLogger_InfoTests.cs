using System;
using ConsoleLogger;
using Xunit;

namespace Tests.ConsoleLogger
{
    public class ThreadLocalLogger_InfoTests : TestBase
    {
        [Fact]
        public void Info_AllOverloads_GenerateCorrectOutput()
        {
            var logger = ThreadLocalLogger.Current;
            Logger.SetLevel(LogLevel.Info);
            string file = "testfile.cs";
            string member = "TestMember";
            int line = 123;

            // Test 1 field overload
            var f1 = Field.String("f1"u8, "v1");
            logger.Info(ref f1, file, member, line);
            var output = GetCapturedOutput();
            Assert.Contains("\"f1\":\"v1\"", output);
            Assert.Contains("\"level\":\"info\"", output);
            ClearCapturedOutput();

            // Test 20 fields overload
            var fields = new Field[20];
            for (int i = 0; i < 20; i++) fields[i] = Field.Int64(System.Text.Encoding.UTF8.GetBytes($"f{i+1}"), i+1);

            logger.Info(
                ref fields[0], ref fields[1], ref fields[2], ref fields[3], ref fields[4],
                ref fields[5], ref fields[6], ref fields[7], ref fields[8], ref fields[9],
                ref fields[10], ref fields[11], ref fields[12], ref fields[13], ref fields[14],
                ref fields[15], ref fields[16], ref fields[17], ref fields[18], ref fields[19],
                file, member, line
            );
            
            output = GetCapturedOutput();
            for (int i = 1; i <= 20; i++)
            {
                Assert.Contains($"\"f{i}\":{i}", output);
            }
            ClearCapturedOutput();
        }
    }
}
