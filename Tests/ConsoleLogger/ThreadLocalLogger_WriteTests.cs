using System;
using ConsoleLogger;
using Xunit;

namespace Tests.ConsoleLogger
{
    // Checks that the internal write methods work correctly including prefix handling
    public class ThreadLocalLogger_WriteTests : TestBase
    {
         [Fact]
         public void Write_WithPrefix_FormatsCorrectly()
         {
             var logger = ThreadLocalLogger.Current;
             var prefix = System.Text.Encoding.UTF8.GetBytes("\"prefix_key\":\"prefix_val\"");
             var f1 = Field.String("msg"u8, "hello");
             
             // Directly calling internal write1 since tests are in same assembly
             logger.write1(prefix, ref f1, "info", "file.cs", "Member", 100);
             
             var output = GetCapturedOutput();
             // Expect: { "prefix_key":"prefix_val", "msg":"hello", ... }
             // Note: writeN handles adding comma if prefix is not empty
             Assert.Contains("\"prefix_key\":\"prefix_val\"", output);
             Assert.Contains("\"msg\":\"hello\"", output);
             Assert.Contains("\"level\":\"info\"", output);
         }
         
         [Fact]
         public void Write_WithEmptyPrefix_FormatsCorrectly()
         {
             var logger = ThreadLocalLogger.Current;
             var prefix = Array.Empty<byte>();
             var f1 = Field.String("msg"u8, "hello");
             
             logger.write1(prefix, ref f1, "info", "file.cs", "Member", 100);
             
             var output = GetCapturedOutput();
             // Expect: { "msg":"hello", ... } - no leading comma inside
             Assert.Contains("\"msg\":\"hello\"", output);
             Assert.DoesNotContain(",,", output); 
         }
    }
}
