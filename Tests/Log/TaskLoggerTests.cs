using System;
using System.Text;
using System.Text.Json;
using Log;
using Xunit;
using LogLevel = global::Log.LogLevel;

namespace Tests.Log {
    /// <summary>
    /// Tests for TaskLogger.cs
    /// Note: These tests depend on Logger singleton initialization
    /// </summary>
    [Collection("LoggerTests")]
    public class TaskLoggerTests : IDisposable {
        public TaskLoggerTests() {
            // Ensure Logger is initialized
            if (Logger.Instance != null) {
                try { Logger.Shutdown(); } catch { }
            }
            Logger.Init(level: LogLevel.Debug, flushIntervalMs: 10000);
        }

        public void Dispose() {
            if (Logger.Instance != null) {
                try { Logger.Shutdown(); } catch { }
            }
        }

        #region TestCase Structures
        public struct LogLevelTestCase {
            public string Name;
            public global::Log.LogLevel GlobalLevel;
            public string MethodToCall;
            public bool ShouldLog;
        }
        #endregion

        [Fact]
        public void Constructor_WithNoGlobalTags_InitializesProperly() {
            var logger = new TaskLogger();

            Assert.NotNull(logger);
        }

        [Fact]
        public void Constructor_WithGlobalTags_IncludesTagsInPrefix() {
            // Shutdown and reinitialize with tags
            Logger.Shutdown();
            var tags = new System.Collections.Generic.Dictionary<string, string> {
                { "service", "test-service" },
                { "version", "1.0.0" }
            };
            Logger.Init(level: LogLevel.Debug, tags: tags);

            var logger = new TaskLogger();

            Assert.NotNull(logger);
        }

        [Fact]
        public void WithFields_SingleField_ReturnsNewLoggerWithField() {
            var logger = new TaskLogger();
            var field = Field.String("key"u8.ToArray(), "value");

            var newLogger = logger.WithFields(field);

            Assert.NotNull(newLogger);
            Assert.NotSame(logger, newLogger);
        }

        [Fact]
        public void WithFields_MultipleOverloads_ReturnNewLoggers() {
            var logger = new TaskLogger();

            // Test 2-field overload
            var logger2 = logger.WithFields(
                Field.String("k1"u8.ToArray(), "v1"),
                Field.String("k2"u8.ToArray(), "v2")
            );
            Assert.NotNull(logger2);
            Assert.NotSame(logger, logger2);

            // Test 3-field overload
            var logger3 = logger.WithFields(
                Field.String("k1"u8.ToArray(), "v1"),
                Field.String("k2"u8.ToArray(), "v2"),
                Field.String("k3"u8.ToArray(), "v3")
            );
            Assert.NotNull(logger3);

            // Test 4-field overload
            var logger4 = logger.WithFields(
                Field.String("k1"u8.ToArray(), "v1"),
                Field.String("k2"u8.ToArray(), "v2"),
                Field.String("k3"u8.ToArray(), "v3"),
                Field.String("k4"u8.ToArray(), "v4")
            );
            Assert.NotNull(logger4);

            // Test 5-field overload
            var logger5 = logger.WithFields(
                Field.String("k1"u8.ToArray(), "v1"),
                Field.String("k2"u8.ToArray(), "v2"),
                Field.String("k3"u8.ToArray(), "v3"),
                Field.String("k4"u8.ToArray(), "v4"),
                Field.String("k5"u8.ToArray(), "v5")
            );
            Assert.NotNull(logger5);
        }

        [Fact]
        public void WithFields_MoreOverloads_ReturnNewLoggers() {
            var logger = new TaskLogger();

            // Test 6-field overload
            var logger6 = logger.WithFields(
                Field.Int64("f1"u8.ToArray(), 1),
                Field.Int64("f2"u8.ToArray(), 2),
                Field.Int64("f3"u8.ToArray(), 3),
                Field.Int64("f4"u8.ToArray(), 4),
                Field.Int64("f5"u8.ToArray(), 5),
                Field.Int64("f6"u8.ToArray(), 6)
            );
            Assert.NotNull(logger6);

            // Test 7-field overload
            var logger7 = logger.WithFields(
                Field.Bool("f1"u8.ToArray(), true),
                Field.Bool("f2"u8.ToArray(), false),
                Field.Bool("f3"u8.ToArray(), true),
                Field.Bool("f4"u8.ToArray(), false),
                Field.Bool("f5"u8.ToArray(), true),
                Field.Bool("f6"u8.ToArray(), false),
                Field.Bool("f7"u8.ToArray(), true)
            );
            Assert.NotNull(logger7);

            // Test 8-field overload
            var logger8 = logger.WithFields(
                Field.Float64("f1"u8.ToArray(), 1.1),
                Field.Float64("f2"u8.ToArray(), 2.2),
                Field.Float64("f3"u8.ToArray(), 3.3),
                Field.Float64("f4"u8.ToArray(), 4.4),
                Field.Float64("f5"u8.ToArray(), 5.5),
                Field.Float64("f6"u8.ToArray(), 6.6),
                Field.Float64("f7"u8.ToArray(), 7.7),
                Field.Float64("f8"u8.ToArray(), 8.8)
            );
            Assert.NotNull(logger8);
        }

