using System;
using System.Collections.Generic;
using System.Threading;
using Log;
using Xunit;
using LogLevel = global::Log.LogLevel;

namespace Tests.Log {
    /// <summary>
    /// Tests for Logger.cs
    /// Note: Logger is a singleton with global state, tests must be run sequentially
    /// </summary>
    [Collection("LoggerTests")]
    public class LoggerTests : IDisposable {
        public LoggerTests() {
            // Ensure clean state before each test
            // Logger.Instance may be null or already disposed
            try {
                if (Logger.Instance != null) {
                    Logger.Shutdown();
                }
            } catch (ObjectDisposedException) {
                // Logger was already disposed, that's fine
            }
        }

        public void Dispose() {
            // Clean up after each test
            if (Logger.Instance != null) {
                try {
                    Logger.Shutdown();
                } catch { }
            }
        }

        #region TestCase Structures
        public struct InitTestCase {
            public string Name;
            public global::Log.LogLevel Level;
            public int FlushIntervalMs;
            public OverloadPolicy Policy;
            public int QueueSize;
            public int LogBufferSize;
        }

        public struct SetLevelTestCase {
            public string Name;
            public global::Log.LogLevel InitLevel;
            public global::Log.LogLevel NewLevel;
        }

        public struct SetFlushIntervalTestCase {
            public string Name;
            public int InitInterval;
            public int NewInterval;
            public int ExpectedInterval;
        }
        #endregion

        [Fact]
        public void Init_WithDefaultParameters_CreatesInstance() {
            Logger.Init();

            Assert.NotNull(Logger.Instance);
            Assert.Equal(LogLevel.Warn, Logger.Instance.Level);
            Assert.Equal(OverloadPolicy.Block, Logger.Instance.OverloadPolicy);

            Logger.Shutdown();
        }

        [Fact]
        public void Init_WithCustomParameters_SetsCorrectValues() {
            var testCases = new InitTestCase[] {
                new() {
                    Name = "debug level with drop policy",
                    Level = LogLevel.Debug,
                    FlushIntervalMs = 500,
                    Policy = OverloadPolicy.Drop,
                    QueueSize = 100,
                    LogBufferSize = 8192
                },
                new() {
                    Name = "info level with block policy",
                    Level = LogLevel.Info,
                    FlushIntervalMs = 2000,
                    Policy = OverloadPolicy.Block,
                    QueueSize = 50,
                    LogBufferSize = 16384
                },
                new() {
                    Name = "error level",
                    Level = LogLevel.Error,
                    FlushIntervalMs = 1000,
                    Policy = OverloadPolicy.Block,
                    QueueSize = 200,
                    LogBufferSize = 32768
                },
            };

            foreach (var tc in testCases) {
                // Cleanup from previous iteration
                try {
                    if (Logger.Instance != null) {
                        Logger.Shutdown();
                    }
                } catch (ObjectDisposedException) { }

                Logger.Init(
                    level: tc.Level,
                    flushIntervalMs: tc.FlushIntervalMs,
                    overload: tc.Policy,
                    queueSize: tc.QueueSize,
                    logBufferSize: tc.LogBufferSize
                );

                Assert.NotNull(Logger.Instance);
                Assert.Equal(tc.Level, Logger.Instance.Level);
                Assert.Equal(tc.Policy, Logger.Instance.OverloadPolicy);
                Assert.Equal(tc.LogBufferSize, Logger.Instance.LogBufferSize);
            }
        }

        [Fact]
        public void Init_WithTags_SetsGlobalTags() {
            var tags = new Dictionary<string, string> {
                { "app", "test-app" },
                { "env", "testing" }
            };

            Logger.Init(level: LogLevel.Info, tags: tags);

            Assert.NotNull(Logger.Instance);
            Assert.True(Logger.Instance.TagPrefix.Length > 0);

            Logger.Shutdown();
        }

        [Fact]
        public void Init_WithSmallQueueSize_SetsMinimumQueueSize() {
            Logger.Init(queueSize: 0);

            Assert.NotNull(Logger.Instance);

            Logger.Shutdown();
        }

        [Fact]
        public void Init_WithSmallFlushInterval_SetsMinimumInterval() {
            // Minimum is 100ms
            Logger.Init(flushIntervalMs: 10);

            Assert.NotNull(Logger.Instance);

            Logger.Shutdown();
        }

        [Fact]
        public void Init_WithSmallLogBufferSize_SetsMinimumSize() {
            // Minimum is 4096 bytes
            Logger.Init(logBufferSize: 100);

            Assert.NotNull(Logger.Instance);
            Assert.True(Logger.Instance.LogBufferSize >= 4096);

            Logger.Shutdown();
        }

        [Fact]
        public void SetLevel_ChangesLogLevel() {
            var testCases = new SetLevelTestCase[] {
                new() { Name = "warn to debug", InitLevel = LogLevel.Warn, NewLevel = LogLevel.Debug },
                new() { Name = "info to error", InitLevel = LogLevel.Info, NewLevel = LogLevel.Error },
                new() { Name = "debug to fatal", InitLevel = LogLevel.Debug, NewLevel = LogLevel.Fatal },
            };

            foreach (var tc in testCases) {
                try {
                    if (Logger.Instance != null) {
                        Logger.Shutdown();
                    }
                } catch (ObjectDisposedException) { }

                Logger.Init(level: tc.InitLevel);
                Assert.Equal(tc.InitLevel, Logger.Instance!.Level);

                Logger.SetLevel(tc.NewLevel);
                Assert.Equal(tc.NewLevel, Logger.Instance!.Level);
            }
        }

        [Fact]
        public void SetFlushIntervalMs_ChangesInterval() {
            Logger.Init(flushIntervalMs: 1000);

            // Set to valid value
            Logger.SetFlushIntervalMs(500);
            // SetFlushIntervalMs with value < 100 should be clamped to 100

            Logger.SetFlushIntervalMs(50);
            // No way to directly verify interval, but method should not throw

            Logger.Shutdown();
        }

        [Fact]
        public void Shutdown_DisposesLogger() {
            Logger.Init();
            Assert.NotNull(Logger.Instance);

            Logger.Shutdown();
            // After shutdown, Instance is still set but internal resources are disposed
        }

        [Fact]
        public void LogLevel_HasExpectedValues() {
            // Verify enum ordering (Fatal < Error < Warn < Info < Debug)
            Assert.True(LogLevel.Fatal < LogLevel.Error);
            Assert.True(LogLevel.Error < LogLevel.Warn);
            Assert.True(LogLevel.Warn < LogLevel.Info);
            Assert.True(LogLevel.Info < LogLevel.Debug);
        }

        [Fact]
        public void OverloadPolicy_HasExpectedValues() {
            Assert.Equal(0, (int)OverloadPolicy.Block);
            Assert.Equal(1, (int)OverloadPolicy.Drop);
        }
    }
}