        [Fact]
        public void WithFields_HighFieldCount_ReturnNewLoggers() {
            var logger = new TaskLogger();

            // Test 10-field overload
            var logger10 = logger.WithFields(
                Field.String("f1"u8.ToArray(), "1"),
                Field.String("f2"u8.ToArray(), "2"),
                Field.String("f3"u8.ToArray(), "3"),
                Field.String("f4"u8.ToArray(), "4"),
                Field.String("f5"u8.ToArray(), "5"),
                Field.String("f6"u8.ToArray(), "6"),
                Field.String("f7"u8.ToArray(), "7"),
                Field.String("f8"u8.ToArray(), "8"),
                Field.String("f9"u8.ToArray(), "9"),
                Field.String("f10"u8.ToArray(), "10")
            );
            Assert.NotNull(logger10);

            // Test 15-field overload
            var logger15 = logger.WithFields(
                Field.String("f1"u8.ToArray(), "1"),
                Field.String("f2"u8.ToArray(), "2"),
                Field.String("f3"u8.ToArray(), "3"),
                Field.String("f4"u8.ToArray(), "4"),
                Field.String("f5"u8.ToArray(), "5"),
                Field.String("f6"u8.ToArray(), "6"),
                Field.String("f7"u8.ToArray(), "7"),
                Field.String("f8"u8.ToArray(), "8"),
                Field.String("f9"u8.ToArray(), "9"),
                Field.String("f10"u8.ToArray(), "10"),
                Field.String("f11"u8.ToArray(), "11"),
                Field.String("f12"u8.ToArray(), "12"),
                Field.String("f13"u8.ToArray(), "13"),
                Field.String("f14"u8.ToArray(), "14"),
                Field.String("f15"u8.ToArray(), "15")
            );
            Assert.NotNull(logger15);

            // Test 20-field overload
            var logger20 = logger.WithFields(
                Field.String("f1"u8.ToArray(), "1"),
                Field.String("f2"u8.ToArray(), "2"),
                Field.String("f3"u8.ToArray(), "3"),
                Field.String("f4"u8.ToArray(), "4"),
                Field.String("f5"u8.ToArray(), "5"),
                Field.String("f6"u8.ToArray(), "6"),
                Field.String("f7"u8.ToArray(), "7"),
                Field.String("f8"u8.ToArray(), "8"),
                Field.String("f9"u8.ToArray(), "9"),
                Field.String("f10"u8.ToArray(), "10"),
                Field.String("f11"u8.ToArray(), "11"),
                Field.String("f12"u8.ToArray(), "12"),
                Field.String("f13"u8.ToArray(), "13"),
                Field.String("f14"u8.ToArray(), "14"),
                Field.String("f15"u8.ToArray(), "15"),
                Field.String("f16"u8.ToArray(), "16"),
                Field.String("f17"u8.ToArray(), "17"),
                Field.String("f18"u8.ToArray(), "18"),
                Field.String("f19"u8.ToArray(), "19"),
                Field.String("f20"u8.ToArray(), "20")
            );
            Assert.NotNull(logger20);
        }

        [Fact]
        public void Info_SingleField_LogsWithoutException() {
            var logger = new TaskLogger();

            // Should not throw
            logger.Info(Field.String("msg"u8.ToArray(), "test message"));
        }

        [Fact]
        public void Info_MultipleFields_LogsWithoutException() {
            var logger = new TaskLogger();

            // Test various overloads
            logger.Info(
                Field.String("msg"u8.ToArray(), "test"),
                Field.Int64("count"u8.ToArray(), 42)
            );

            logger.Info(
                Field.String("msg"u8.ToArray(), "test"),
                Field.Int64("count"u8.ToArray(), 42),
                Field.Bool("active"u8.ToArray(), true)
            );

            logger.Info(
                Field.String("f1"u8.ToArray(), "v1"),
                Field.String("f2"u8.ToArray(), "v2"),
                Field.String("f3"u8.ToArray(), "v3"),
                Field.String("f4"u8.ToArray(), "v4")
            );

            logger.Info(
                Field.String("f1"u8.ToArray(), "v1"),
                Field.String("f2"u8.ToArray(), "v2"),
                Field.String("f3"u8.ToArray(), "v3"),
                Field.String("f4"u8.ToArray(), "v4"),
                Field.String("f5"u8.ToArray(), "v5")
            );
        }

        [Fact]
        public void Debug_SingleField_LogsWithoutException() {
            var logger = new TaskLogger();

            logger.Debug(Field.String("debug_msg"u8.ToArray(), "debug test"));
        }

        [Fact]
        public void Debug_MultipleFields_LogsWithoutException() {
            var logger = new TaskLogger();

            logger.Debug(
                Field.String("msg"u8.ToArray(), "debug"),
                Field.Int64("line"u8.ToArray(), 100)
            );

            logger.Debug(
                Field.String("f1"u8.ToArray(), "v1"),
                Field.String("f2"u8.ToArray(), "v2"),
                Field.String("f3"u8.ToArray(), "v3")
            );
        }

        [Fact]
        public void Warn_SingleField_LogsWithoutException() {
            var logger = new TaskLogger();

            logger.Warn(Field.String("warning"u8.ToArray(), "this is a warning"));
        }

        [Fact]
        public void Warn_MultipleFields_LogsWithoutException() {
            var logger = new TaskLogger();

            logger.Warn(
                Field.String("msg"u8.ToArray(), "warning"),
                Field.Int64("code"u8.ToArray(), 500)
            );
        }

        [Fact]
        public void Error_SingleField_LogsWithoutException() {
            var logger = new TaskLogger();

            logger.Error(Field.String("error"u8.ToArray(), "error occurred"));
        }

        [Fact]
        public void Error_MultipleFields_LogsWithoutException() {
            var logger = new TaskLogger();

            logger.Error(
                Field.String("msg"u8.ToArray(), "error"),
                Field.String("stack"u8.ToArray(), "stack trace here")
            );
        }

        [Fact]
        public void Fatal_SingleField_LogsWithoutException() {
            var logger = new TaskLogger();

            logger.Fatal(Field.String("fatal"u8.ToArray(), "fatal error"));
        }

        [Fact]
        public void Fatal_MultipleFields_LogsWithoutException() {
            var logger = new TaskLogger();

            logger.Fatal(
                Field.String("msg"u8.ToArray(), "fatal"),
                Field.Int64("exit_code"u8.ToArray(), 1)
            );
        }

        [Fact]
        public void LogLevel_InfoFiltering_SkipsWhenLevelTooLow() {
            // Reinitialize with Error level (lower than Info)
            Logger.Shutdown();
            Logger.Init(level: LogLevel.Warn);

            var logger = new TaskLogger();

            // This should be filtered out at Info level when global is Warn
            logger.Info(Field.String("msg"u8.ToArray(), "should not log"));

            // No exception should occur
        }

        [Fact]
        public void LogLevel_DebugFiltering_SkipsWhenLevelTooLow() {
            Logger.Shutdown();
            Logger.Init(level: LogLevel.Info);

            var logger = new TaskLogger();

            // Debug is higher than Info, so it should be filtered
            logger.Debug(Field.String("msg"u8.ToArray(), "should not log"));
        }

        [Fact]
        public void AllFieldTypes_InSingleLogCall() {
            var logger = new TaskLogger();
            var now = DateTime.UtcNow;

            logger.Info(
                Field.String("str"u8.ToArray(), "hello"),
                Field.Int64("int"u8.ToArray(), 12345),
                Field.UInt64("uint"u8.ToArray(), 99999),
                Field.Bool("bool"u8.ToArray(), true),
                Field.Float64("float"u8.ToArray(), 3.14),
                Field.UtcDateTime("time"u8.ToArray(), now),
                Field.RawJson("json"u8.ToArray(), "{\"nested\":1}")
            );
        }

        [Fact]
        public void WithFields_ChainedCalls_BuildsPrefix() {
            var logger = new TaskLogger();

            var logger1 = logger.WithFields(Field.String("app"u8.ToArray(), "my-app"));
            var logger2 = logger1.WithFields(Field.String("module"u8.ToArray(), "auth"));
            var logger3 = logger2.WithFields(Field.String("action"u8.ToArray(), "login"));

            // Each logger should be independent
            Assert.NotSame(logger, logger1);
            Assert.NotSame(logger1, logger2);
            Assert.NotSame(logger2, logger3);

            // Logging should work with chained logger
            logger3.Info(Field.String("user"u8.ToArray(), "test-user"));
        }

        [Fact]
        public void Info_HighFieldCount_LogsWithoutException() {
            var logger = new TaskLogger();

            // Test 10-field Info
            logger.Info(
                Field.String("f1"u8.ToArray(), "1"),
                Field.String("f2"u8.ToArray(), "2"),
                Field.String("f3"u8.ToArray(), "3"),
                Field.String("f4"u8.ToArray(), "4"),
                Field.String("f5"u8.ToArray(), "5"),
                Field.String("f6"u8.ToArray(), "6"),
                Field.String("f7"u8.ToArray(), "7"),
                Field.String("f8"u8.ToArray(), "8"),
                Field.String("f9"u8.ToArray(), "9"),
                Field.String("f10"u8.ToArray(), "10")
            );

            // Test 15-field Info
            logger.Info(
                Field.String("f1"u8.ToArray(), "1"),
                Field.String("f2"u8.ToArray(), "2"),
                Field.String("f3"u8.ToArray(), "3"),
                Field.String("f4"u8.ToArray(), "4"),
                Field.String("f5"u8.ToArray(), "5"),
                Field.String("f6"u8.ToArray(), "6"),
                Field.String("f7"u8.ToArray(), "7"),
                Field.String("f8"u8.ToArray(), "8"),
                Field.String("f9"u8.ToArray(), "9"),
                Field.String("f10"u8.ToArray(), "10"),
                Field.String("f11"u8.ToArray(), "11"),
                Field.String("f12"u8.ToArray(), "12"),
                Field.String("f13"u8.ToArray(), "13"),
                Field.String("f14"u8.ToArray(), "14"),
                Field.String("f15"u8.ToArray(), "15")
            );

            // Test 20-field Info
            logger.Info(
                Field.String("f1"u8.ToArray(), "1"),
                Field.String("f2"u8.ToArray(), "2"),
                Field.String("f3"u8.ToArray(), "3"),
                Field.String("f4"u8.ToArray(), "4"),
                Field.String("f5"u8.ToArray(), "5"),
                Field.String("f6"u8.ToArray(), "6"),
                Field.String("f7"u8.ToArray(), "7"),
                Field.String("f8"u8.ToArray(), "8"),
                Field.String("f9"u8.ToArray(), "9"),
                Field.String("f10"u8.ToArray(), "10"),
                Field.String("f11"u8.ToArray(), "11"),
                Field.String("f12"u8.ToArray(), "12"),
                Field.String("f13"u8.ToArray(), "13"),
                Field.String("f14"u8.ToArray(), "14"),
                Field.String("f15"u8.ToArray(), "15"),
                Field.String("f16"u8.ToArray(), "16"),
                Field.String("f17"u8.ToArray(), "17"),
                Field.String("f18"u8.ToArray(), "18"),
                Field.String("f19"u8.ToArray(), "19"),
                Field.String("f20"u8.ToArray(), "20")
            );
        }

        [Fact]
        public void SpecialCharacters_InFieldValues_AreEscaped() {
            var logger = new TaskLogger();

            // Test with special characters that need JSON escaping
            logger.Info(
                Field.String("msg"u8.ToArray(), "line1\nline2\ttab\"quote\\backslash")
            );
        }

        [Fact]
        public void Utf8String_InFields_WorksCorrectly() {
            var logger = new TaskLogger();

            logger.Info(
                Field.Utf8String("utf8msg"u8.ToArray(), "hello utf8"u8.ToArray())
            );
        }

        [Fact]
        public void RawJson_InFields_WorksCorrectly() {
            var logger = new TaskLogger();

            logger.Info(
                Field.RawJson("data"u8.ToArray(), "{\"key\":\"value\",\"num\":123}")
            );

            logger.Info(
                Field.RawJson("arr"u8.ToArray(), "[1,2,3,4,5]"u8.ToArray())
            );
        }
    }
}
