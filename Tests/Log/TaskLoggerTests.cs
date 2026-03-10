using Log;
using Xunit;
using LogLevel = global::Log.LogLevel;

namespace Tests.Log;

/// <summary>
/// Tests for TaskLogger.cs
/// Note: These tests depend on Logger singleton initialization
/// </summary>
[Collection("LoggerTests")]
public class TaskLoggerTests : IDisposable
{
    [Fact]
    public void output()
    {
        var logger1 = new global::Log.TaskLogger();
        var logger2 = logger1.WithFields(global::Log.Field.String("pod"u8, "qiwa-test-123456789"));
        logger2 = logger2.WithFields(global::Log.Field.String("user"u8, "alice"), global::Log.Field.Int64("count"u8, 2));
        logger2 = logger2.WithFields(global::Log.Field.String("user"u8, "alice"), global::Log.Field.Int64("count"u8, 2), global::Log.Field.Bool("active"u8, true));
        logger2 = logger2.WithFields(global::Log.Field.String("user"u8, "alice"), global::Log.Field.Int64("count"u8, 2), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 42));
        logger2 = logger2.WithFields(global::Log.Field.String("user"u8, "alice"), global::Log.Field.Int64("count"u8, 2), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 42), global::Log.Field.Float64("ratio"u8, 0.75));
        logger2 = logger2.WithFields(global::Log.Field.String("user"u8, "alice"), global::Log.Field.Int64("count"u8, 2), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 42), global::Log.Field.Float64("ratio"u8, 0.75), global::Log.Field.Utf8String("region"u8, "us-west-2"u8));
        logger2 = logger2.WithFields(global::Log.Field.String("user"u8, "alice"), global::Log.Field.Int64("count"u8, 2), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 42), global::Log.Field.Float64("ratio"u8, 0.75), global::Log.Field.Utf8String("region"u8, "us-west-2"u8), global::Log.Field.RawJson("meta"u8, "{\"env\":\"test\"}"));
        logger2 = logger2.WithFields(global::Log.Field.String("user"u8, "alice"), global::Log.Field.Int64("count"u8, 2), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 42), global::Log.Field.Float64("ratio"u8, 0.75), global::Log.Field.Utf8String("region"u8, "us-west-2"u8), global::Log.Field.RawJson("meta"u8, "{\"env\":\"test\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch));
        logger2 = logger2.WithFields(global::Log.Field.String("user"u8, "alice"), global::Log.Field.Int64("count"u8, 2), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 42), global::Log.Field.Float64("ratio"u8, 0.75), global::Log.Field.Utf8String("region"u8, "us-west-2"u8), global::Log.Field.RawJson("meta"u8, "{\"env\":\"test\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("service"u8, "qiwa-api"));
        logger2 = logger2.WithFields(global::Log.Field.String("user"u8, "alice"), global::Log.Field.Int64("count"u8, 2), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 42), global::Log.Field.Float64("ratio"u8, 0.75), global::Log.Field.Utf8String("region"u8, "us-west-2"u8), global::Log.Field.RawJson("meta"u8, "{\"env\":\"test\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("service"u8, "qiwa-api"), global::Log.Field.Int64("latency_ms"u8, 123));
        logger2 = logger2.WithFields(global::Log.Field.String("user"u8, "alice"), global::Log.Field.Int64("count"u8, 2), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 42), global::Log.Field.Float64("ratio"u8, 0.75), global::Log.Field.Utf8String("region"u8, "us-west-2"u8), global::Log.Field.RawJson("meta"u8, "{\"env\":\"test\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("service"u8, "qiwa-api"), global::Log.Field.Int64("latency_ms"u8, 123), global::Log.Field.Bool("cache_hit"u8, false));
        logger2 = logger2.WithFields(global::Log.Field.String("user"u8, "alice"), global::Log.Field.Int64("count"u8, 2), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 42), global::Log.Field.Float64("ratio"u8, 0.75), global::Log.Field.Utf8String("region"u8, "us-west-2"u8), global::Log.Field.RawJson("meta"u8, "{\"env\":\"test\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("service"u8, "qiwa-api"), global::Log.Field.Int64("latency_ms"u8, 123), global::Log.Field.Bool("cache_hit"u8, false), global::Log.Field.UInt64("req_id"u8, 999));
        logger2 = logger2.WithFields(global::Log.Field.String("user"u8, "alice"), global::Log.Field.Int64("count"u8, 2), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 42), global::Log.Field.Float64("ratio"u8, 0.75), global::Log.Field.Utf8String("region"u8, "us-west-2"u8), global::Log.Field.RawJson("meta"u8, "{\"env\":\"test\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("service"u8, "qiwa-api"), global::Log.Field.Int64("latency_ms"u8, 123), global::Log.Field.Bool("cache_hit"u8, false), global::Log.Field.UInt64("req_id"u8, 999), global::Log.Field.Float64("cpu"u8, 0.42));
        logger2 = logger2.WithFields(global::Log.Field.String("user"u8, "alice"), global::Log.Field.Int64("count"u8, 2), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 42), global::Log.Field.Float64("ratio"u8, 0.75), global::Log.Field.Utf8String("region"u8, "us-west-2"u8), global::Log.Field.RawJson("meta"u8, "{\"env\":\"test\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("service"u8, "qiwa-api"), global::Log.Field.Int64("latency_ms"u8, 123), global::Log.Field.Bool("cache_hit"u8, false), global::Log.Field.UInt64("req_id"u8, 999), global::Log.Field.Float64("cpu"u8, 0.42), global::Log.Field.Utf8String("zone"u8, "cn-north-1"u8));
        logger2 = logger2.WithFields(global::Log.Field.String("user"u8, "alice"), global::Log.Field.Int64("count"u8, 2), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 42), global::Log.Field.Float64("ratio"u8, 0.75), global::Log.Field.Utf8String("region"u8, "us-west-2"u8), global::Log.Field.RawJson("meta"u8, "{\"env\":\"test\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("service"u8, "qiwa-api"), global::Log.Field.Int64("latency_ms"u8, 123), global::Log.Field.Bool("cache_hit"u8, false), global::Log.Field.UInt64("req_id"u8, 999), global::Log.Field.Float64("cpu"u8, 0.42), global::Log.Field.Utf8String("zone"u8, "cn-north-1"u8), global::Log.Field.RawJson("payload"u8, "{\"ok\":true}"u8));
        logger2 = logger2.WithFields(global::Log.Field.String("user"u8, "alice"), global::Log.Field.Int64("count"u8, 2), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 42), global::Log.Field.Float64("ratio"u8, 0.75), global::Log.Field.Utf8String("region"u8, "us-west-2"u8), global::Log.Field.RawJson("meta"u8, "{\"env\":\"test\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("service"u8, "qiwa-api"), global::Log.Field.Int64("latency_ms"u8, 123), global::Log.Field.Bool("cache_hit"u8, false), global::Log.Field.UInt64("req_id"u8, 999), global::Log.Field.Float64("cpu"u8, 0.42), global::Log.Field.Utf8String("zone"u8, "cn-north-1"u8), global::Log.Field.RawJson("payload"u8, "{\"ok\":true}"u8), global::Log.Field.String("path"u8, "/v1/items"));
        logger2 = logger2.WithFields(global::Log.Field.String("user"u8, "alice"), global::Log.Field.Int64("count"u8, 2), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 42), global::Log.Field.Float64("ratio"u8, 0.75), global::Log.Field.Utf8String("region"u8, "us-west-2"u8), global::Log.Field.RawJson("meta"u8, "{\"env\":\"test\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("service"u8, "qiwa-api"), global::Log.Field.Int64("latency_ms"u8, 123), global::Log.Field.Bool("cache_hit"u8, false), global::Log.Field.UInt64("req_id"u8, 999), global::Log.Field.Float64("cpu"u8, 0.42), global::Log.Field.Utf8String("zone"u8, "cn-north-1"u8), global::Log.Field.RawJson("payload"u8, "{\"ok\":true}"u8), global::Log.Field.String("path"u8, "/v1/items"), global::Log.Field.Int64("size"u8, 2048));
        logger2 = logger2.WithFields(global::Log.Field.String("user"u8, "alice"), global::Log.Field.Int64("count"u8, 2), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 42), global::Log.Field.Float64("ratio"u8, 0.75), global::Log.Field.Utf8String("region"u8, "us-west-2"u8), global::Log.Field.RawJson("meta"u8, "{\"env\":\"test\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("service"u8, "qiwa-api"), global::Log.Field.Int64("latency_ms"u8, 123), global::Log.Field.Bool("cache_hit"u8, false), global::Log.Field.UInt64("req_id"u8, 999), global::Log.Field.Float64("cpu"u8, 0.42), global::Log.Field.Utf8String("zone"u8, "cn-north-1"u8), global::Log.Field.RawJson("payload"u8, "{\"ok\":true}"u8), global::Log.Field.String("path"u8, "/v1/items"), global::Log.Field.Int64("size"u8, 2048), global::Log.Field.Bool("debug"u8, true));
        logger2 = logger2.WithFields(global::Log.Field.String("user"u8, "alice"), global::Log.Field.Int64("count"u8, 2), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 42), global::Log.Field.Float64("ratio"u8, 0.75), global::Log.Field.Utf8String("region"u8, "us-west-2"u8), global::Log.Field.RawJson("meta"u8, "{\"env\":\"test\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("service"u8, "qiwa-api"), global::Log.Field.Int64("latency_ms"u8, 123), global::Log.Field.Bool("cache_hit"u8, false), global::Log.Field.UInt64("req_id"u8, 999), global::Log.Field.Float64("cpu"u8, 0.42), global::Log.Field.Utf8String("zone"u8, "cn-north-1"u8), global::Log.Field.RawJson("payload"u8, "{\"ok\":true}"u8), global::Log.Field.String("path"u8, "/v1/items"), global::Log.Field.Int64("size"u8, 2048), global::Log.Field.Bool("debug"u8, true), global::Log.Field.UInt64("build"u8, 20240101));
        logger2 = logger2.WithFields(global::Log.Field.String("user"u8, "alice"), global::Log.Field.Int64("count"u8, 2), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 42), global::Log.Field.Float64("ratio"u8, 0.75), global::Log.Field.Utf8String("region"u8, "us-west-2"u8), global::Log.Field.RawJson("meta"u8, "{\"env\":\"test\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("service"u8, "qiwa-api"), global::Log.Field.Int64("latency_ms"u8, 123), global::Log.Field.Bool("cache_hit"u8, false), global::Log.Field.UInt64("req_id"u8, 999), global::Log.Field.Float64("cpu"u8, 0.42), global::Log.Field.Utf8String("zone"u8, "cn-north-1"u8), global::Log.Field.RawJson("payload"u8, "{\"ok\":true}"u8), global::Log.Field.String("path"u8, "/v1/items"), global::Log.Field.Int64("size"u8, 2048), global::Log.Field.Bool("debug"u8, true), global::Log.Field.UInt64("build"u8, 20240101), global::Log.Field.Float64("temp"u8, 36.5));
        logger2.Debug(global::Log.Field.String("debug1"u8, "this is debug log"));
        logger2.Debug(global::Log.Field.String("user"u8, "bob"), global::Log.Field.Int64("age"u8, 34));
        logger2.Debug(global::Log.Field.String("user"u8, "bob"), global::Log.Field.Int64("age"u8, 34), global::Log.Field.Bool("active"u8, true));
        logger2.Debug(global::Log.Field.String("user"u8, "bob"), global::Log.Field.Int64("age"u8, 34), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 9001));
        logger2.Debug(global::Log.Field.String("user"u8, "bob"), global::Log.Field.Int64("age"u8, 34), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 9001), global::Log.Field.Float64("score"u8, 98.6));
        logger2.Debug(global::Log.Field.String("user"u8, "bob"), global::Log.Field.Int64("age"u8, 34), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 9001), global::Log.Field.Float64("score"u8, 98.6), global::Log.Field.Utf8String("ip"u8, "10.0.0.1"u8));
        logger2.Debug(global::Log.Field.String("user"u8, "bob"), global::Log.Field.Int64("age"u8, 34), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 9001), global::Log.Field.Float64("score"u8, 98.6), global::Log.Field.Utf8String("ip"u8, "10.0.0.1"u8), global::Log.Field.RawJson("meta"u8, "{\"tier\":\"gold\"}"));
        logger2.Debug(global::Log.Field.String("user"u8, "bob"), global::Log.Field.Int64("age"u8, 34), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 9001), global::Log.Field.Float64("score"u8, 98.6), global::Log.Field.Utf8String("ip"u8, "10.0.0.1"u8), global::Log.Field.RawJson("meta"u8, "{\"tier\":\"gold\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch));
        logger2.Debug(global::Log.Field.String("user"u8, "bob"), global::Log.Field.Int64("age"u8, 34), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 9001), global::Log.Field.Float64("score"u8, 98.6), global::Log.Field.Utf8String("ip"u8, "10.0.0.1"u8), global::Log.Field.RawJson("meta"u8, "{\"tier\":\"gold\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("tenant"u8, "acme"));
        logger2.Debug(global::Log.Field.String("user"u8, "bob"), global::Log.Field.Int64("age"u8, 34), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 9001), global::Log.Field.Float64("score"u8, 98.6), global::Log.Field.Utf8String("ip"u8, "10.0.0.1"u8), global::Log.Field.RawJson("meta"u8, "{\"tier\":\"gold\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("tenant"u8, "acme"), global::Log.Field.Int64("limit"u8, 50));
        logger2.Debug(global::Log.Field.String("user"u8, "bob"), global::Log.Field.Int64("age"u8, 34), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 9001), global::Log.Field.Float64("score"u8, 98.6), global::Log.Field.Utf8String("ip"u8, "10.0.0.1"u8), global::Log.Field.RawJson("meta"u8, "{\"tier\":\"gold\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("tenant"u8, "acme"), global::Log.Field.Int64("limit"u8, 50), global::Log.Field.Bool("cache"u8, false));
        logger2.Debug(global::Log.Field.String("user"u8, "bob"), global::Log.Field.Int64("age"u8, 34), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 9001), global::Log.Field.Float64("score"u8, 98.6), global::Log.Field.Utf8String("ip"u8, "10.0.0.1"u8), global::Log.Field.RawJson("meta"u8, "{\"tier\":\"gold\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("tenant"u8, "acme"), global::Log.Field.Int64("limit"u8, 50), global::Log.Field.Bool("cache"u8, false), global::Log.Field.UInt64("trace_id"u8, 1234567890));
        logger2.Debug(global::Log.Field.String("user"u8, "bob"), global::Log.Field.Int64("age"u8, 34), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 9001), global::Log.Field.Float64("score"u8, 98.6), global::Log.Field.Utf8String("ip"u8, "10.0.0.1"u8), global::Log.Field.RawJson("meta"u8, "{\"tier\":\"gold\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("tenant"u8, "acme"), global::Log.Field.Int64("limit"u8, 50), global::Log.Field.Bool("cache"u8, false), global::Log.Field.UInt64("trace_id"u8, 1234567890), global::Log.Field.Float64("ratio"u8, 0.33));
        logger2.Debug(global::Log.Field.String("user"u8, "bob"), global::Log.Field.Int64("age"u8, 34), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 9001), global::Log.Field.Float64("score"u8, 98.6), global::Log.Field.Utf8String("ip"u8, "10.0.0.1"u8), global::Log.Field.RawJson("meta"u8, "{\"tier\":\"gold\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("tenant"u8, "acme"), global::Log.Field.Int64("limit"u8, 50), global::Log.Field.Bool("cache"u8, false), global::Log.Field.UInt64("trace_id"u8, 1234567890), global::Log.Field.Float64("ratio"u8, 0.33), global::Log.Field.Utf8String("zone"u8, "eu-central-1a"u8));
        logger2.Debug(global::Log.Field.String("user"u8, "bob"), global::Log.Field.Int64("age"u8, 34), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 9001), global::Log.Field.Float64("score"u8, 98.6), global::Log.Field.Utf8String("ip"u8, "10.0.0.1"u8), global::Log.Field.RawJson("meta"u8, "{\"tier\":\"gold\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("tenant"u8, "acme"), global::Log.Field.Int64("limit"u8, 50), global::Log.Field.Bool("cache"u8, false), global::Log.Field.UInt64("trace_id"u8, 1234567890), global::Log.Field.Float64("ratio"u8, 0.33), global::Log.Field.Utf8String("zone"u8, "eu-central-1a"u8), global::Log.Field.RawJson("payload"u8, "{\"ok\":true}"u8));
        logger2.Debug(global::Log.Field.String("user"u8, "bob"), global::Log.Field.Int64("age"u8, 34), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 9001), global::Log.Field.Float64("score"u8, 98.6), global::Log.Field.Utf8String("ip"u8, "10.0.0.1"u8), global::Log.Field.RawJson("meta"u8, "{\"tier\":\"gold\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("tenant"u8, "acme"), global::Log.Field.Int64("limit"u8, 50), global::Log.Field.Bool("cache"u8, false), global::Log.Field.UInt64("trace_id"u8, 1234567890), global::Log.Field.Float64("ratio"u8, 0.33), global::Log.Field.Utf8String("zone"u8, "eu-central-1a"u8), global::Log.Field.RawJson("payload"u8, "{\"ok\":true}"u8), global::Log.Field.String("method"u8, "GET"));
        logger2.Debug(global::Log.Field.String("user"u8, "bob"), global::Log.Field.Int64("age"u8, 34), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 9001), global::Log.Field.Float64("score"u8, 98.6), global::Log.Field.Utf8String("ip"u8, "10.0.0.1"u8), global::Log.Field.RawJson("meta"u8, "{\"tier\":\"gold\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("tenant"u8, "acme"), global::Log.Field.Int64("limit"u8, 50), global::Log.Field.Bool("cache"u8, false), global::Log.Field.UInt64("trace_id"u8, 1234567890), global::Log.Field.Float64("ratio"u8, 0.33), global::Log.Field.Utf8String("zone"u8, "eu-central-1a"u8), global::Log.Field.RawJson("payload"u8, "{\"ok\":true}"u8), global::Log.Field.String("method"u8, "GET"), global::Log.Field.Int64("status"u8, 200));
        logger2.Debug(global::Log.Field.String("user"u8, "bob"), global::Log.Field.Int64("age"u8, 34), global::Log.Field.Bool("active"u8, true), global::Log.Field.UInt64("uid"u8, 9001), global::Log.Field.Float64("score"u8, 98.6), global::Log.Field.Utf8String("ip"u8, "10.0.0.1"u8), global::Log.Field.RawJson("meta"u8, "{\"tier\":\"gold\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("tenant"u8, "acme"), global::Log.Field.Int64("limit"u8, 50), global::Log.Field.Bool("cache"u8, false), global::Log.Field.UInt64("trace_id"u8, 1234567890), global::Log.Field.Float64("ratio"u8, 0.33), global::Log.Field.Utf8String("zone"u8, "eu-central-1a"u8), global::Log.Field.RawJson("payload"u8, "{\"ok\":true}"u8), global::Log.Field.String("method"u8, "GET"), global::Log.Field.Int64("status"u8, 200), global::Log.Field.Bool("retry"u8, false));
        logger2.Debug(global::Log.Field.String("user"u8, "bob"), global::Log.Field.Int64("age"u8, 34), global::Log.Field.Bool("active"u8, false), global::Log.Field.UInt64("uid"u8, 9001), global::Log.Field.Float64("score"u8, 98.6), global::Log.Field.Utf8String("ip"u8, "10.0.0.1"u8), global::Log.Field.RawJson("meta"u8, "{\"tier\":\"gold\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("tenant"u8, "acme"), global::Log.Field.Int64("limit"u8, 50), global::Log.Field.Bool("cache"u8, false), global::Log.Field.UInt64("trace_id"u8, 1234567890), global::Log.Field.Float64("ratio"u8, 0.33), global::Log.Field.Utf8String("zone"u8, "eu-central-1a"u8), global::Log.Field.RawJson("payload"u8, "{\"ok\":true}"u8), global::Log.Field.String("method"u8, "GET"), global::Log.Field.Int64("status"u8, 200), global::Log.Field.Bool("retry"u8, false), global::Log.Field.UInt64("bytes"u8, 4096));
        logger2.Debug(global::Log.Field.String("user"u8, "bob"), global::Log.Field.Int64("age"u8, 34), global::Log.Field.Bool("active"u8, false), global::Log.Field.UInt64("uid"u8, 9001), global::Log.Field.Float64("score"u8, 98.6), global::Log.Field.Utf8String("ip"u8, "10.0.0.1"u8), global::Log.Field.RawJson("meta"u8, "{\"tier\":\"gold\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("tenant"u8, "acme"), global::Log.Field.Int64("limit"u8, 50), global::Log.Field.Bool("cache"u8, false), global::Log.Field.UInt64("trace_id"u8, 1234567890), global::Log.Field.Float64("ratio"u8, 0.33), global::Log.Field.Utf8String("zone"u8, "eu-central-1a"u8), global::Log.Field.RawJson("payload"u8, "{\"ok\":true}"u8), global::Log.Field.String("method"u8, "GET"), global::Log.Field.Int64("status"u8, 200), global::Log.Field.Bool("retry"u8, false), global::Log.Field.UInt64("bytes"u8, 4096), global::Log.Field.Float64("latency_ms"u8, 12.34));
        logger2.Info(global::Log.Field.String("info1"u8, "this is info log"));
        logger2.Info(global::Log.Field.String("session"u8, "s1"), global::Log.Field.Int64("attempt"u8, 1));
        logger2.Info(global::Log.Field.String("session"u8, "s1"), global::Log.Field.Int64("attempt"u8, 1), global::Log.Field.Bool("success"u8, true));
        logger2.Info(global::Log.Field.String("session"u8, "s1"), global::Log.Field.Int64("attempt"u8, 1), global::Log.Field.Bool("success"u8, true), global::Log.Field.UInt64("seq"u8, 7));
        logger2.Info(global::Log.Field.String("session"u8, "s1"), global::Log.Field.Int64("attempt"u8, 1), global::Log.Field.Bool("success"u8, true), global::Log.Field.UInt64("seq"u8, 7), global::Log.Field.Float64("elapsed"u8, 1.23));
        logger2.Info(global::Log.Field.String("session"u8, "s1"), global::Log.Field.Int64("attempt"u8, 1), global::Log.Field.Bool("success"u8, true), global::Log.Field.UInt64("seq"u8, 7), global::Log.Field.Float64("elapsed"u8, 1.23), global::Log.Field.Utf8String("host"u8, "node-01"u8));
        logger2.Info(global::Log.Field.String("session"u8, "s1"), global::Log.Field.Int64("attempt"u8, 1), global::Log.Field.Bool("success"u8, true), global::Log.Field.UInt64("seq"u8, 7), global::Log.Field.Float64("elapsed"u8, 1.23), global::Log.Field.Utf8String("host"u8, "node-01"u8), global::Log.Field.RawJson("tags"u8, "{\"a\":1}"));
        logger2.Info(global::Log.Field.String("session"u8, "s1"), global::Log.Field.Int64("attempt"u8, 1), global::Log.Field.Bool("success"u8, true), global::Log.Field.UInt64("seq"u8, 7), global::Log.Field.Float64("elapsed"u8, 1.23), global::Log.Field.Utf8String("host"u8, "node-01"u8), global::Log.Field.RawJson("tags"u8, "{\"a\":1}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch));
        logger2.Info(global::Log.Field.String("session"u8, "s1"), global::Log.Field.Int64("attempt"u8, 1), global::Log.Field.Bool("success"u8, true), global::Log.Field.UInt64("seq"u8, 7), global::Log.Field.Float64("elapsed"u8, 1.23), global::Log.Field.Utf8String("host"u8, "node-01"u8), global::Log.Field.RawJson("tags"u8, "{\"a\":1}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("route"u8, "/v2/order"));
        logger2.Info(global::Log.Field.String("session"u8, "s1"), global::Log.Field.Int64("attempt"u8, 1), global::Log.Field.Bool("success"u8, true), global::Log.Field.UInt64("seq"u8, 7), global::Log.Field.Float64("elapsed"u8, 1.23), global::Log.Field.Utf8String("host"u8, "node-01"u8), global::Log.Field.RawJson("tags"u8, "{\"a\":1}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("route"u8, "/v2/order"), global::Log.Field.Int64("bytes"u8, 512));
        logger2.Info(global::Log.Field.String("session"u8, "s1"), global::Log.Field.Int64("attempt"u8, 1), global::Log.Field.Bool("success"u8, true), global::Log.Field.UInt64("seq"u8, 7), global::Log.Field.Float64("elapsed"u8, 1.23), global::Log.Field.Utf8String("host"u8, "node-01"u8), global::Log.Field.RawJson("tags"u8, "{\"a\":1}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("route"u8, "/v2/order"), global::Log.Field.Int64("bytes"u8, 512), global::Log.Field.Bool("compressed"u8, false));
        logger2.Info(global::Log.Field.String("session"u8, "s1"), global::Log.Field.Int64("attempt"u8, 1), global::Log.Field.Bool("success"u8, true), global::Log.Field.UInt64("seq"u8, 7), global::Log.Field.Float64("elapsed"u8, 1.23), global::Log.Field.Utf8String("host"u8, "node-01"u8), global::Log.Field.RawJson("tags"u8, "{\"a\":1}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("route"u8, "/v2/order"), global::Log.Field.Int64("bytes"u8, 512), global::Log.Field.Bool("compressed"u8, false), global::Log.Field.UInt64("span_id"u8, 1234));
        logger2.Info(global::Log.Field.String("session"u8, "s1"), global::Log.Field.Int64("attempt"u8, 1), global::Log.Field.Bool("success"u8, true), global::Log.Field.UInt64("seq"u8, 7), global::Log.Field.Float64("elapsed"u8, 1.23), global::Log.Field.Utf8String("host"u8, "node-01"u8), global::Log.Field.RawJson("tags"u8, "{\"a\":1}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("route"u8, "/v2/order"), global::Log.Field.Int64("bytes"u8, 512), global::Log.Field.Bool("compressed"u8, false), global::Log.Field.UInt64("span_id"u8, 1234), global::Log.Field.Float64("cpu"u8, 0.12));
        logger2.Info(global::Log.Field.String("session"u8, "s1"), global::Log.Field.Int64("attempt"u8, 1), global::Log.Field.Bool("success"u8, true), global::Log.Field.UInt64("seq"u8, 7), global::Log.Field.Float64("elapsed"u8, 1.23), global::Log.Field.Utf8String("host"u8, "node-01"u8), global::Log.Field.RawJson("tags"u8, "{\"a\":1}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("route"u8, "/v2/order"), global::Log.Field.Int64("bytes"u8, 512), global::Log.Field.Bool("compressed"u8, false), global::Log.Field.UInt64("span_id"u8, 1234), global::Log.Field.Float64("cpu"u8, 0.12), global::Log.Field.Utf8String("az"u8, "us-east-1a"u8));
        logger2.Info(global::Log.Field.String("session"u8, "s1"), global::Log.Field.Int64("attempt"u8, 1), global::Log.Field.Bool("success"u8, true), global::Log.Field.UInt64("seq"u8, 7), global::Log.Field.Float64("elapsed"u8, 1.23), global::Log.Field.Utf8String("host"u8, "node-01"u8), global::Log.Field.RawJson("tags"u8, "{\"a\":1}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("route"u8, "/v2/order"), global::Log.Field.Int64("bytes"u8, 512), global::Log.Field.Bool("compressed"u8, false), global::Log.Field.UInt64("span_id"u8, 1234), global::Log.Field.Float64("cpu"u8, 0.12), global::Log.Field.Utf8String("az"u8, "us-east-1a"u8), global::Log.Field.RawJson("payload"u8, "{\"ok\":true}"u8));
        logger2.Info(global::Log.Field.String("session"u8, "s1"), global::Log.Field.Int64("attempt"u8, 1), global::Log.Field.Bool("success"u8, true), global::Log.Field.UInt64("seq"u8, 7), global::Log.Field.Float64("elapsed"u8, 1.23), global::Log.Field.Utf8String("host"u8, "node-01"u8), global::Log.Field.RawJson("tags"u8, "{\"a\":1}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("route"u8, "/v2/order"), global::Log.Field.Int64("bytes"u8, 512), global::Log.Field.Bool("compressed"u8, false), global::Log.Field.UInt64("span_id"u8, 1234), global::Log.Field.Float64("cpu"u8, 0.12), global::Log.Field.Utf8String("az"u8, "us-east-1a"u8), global::Log.Field.RawJson("payload"u8, "{\"ok\":true}"u8), global::Log.Field.String("client"u8, "mobile"));
        logger2.Info(global::Log.Field.String("session"u8, "s1"), global::Log.Field.Int64("attempt"u8, 1), global::Log.Field.Bool("success"u8, true), global::Log.Field.UInt64("seq"u8, 7), global::Log.Field.Float64("elapsed"u8, 1.23), global::Log.Field.Utf8String("host"u8, "node-01"u8), global::Log.Field.RawJson("tags"u8, "{\"a\":1}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("route"u8, "/v2/order"), global::Log.Field.Int64("bytes"u8, 512), global::Log.Field.Bool("compressed"u8, false), global::Log.Field.UInt64("span_id"u8, 1234), global::Log.Field.Float64("cpu"u8, 0.12), global::Log.Field.Utf8String("az"u8, "us-east-1a"u8), global::Log.Field.RawJson("payload"u8, "{\"ok\":true}"u8), global::Log.Field.String("client"u8, "mobile"), global::Log.Field.Int64("status"u8, 201));
        logger2.Info(global::Log.Field.String("session"u8, "s1"), global::Log.Field.Int64("attempt"u8, 1), global::Log.Field.Bool("success"u8, true), global::Log.Field.UInt64("seq"u8, 7), global::Log.Field.Float64("elapsed"u8, 1.23), global::Log.Field.Utf8String("host"u8, "node-01"u8), global::Log.Field.RawJson("tags"u8, "{\"a\":1}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("route"u8, "/v2/order"), global::Log.Field.Int64("bytes"u8, 512), global::Log.Field.Bool("compressed"u8, false), global::Log.Field.UInt64("span_id"u8, 1234), global::Log.Field.Float64("cpu"u8, 0.12), global::Log.Field.Utf8String("az"u8, "us-east-1a"u8), global::Log.Field.RawJson("payload"u8, "{\"ok\":true}"u8), global::Log.Field.String("client"u8, "mobile"), global::Log.Field.Int64("status"u8, 201), global::Log.Field.Bool("retry"u8, false));
        logger2.Info(global::Log.Field.String("session"u8, "s1"), global::Log.Field.Int64("attempt"u8, 1), global::Log.Field.Bool("success"u8, true), global::Log.Field.UInt64("seq"u8, 7), global::Log.Field.Float64("elapsed"u8, 1.23), global::Log.Field.Utf8String("host"u8, "node-01"u8), global::Log.Field.RawJson("tags"u8, "{\"a\":1}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("route"u8, "/v2/order"), global::Log.Field.Int64("bytes"u8, 512), global::Log.Field.Bool("compressed"u8, false), global::Log.Field.UInt64("span_id"u8, 1234), global::Log.Field.Float64("cpu"u8, 0.12), global::Log.Field.Utf8String("az"u8, "us-east-1a"u8), global::Log.Field.RawJson("payload"u8, "{\"ok\":true}"u8), global::Log.Field.String("client"u8, "mobile"), global::Log.Field.Int64("status"u8, 201), global::Log.Field.Bool("retry"u8, false), global::Log.Field.UInt64("build"u8, 20240202));
        logger2.Info(global::Log.Field.String("session"u8, "s1"), global::Log.Field.Int64("attempt"u8, 1), global::Log.Field.Bool("success"u8, true), global::Log.Field.UInt64("seq"u8, 7), global::Log.Field.Float64("elapsed"u8, 1.23), global::Log.Field.Utf8String("host"u8, "node-01"u8), global::Log.Field.RawJson("tags"u8, "{\"a\":1}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("route"u8, "/v2/order"), global::Log.Field.Int64("bytes"u8, 512), global::Log.Field.Bool("compressed"u8, false), global::Log.Field.UInt64("span_id"u8, 1234), global::Log.Field.Float64("cpu"u8, 0.12), global::Log.Field.Utf8String("az"u8, "us-east-1a"u8), global::Log.Field.RawJson("payload"u8, "{\"ok\":true}"u8), global::Log.Field.String("client"u8, "mobile"), global::Log.Field.Int64("status"u8, 201), global::Log.Field.Bool("retry"u8, false), global::Log.Field.UInt64("build"u8, 20240202), global::Log.Field.Float64("temp"u8, 35.8));
        logger2.Warn(global::Log.Field.String("warn1"u8, "this is warn log"));
        logger2.Warn(global::Log.Field.String("module"u8, "cache"), global::Log.Field.Int64("code"u8, 120));
        logger2.Warn(global::Log.Field.String("module"u8, "cache"), global::Log.Field.Int64("code"u8, 120), global::Log.Field.Bool("stale"u8, true));
        logger2.Warn(global::Log.Field.String("module"u8, "cache"), global::Log.Field.Int64("code"u8, 120), global::Log.Field.Bool("stale"u8, true), global::Log.Field.UInt64("req_id"u8, 88));
        logger2.Warn(global::Log.Field.String("module"u8, "cache"), global::Log.Field.Int64("code"u8, 120), global::Log.Field.Bool("stale"u8, true), global::Log.Field.UInt64("req_id"u8, 88), global::Log.Field.Float64("ratio"u8, 0.81));
        logger2.Warn(global::Log.Field.String("module"u8, "cache"), global::Log.Field.Int64("code"u8, 120), global::Log.Field.Bool("stale"u8, true), global::Log.Field.UInt64("req_id"u8, 88), global::Log.Field.Float64("ratio"u8, 0.81), global::Log.Field.Utf8String("node"u8, "edge-7"u8));
        logger2.Warn(global::Log.Field.String("module"u8, "cache"), global::Log.Field.Int64("code"u8, 120), global::Log.Field.Bool("stale"u8, true), global::Log.Field.UInt64("req_id"u8, 88), global::Log.Field.Float64("ratio"u8, 0.81), global::Log.Field.Utf8String("node"u8, "edge-7"u8), global::Log.Field.RawJson("extra"u8, "{\"source\":\"timer\"}"));
        logger2.Warn(global::Log.Field.String("module"u8, "cache"), global::Log.Field.Int64("code"u8, 120), global::Log.Field.Bool("stale"u8, true), global::Log.Field.UInt64("req_id"u8, 88), global::Log.Field.Float64("ratio"u8, 0.81), global::Log.Field.Utf8String("node"u8, "edge-7"u8), global::Log.Field.RawJson("extra"u8, "{\"source\":\"timer\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch));
        logger2.Warn(global::Log.Field.String("module"u8, "cache"), global::Log.Field.Int64("code"u8, 120), global::Log.Field.Bool("stale"u8, true), global::Log.Field.UInt64("req_id"u8, 88), global::Log.Field.Float64("ratio"u8, 0.81), global::Log.Field.Utf8String("node"u8, "edge-7"u8), global::Log.Field.RawJson("extra"u8, "{\"source\":\"timer\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("path"u8, "/cache/refresh"));
        logger2.Warn(global::Log.Field.String("module"u8, "cache"), global::Log.Field.Int64("code"u8, 120), global::Log.Field.Bool("stale"u8, true), global::Log.Field.UInt64("req_id"u8, 88), global::Log.Field.Float64("ratio"u8, 0.81), global::Log.Field.Utf8String("node"u8, "edge-7"u8), global::Log.Field.RawJson("extra"u8, "{\"source\":\"timer\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("path"u8, "/cache/refresh"), global::Log.Field.Int64("size"u8, 4096));
        logger2.Warn(global::Log.Field.String("module"u8, "cache"), global::Log.Field.Int64("code"u8, 120), global::Log.Field.Bool("stale"u8, true), global::Log.Field.UInt64("req_id"u8, 88), global::Log.Field.Float64("ratio"u8, 0.81), global::Log.Field.Utf8String("node"u8, "edge-7"u8), global::Log.Field.RawJson("extra"u8, "{\"source\":\"timer\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("path"u8, "/cache/refresh"), global::Log.Field.Int64("size"u8, 4096), global::Log.Field.Bool("retry"u8, false));
        logger2.Warn(global::Log.Field.String("module"u8, "cache"), global::Log.Field.Int64("code"u8, 120), global::Log.Field.Bool("stale"u8, true), global::Log.Field.UInt64("req_id"u8, 88), global::Log.Field.Float64("ratio"u8, 0.81), global::Log.Field.Utf8String("node"u8, "edge-7"u8), global::Log.Field.RawJson("extra"u8, "{\"source\":\"timer\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("path"u8, "/cache/refresh"), global::Log.Field.Int64("size"u8, 4096), global::Log.Field.Bool("retry"u8, false), global::Log.Field.UInt64("trace"u8, 123456));
        logger2.Warn(global::Log.Field.String("module"u8, "cache"), global::Log.Field.Int64("code"u8, 120), global::Log.Field.Bool("stale"u8, true), global::Log.Field.UInt64("req_id"u8, 88), global::Log.Field.Float64("ratio"u8, 0.81), global::Log.Field.Utf8String("node"u8, "edge-7"u8), global::Log.Field.RawJson("extra"u8, "{\"source\":\"timer\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("path"u8, "/cache/refresh"), global::Log.Field.Int64("size"u8, 4096), global::Log.Field.Bool("retry"u8, false), global::Log.Field.UInt64("trace"u8, 123456), global::Log.Field.Float64("load"u8, 0.42));
        logger2.Warn(global::Log.Field.String("module"u8, "cache"), global::Log.Field.Int64("code"u8, 120), global::Log.Field.Bool("stale"u8, true), global::Log.Field.UInt64("req_id"u8, 88), global::Log.Field.Float64("ratio"u8, 0.81), global::Log.Field.Utf8String("node"u8, "edge-7"u8), global::Log.Field.RawJson("extra"u8, "{\"source\":\"timer\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("path"u8, "/cache/refresh"), global::Log.Field.Int64("size"u8, 4096), global::Log.Field.Bool("retry"u8, false), global::Log.Field.UInt64("trace"u8, 123456), global::Log.Field.Float64("load"u8, 0.42), global::Log.Field.Utf8String("region"u8, "ap-south-1"u8));
        logger2.Warn(global::Log.Field.String("module"u8, "cache"), global::Log.Field.Int64("code"u8, 120), global::Log.Field.Bool("stale"u8, true), global::Log.Field.UInt64("req_id"u8, 88), global::Log.Field.Float64("ratio"u8, 0.81), global::Log.Field.Utf8String("node"u8, "edge-7"u8), global::Log.Field.RawJson("extra"u8, "{\"source\":\"timer\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("path"u8, "/cache/refresh"), global::Log.Field.Int64("size"u8, 4096), global::Log.Field.Bool("retry"u8, false), global::Log.Field.UInt64("trace"u8, 123456), global::Log.Field.Float64("load"u8, 0.42), global::Log.Field.Utf8String("region"u8, "ap-south-1"u8), global::Log.Field.RawJson("payload"u8, "{\"ok\":true}"u8));
        logger2.Warn(global::Log.Field.String("module"u8, "cache"), global::Log.Field.Int64("code"u8, 120), global::Log.Field.Bool("stale"u8, true), global::Log.Field.UInt64("req_id"u8, 88), global::Log.Field.Float64("ratio"u8, 0.81), global::Log.Field.Utf8String("node"u8, "edge-7"u8), global::Log.Field.RawJson("extra"u8, "{\"source\":\"timer\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("path"u8, "/cache/refresh"), global::Log.Field.Int64("size"u8, 4096), global::Log.Field.Bool("retry"u8, false), global::Log.Field.UInt64("trace"u8, 123456), global::Log.Field.Float64("load"u8, 0.42), global::Log.Field.Utf8String("region"u8, "ap-south-1"u8), global::Log.Field.RawJson("payload"u8, "{\"ok\":true}"u8), global::Log.Field.String("user"u8, "svc-cache"));
        logger2.Warn(global::Log.Field.String("module"u8, "cache"), global::Log.Field.Int64("code"u8, 120), global::Log.Field.Bool("stale"u8, true), global::Log.Field.UInt64("req_id"u8, 88), global::Log.Field.Float64("ratio"u8, 0.81), global::Log.Field.Utf8String("node"u8, "edge-7"u8), global::Log.Field.RawJson("extra"u8, "{\"source\":\"timer\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("path"u8, "/cache/refresh"), global::Log.Field.Int64("size"u8, 4096), global::Log.Field.Bool("retry"u8, false), global::Log.Field.UInt64("trace"u8, 123456), global::Log.Field.Float64("load"u8, 0.42), global::Log.Field.Utf8String("region"u8, "ap-south-1"u8), global::Log.Field.RawJson("payload"u8, "{\"ok\":true}"u8), global::Log.Field.String("user"u8, "svc-cache"), global::Log.Field.Int64("limit"u8, 5));
        logger2.Warn(global::Log.Field.String("module"u8, "cache"), global::Log.Field.Int64("code"u8, 120), global::Log.Field.Bool("stale"u8, true), global::Log.Field.UInt64("req_id"u8, 88), global::Log.Field.Float64("ratio"u8, 0.81), global::Log.Field.Utf8String("node"u8, "edge-7"u8), global::Log.Field.RawJson("extra"u8, "{\"source\":\"timer\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("path"u8, "/cache/refresh"), global::Log.Field.Int64("size"u8, 4096), global::Log.Field.Bool("retry"u8, false), global::Log.Field.UInt64("trace"u8, 123456), global::Log.Field.Float64("load"u8, 0.42), global::Log.Field.Utf8String("region"u8, "ap-south-1"u8), global::Log.Field.RawJson("payload"u8, "{\"ok\":true}"u8), global::Log.Field.String("user"u8, "svc-cache"), global::Log.Field.Int64("limit"u8, 5), global::Log.Field.Bool("throttle"u8, true));
        logger2.Warn(global::Log.Field.String("module"u8, "cache"), global::Log.Field.Int64("code"u8, 120), global::Log.Field.Bool("stale"u8, true), global::Log.Field.UInt64("req_id"u8, 88), global::Log.Field.Float64("ratio"u8, 0.81), global::Log.Field.Utf8String("node"u8, "edge-7"u8), global::Log.Field.RawJson("extra"u8, "{\"source\":\"timer\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("path"u8, "/cache/refresh"), global::Log.Field.Int64("size"u8, 4096), global::Log.Field.Bool("retry"u8, false), global::Log.Field.UInt64("trace"u8, 123456), global::Log.Field.Float64("load"u8, 0.42), global::Log.Field.Utf8String("region"u8, "ap-south-1"u8), global::Log.Field.RawJson("payload"u8, "{\"ok\":true}"u8), global::Log.Field.String("user"u8, "svc-cache"), global::Log.Field.Int64("limit"u8, 5), global::Log.Field.Bool("throttle"u8, true), global::Log.Field.UInt64("build"u8, 20240301));
        logger2.Warn(global::Log.Field.String("module"u8, "cache"), global::Log.Field.Int64("code"u8, 120), global::Log.Field.Bool("stale"u8, true), global::Log.Field.UInt64("req_id"u8, 88), global::Log.Field.Float64("ratio"u8, 0.81), global::Log.Field.Utf8String("node"u8, "edge-7"u8), global::Log.Field.RawJson("extra"u8, "{\"source\":\"timer\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("path"u8, "/cache/refresh"), global::Log.Field.Int64("size"u8, 4096), global::Log.Field.Bool("retry"u8, false), global::Log.Field.UInt64("trace"u8, 123456), global::Log.Field.Float64("load"u8, 0.42), global::Log.Field.Utf8String("region"u8, "ap-south-1"u8), global::Log.Field.RawJson("payload"u8, "{\"ok\":true}"u8), global::Log.Field.String("user"u8, "svc-cache"), global::Log.Field.Int64("limit"u8, 5), global::Log.Field.Bool("throttle"u8, true), global::Log.Field.UInt64("build"u8, 20240301), global::Log.Field.Float64("temp"u8, 41.2));
        logger2.Error(global::Log.Field.String("error1"u8, "this is error log"));
        logger2.Error(global::Log.Field.String("service"u8, "auth"), global::Log.Field.Int64("err_code"u8, 401));
        logger2.Error(global::Log.Field.String("service"u8, "auth"), global::Log.Field.Int64("err_code"u8, 401), global::Log.Field.Bool("locked"u8, true));
        logger2.Error(global::Log.Field.String("service"u8, "auth"), global::Log.Field.Int64("err_code"u8, 401), global::Log.Field.Bool("locked"u8, true), global::Log.Field.UInt64("uid"u8, 1001));
        logger2.Error(global::Log.Field.String("service"u8, "auth"), global::Log.Field.Int64("err_code"u8, 401), global::Log.Field.Bool("locked"u8, true), global::Log.Field.UInt64("uid"u8, 1001), global::Log.Field.Float64("latency"u8, 12.8));
        logger2.Error(global::Log.Field.String("service"u8, "auth"), global::Log.Field.Int64("err_code"u8, 401), global::Log.Field.Bool("locked"u8, true), global::Log.Field.UInt64("uid"u8, 1001), global::Log.Field.Float64("latency"u8, 12.8), global::Log.Field.Utf8String("ip"u8, "192.168.1.9"u8));
        logger2.Error(global::Log.Field.String("service"u8, "auth"), global::Log.Field.Int64("err_code"u8, 401), global::Log.Field.Bool("locked"u8, true), global::Log.Field.UInt64("uid"u8, 1001), global::Log.Field.Float64("latency"u8, 12.8), global::Log.Field.Utf8String("ip"u8, "192.168.1.9"u8), global::Log.Field.RawJson("detail"u8, "{\"reason\":\"expired\"}"));
        logger2.Error(global::Log.Field.String("service"u8, "auth"), global::Log.Field.Int64("err_code"u8, 401), global::Log.Field.Bool("locked"u8, true), global::Log.Field.UInt64("uid"u8, 1001), global::Log.Field.Float64("latency"u8, 12.8), global::Log.Field.Utf8String("ip"u8, "192.168.1.9"u8), global::Log.Field.RawJson("detail"u8, "{\"reason\":\"expired\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch));
        logger2.Error(global::Log.Field.String("service"u8, "auth"), global::Log.Field.Int64("err_code"u8, 401), global::Log.Field.Bool("locked"u8, true), global::Log.Field.UInt64("uid"u8, 1001), global::Log.Field.Float64("latency"u8, 12.8), global::Log.Field.Utf8String("ip"u8, "192.168.1.9"u8), global::Log.Field.RawJson("detail"u8, "{\"reason\":\"expired\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("route"u8, "/login"));
        logger2.Error(global::Log.Field.String("service"u8, "auth"), global::Log.Field.Int64("err_code"u8, 401), global::Log.Field.Bool("locked"u8, true), global::Log.Field.UInt64("uid"u8, 1001), global::Log.Field.Float64("latency"u8, 12.8), global::Log.Field.Utf8String("ip"u8, "192.168.1.9"u8), global::Log.Field.RawJson("detail"u8, "{\"reason\":\"expired\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("route"u8, "/login"), global::Log.Field.Int64("attempt"u8, 3));
        logger2.Error(global::Log.Field.String("service"u8, "auth"), global::Log.Field.Int64("err_code"u8, 401), global::Log.Field.Bool("locked"u8, true), global::Log.Field.UInt64("uid"u8, 1001), global::Log.Field.Float64("latency"u8, 12.8), global::Log.Field.Utf8String("ip"u8, "192.168.1.9"u8), global::Log.Field.RawJson("detail"u8, "{\"reason\":\"expired\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("route"u8, "/login"), global::Log.Field.Int64("attempt"u8, 3), global::Log.Field.Bool("mfa"u8, false));
        logger2.Error(global::Log.Field.String("service"u8, "auth"), global::Log.Field.Int64("err_code"u8, 401), global::Log.Field.Bool("locked"u8, true), global::Log.Field.UInt64("uid"u8, 1001), global::Log.Field.Float64("latency"u8, 12.8), global::Log.Field.Utf8String("ip"u8, "192.168.1.9"u8), global::Log.Field.RawJson("detail"u8, "{\"reason\":\"expired\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("route"u8, "/login"), global::Log.Field.Int64("attempt"u8, 3), global::Log.Field.Bool("mfa"u8, false), global::Log.Field.UInt64("trace_id"u8, 987654));
        logger2.Error(global::Log.Field.String("service"u8, "auth"), global::Log.Field.Int64("err_code"u8, 401), global::Log.Field.Bool("locked"u8, true), global::Log.Field.UInt64("uid"u8, 1001), global::Log.Field.Float64("latency"u8, 12.8), global::Log.Field.Utf8String("ip"u8, "192.168.1.9"u8), global::Log.Field.RawJson("detail"u8, "{\"reason\":\"expired\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("route"u8, "/login"), global::Log.Field.Int64("attempt"u8, 3), global::Log.Field.Bool("mfa"u8, false), global::Log.Field.UInt64("trace_id"u8, 987654), global::Log.Field.Float64("cpu"u8, 0.77));
        logger2.Error(global::Log.Field.String("service"u8, "auth"), global::Log.Field.Int64("err_code"u8, 401), global::Log.Field.Bool("locked"u8, true), global::Log.Field.UInt64("uid"u8, 1001), global::Log.Field.Float64("latency"u8, 12.8), global::Log.Field.Utf8String("ip"u8, "192.168.1.9"u8), global::Log.Field.RawJson("detail"u8, "{\"reason\":\"expired\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("route"u8, "/login"), global::Log.Field.Int64("attempt"u8, 3), global::Log.Field.Bool("mfa"u8, false), global::Log.Field.UInt64("trace_id"u8, 987654), global::Log.Field.Float64("cpu"u8, 0.77), global::Log.Field.Utf8String("az"u8, "us-west-1b"u8));
        logger2.Error(global::Log.Field.String("service"u8, "auth"), global::Log.Field.Int64("err_code"u8, 401), global::Log.Field.Bool("locked"u8, true), global::Log.Field.UInt64("uid"u8, 1001), global::Log.Field.Float64("latency"u8, 12.8), global::Log.Field.Utf8String("ip"u8, "192.168.1.9"u8), global::Log.Field.RawJson("detail"u8, "{\"reason\":\"expired\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("route"u8, "/login"), global::Log.Field.Int64("attempt"u8, 3), global::Log.Field.Bool("mfa"u8, false), global::Log.Field.UInt64("trace_id"u8, 987654), global::Log.Field.Float64("cpu"u8, 0.77), global::Log.Field.Utf8String("az"u8, "us-west-1b"u8), global::Log.Field.RawJson("ctx"u8, "{\"k\":\"v\"}"u8));
        logger2.Error(global::Log.Field.String("service"u8, "auth"), global::Log.Field.Int64("err_code"u8, 401), global::Log.Field.Bool("locked"u8, true), global::Log.Field.UInt64("uid"u8, 1001), global::Log.Field.Float64("latency"u8, 12.8), global::Log.Field.Utf8String("ip"u8, "192.168.1.9"u8), global::Log.Field.RawJson("detail"u8, "{\"reason\":\"expired\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("route"u8, "/login"), global::Log.Field.Int64("attempt"u8, 3), global::Log.Field.Bool("mfa"u8, false), global::Log.Field.UInt64("trace_id"u8, 987654), global::Log.Field.Float64("cpu"u8, 0.77), global::Log.Field.Utf8String("az"u8, "us-west-1b"u8), global::Log.Field.RawJson("ctx"u8, "{\"k\":\"v\"}"u8), global::Log.Field.String("client"u8, "web"));
        logger2.Error(global::Log.Field.String("service"u8, "auth"), global::Log.Field.Int64("err_code"u8, 401), global::Log.Field.Bool("locked"u8, true), global::Log.Field.UInt64("uid"u8, 1001), global::Log.Field.Float64("latency"u8, 12.8), global::Log.Field.Utf8String("ip"u8, "192.168.1.9"u8), global::Log.Field.RawJson("detail"u8, "{\"reason\":\"expired\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("route"u8, "/login"), global::Log.Field.Int64("attempt"u8, 3), global::Log.Field.Bool("mfa"u8, false), global::Log.Field.UInt64("trace_id"u8, 987654), global::Log.Field.Float64("cpu"u8, 0.77), global::Log.Field.Utf8String("az"u8, "us-west-1b"u8), global::Log.Field.RawJson("ctx"u8, "{\"k\":\"v\"}"u8), global::Log.Field.String("client"u8, "web"), global::Log.Field.Int64("quota"u8, 100));
        logger2.Error(global::Log.Field.String("service"u8, "auth"), global::Log.Field.Int64("err_code"u8, 401), global::Log.Field.Bool("locked"u8, true), global::Log.Field.UInt64("uid"u8, 1001), global::Log.Field.Float64("latency"u8, 12.8), global::Log.Field.Utf8String("ip"u8, "192.168.1.9"u8), global::Log.Field.RawJson("detail"u8, "{\"reason\":\"expired\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("route"u8, "/login"), global::Log.Field.Int64("attempt"u8, 3), global::Log.Field.Bool("mfa"u8, false), global::Log.Field.UInt64("trace_id"u8, 987654), global::Log.Field.Float64("cpu"u8, 0.77), global::Log.Field.Utf8String("az"u8, "us-west-1b"u8), global::Log.Field.RawJson("ctx"u8, "{\"k\":\"v\"}"u8), global::Log.Field.String("client"u8, "web"), global::Log.Field.Int64("quota"u8, 100), global::Log.Field.Bool("recoverable"u8, true));
        logger2.Error(global::Log.Field.String("service"u8, "auth"), global::Log.Field.Int64("err_code"u8, 401), global::Log.Field.Bool("locked"u8, true), global::Log.Field.UInt64("uid"u8, 1001), global::Log.Field.Float64("latency"u8, 12.8), global::Log.Field.Utf8String("ip"u8, "192.168.1.9"u8), global::Log.Field.RawJson("detail"u8, "{\"reason\":\"expired\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("route"u8, "/login"), global::Log.Field.Int64("attempt"u8, 3), global::Log.Field.Bool("mfa"u8, false), global::Log.Field.UInt64("trace_id"u8, 987654), global::Log.Field.Float64("cpu"u8, 0.77), global::Log.Field.Utf8String("az"u8, "us-west-1b"u8), global::Log.Field.RawJson("ctx"u8, "{\"k\":\"v\"}"u8), global::Log.Field.String("client"u8, "web"), global::Log.Field.Int64("quota"u8, 100), global::Log.Field.Bool("recoverable"u8, true), global::Log.Field.UInt64("build"u8, 20240401));
        logger2.Error(global::Log.Field.String("service"u8, "auth"), global::Log.Field.Int64("err_code"u8, 401), global::Log.Field.Bool("locked"u8, true), global::Log.Field.UInt64("uid"u8, 1001), global::Log.Field.Float64("latency"u8, 12.8), global::Log.Field.Utf8String("ip"u8, "192.168.1.9"u8), global::Log.Field.RawJson("detail"u8, "{\"reason\":\"expired\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("route"u8, "/login"), global::Log.Field.Int64("attempt"u8, 3), global::Log.Field.Bool("mfa"u8, false), global::Log.Field.UInt64("trace_id"u8, 987654), global::Log.Field.Float64("cpu"u8, 0.77), global::Log.Field.Utf8String("az"u8, "us-west-1b"u8), global::Log.Field.RawJson("ctx"u8, "{\"k\":\"v\"}"u8), global::Log.Field.String("client"u8, "web"), global::Log.Field.Int64("quota"u8, 100), global::Log.Field.Bool("recoverable"u8, true), global::Log.Field.UInt64("build"u8, 20240401), global::Log.Field.Float64("temp"u8, 33.3));
        logger2.Fatal(global::Log.Field.String("fatal1"u8, "this is fatal log"));
        logger2.Fatal(global::Log.Field.String("component"u8, "db"), global::Log.Field.Int64("err"u8, 500));
        logger2.Fatal(global::Log.Field.String("component"u8, "db"), global::Log.Field.Int64("err"u8, 500), global::Log.Field.Bool("panic"u8, true));
        logger2.Fatal(global::Log.Field.String("component"u8, "db"), global::Log.Field.Int64("err"u8, 500), global::Log.Field.Bool("panic"u8, true), global::Log.Field.UInt64("conn_id"u8, 42));
        logger2.Fatal(global::Log.Field.String("component"u8, "db"), global::Log.Field.Int64("err"u8, 500), global::Log.Field.Bool("panic"u8, true), global::Log.Field.UInt64("conn_id"u8, 42), global::Log.Field.Float64("wait"u8, 3.14));
        logger2.Fatal(global::Log.Field.String("component"u8, "db"), global::Log.Field.Int64("err"u8, 500), global::Log.Field.Bool("panic"u8, true), global::Log.Field.UInt64("conn_id"u8, 42), global::Log.Field.Float64("wait"u8, 3.14), global::Log.Field.Utf8String("host"u8, "db-01"u8));
        logger2.Fatal(global::Log.Field.String("component"u8, "db"), global::Log.Field.Int64("err"u8, 500), global::Log.Field.Bool("panic"u8, true), global::Log.Field.UInt64("conn_id"u8, 42), global::Log.Field.Float64("wait"u8, 3.14), global::Log.Field.Utf8String("host"u8, "db-01"u8), global::Log.Field.RawJson("sql"u8, "{\"query\":\"select\"}"));
        logger2.Fatal(global::Log.Field.String("component"u8, "db"), global::Log.Field.Int64("err"u8, 500), global::Log.Field.Bool("panic"u8, true), global::Log.Field.UInt64("conn_id"u8, 42), global::Log.Field.Float64("wait"u8, 3.14), global::Log.Field.Utf8String("host"u8, "db-01"u8), global::Log.Field.RawJson("sql"u8, "{\"query\":\"select\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch));
        logger2.Fatal(global::Log.Field.String("component"u8, "db"), global::Log.Field.Int64("err"u8, 500), global::Log.Field.Bool("panic"u8, true), global::Log.Field.UInt64("conn_id"u8, 42), global::Log.Field.Float64("wait"u8, 3.14), global::Log.Field.Utf8String("host"u8, "db-01"u8), global::Log.Field.RawJson("sql"u8, "{\"query\":\"select\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("table"u8, "users"));
        logger2.Fatal(global::Log.Field.String("component"u8, "db"), global::Log.Field.Int64("err"u8, 500), global::Log.Field.Bool("panic"u8, true), global::Log.Field.UInt64("conn_id"u8, 42), global::Log.Field.Float64("wait"u8, 3.14), global::Log.Field.Utf8String("host"u8, "db-01"u8), global::Log.Field.RawJson("sql"u8, "{\"query\":\"select\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("table"u8, "users"), global::Log.Field.Int64("rows"u8, 0));
        logger2.Fatal(global::Log.Field.String("component"u8, "db"), global::Log.Field.Int64("err"u8, 500), global::Log.Field.Bool("panic"u8, true), global::Log.Field.UInt64("conn_id"u8, 42), global::Log.Field.Float64("wait"u8, 3.14), global::Log.Field.Utf8String("host"u8, "db-01"u8), global::Log.Field.RawJson("sql"u8, "{\"query\":\"select\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("table"u8, "users"), global::Log.Field.Int64("rows"u8, 0), global::Log.Field.Bool("readonly"u8, false));
        logger2.Fatal(global::Log.Field.String("component"u8, "db"), global::Log.Field.Int64("err"u8, 500), global::Log.Field.Bool("panic"u8, true), global::Log.Field.UInt64("conn_id"u8, 42), global::Log.Field.Float64("wait"u8, 3.14), global::Log.Field.Utf8String("host"u8, "db-01"u8), global::Log.Field.RawJson("sql"u8, "{\"query\":\"select\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("table"u8, "users"), global::Log.Field.Int64("rows"u8, 0), global::Log.Field.Bool("readonly"u8, false), global::Log.Field.UInt64("tx"u8, 777));
        logger2.Fatal(global::Log.Field.String("component"u8, "db"), global::Log.Field.Int64("err"u8, 500), global::Log.Field.Bool("panic"u8, true), global::Log.Field.UInt64("conn_id"u8, 42), global::Log.Field.Float64("wait"u8, 3.14), global::Log.Field.Utf8String("host"u8, "db-01"u8), global::Log.Field.RawJson("sql"u8, "{\"query\":\"select\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("table"u8, "users"), global::Log.Field.Int64("rows"u8, 0), global::Log.Field.Bool("readonly"u8, false), global::Log.Field.UInt64("tx"u8, 777), global::Log.Field.Float64("disk"u8, 0.91));
        logger2.Fatal(global::Log.Field.String("component"u8, "db"), global::Log.Field.Int64("err"u8, 500), global::Log.Field.Bool("panic"u8, true), global::Log.Field.UInt64("conn_id"u8, 42), global::Log.Field.Float64("wait"u8, 3.14), global::Log.Field.Utf8String("host"u8, "db-01"u8), global::Log.Field.RawJson("sql"u8, "{\"query\":\"select\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("table"u8, "users"), global::Log.Field.Int64("rows"u8, 0), global::Log.Field.Bool("readonly"u8, false), global::Log.Field.UInt64("tx"u8, 777), global::Log.Field.Float64("disk"u8, 0.91), global::Log.Field.Utf8String("region"u8, "eu-west-2"u8));
        logger2.Fatal(global::Log.Field.String("component"u8, "db"), global::Log.Field.Int64("err"u8, 500), global::Log.Field.Bool("panic"u8, true), global::Log.Field.UInt64("conn_id"u8, 42), global::Log.Field.Float64("wait"u8, 3.14), global::Log.Field.Utf8String("host"u8, "db-01"u8), global::Log.Field.RawJson("sql"u8, "{\"query\":\"select\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("table"u8, "users"), global::Log.Field.Int64("rows"u8, 0), global::Log.Field.Bool("readonly"u8, false), global::Log.Field.UInt64("tx"u8, 777), global::Log.Field.Float64("disk"u8, 0.91), global::Log.Field.Utf8String("region"u8, "eu-west-2"u8), global::Log.Field.RawJson("meta"u8, "{\"severity\":\"high\"}"u8));
        logger2.Fatal(global::Log.Field.String("component"u8, "db"), global::Log.Field.Int64("err"u8, 500), global::Log.Field.Bool("panic"u8, true), global::Log.Field.UInt64("conn_id"u8, 42), global::Log.Field.Float64("wait"u8, 3.14), global::Log.Field.Utf8String("host"u8, "db-01"u8), global::Log.Field.RawJson("sql"u8, "{\"query\":\"select\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("table"u8, "users"), global::Log.Field.Int64("rows"u8, 0), global::Log.Field.Bool("readonly"u8, false), global::Log.Field.UInt64("tx"u8, 777), global::Log.Field.Float64("disk"u8, 0.91), global::Log.Field.Utf8String("region"u8, "eu-west-2"u8), global::Log.Field.RawJson("meta"u8, "{\"severity\":\"high\"}"u8), global::Log.Field.String("cluster"u8, "primary"));
        logger2.Fatal(global::Log.Field.String("component"u8, "db"), global::Log.Field.Int64("err"u8, 500), global::Log.Field.Bool("panic"u8, true), global::Log.Field.UInt64("conn_id"u8, 42), global::Log.Field.Float64("wait"u8, 3.14), global::Log.Field.Utf8String("host"u8, "db-01"u8), global::Log.Field.RawJson("sql"u8, "{\"query\":\"select\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("table"u8, "users"), global::Log.Field.Int64("rows"u8, 0), global::Log.Field.Bool("readonly"u8, false), global::Log.Field.UInt64("tx"u8, 777), global::Log.Field.Float64("disk"u8, 0.91), global::Log.Field.Utf8String("region"u8, "eu-west-2"u8), global::Log.Field.RawJson("meta"u8, "{\"severity\":\"high\"}"u8), global::Log.Field.String("cluster"u8, "primary"), global::Log.Field.Int64("retry_after"u8, 60));
        logger2.Fatal(global::Log.Field.String("component"u8, "db"), global::Log.Field.Int64("err"u8, 500), global::Log.Field.Bool("panic"u8, true), global::Log.Field.UInt64("conn_id"u8, 42), global::Log.Field.Float64("wait"u8, 3.14), global::Log.Field.Utf8String("host"u8, "db-01"u8), global::Log.Field.RawJson("sql"u8, "{\"query\":\"select\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("table"u8, "users"), global::Log.Field.Int64("rows"u8, 0), global::Log.Field.Bool("readonly"u8, false), global::Log.Field.UInt64("tx"u8, 777), global::Log.Field.Float64("disk"u8, 0.91), global::Log.Field.Utf8String("region"u8, "eu-west-2"u8), global::Log.Field.RawJson("meta"u8, "{\"severity\":\"high\"}"u8), global::Log.Field.String("cluster"u8, "primary"), global::Log.Field.Int64("retry_after"u8, 60), global::Log.Field.Bool("alert"u8, true));
        logger2.Fatal(global::Log.Field.String("component"u8, "db"), global::Log.Field.Int64("err"u8, 500), global::Log.Field.Bool("panic"u8, true), global::Log.Field.UInt64("conn_id"u8, 42), global::Log.Field.Float64("wait"u8, 3.14), global::Log.Field.Utf8String("host"u8, "db-01"u8), global::Log.Field.RawJson("sql"u8, "{\"query\":\"select\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("table"u8, "users"), global::Log.Field.Int64("rows"u8, 0), global::Log.Field.Bool("readonly"u8, false), global::Log.Field.UInt64("tx"u8, 777), global::Log.Field.Float64("disk"u8, 0.91), global::Log.Field.Utf8String("region"u8, "eu-west-2"u8), global::Log.Field.RawJson("meta"u8, "{\"severity\":\"high\"}"u8), global::Log.Field.String("cluster"u8, "primary"), global::Log.Field.Int64("retry_after"u8, 60), global::Log.Field.Bool("alert"u8, true), global::Log.Field.UInt64("build"u8, 20240501));
        logger2.Fatal(global::Log.Field.String("component"u8, "db"), global::Log.Field.Int64("err"u8, 500), global::Log.Field.Bool("panic"u8, true), global::Log.Field.UInt64("conn_id"u8, 42), global::Log.Field.Float64("wait"u8, 3.14), global::Log.Field.Utf8String("host"u8, "db-01"u8), global::Log.Field.RawJson("sql"u8, "{\"query\":\"select\"}"), global::Log.Field.UtcDateTime("ts"u8, DateTime.UnixEpoch), global::Log.Field.String("table"u8, "users"), global::Log.Field.Int64("rows"u8, 0), global::Log.Field.Bool("readonly"u8, false), global::Log.Field.UInt64("tx"u8, 777), global::Log.Field.Float64("disk"u8, 0.91), global::Log.Field.Utf8String("region"u8, "eu-west-2"u8), global::Log.Field.RawJson("meta"u8, "{\"severity\":\"high\"}"u8), global::Log.Field.String("cluster"u8, "primary"), global::Log.Field.Int64("retry_after"u8, 60), global::Log.Field.Bool("alert"u8, true), global::Log.Field.UInt64("build"u8, 20240501), global::Log.Field.Float64("temp"u8, 55.5));
    }

    public TaskLoggerTests()
    {
        // Ensure Logger is initialized
        if (Logger.Instance != null)
        {
            try { Logger.Shutdown(); } catch { }
        }
        Logger.Init(
            level: LogLevel.Debug,
            flushIntervalMs: 1000,
            tags: new Dictionary<string, string> { },
            overload: OverloadPolicy.Direct,
            queueSize: 1,
            logBufferSize: 1024 * 4
        );
        output();
        Logger.SetLevel(LogLevel.Info);
        output();
        Logger.SetLevel(LogLevel.Warn);
        output();
        Logger.SetLevel(LogLevel.Error);
        output();
        Logger.SetLevel(LogLevel.Fatal);
        output();
        Thread.Sleep(1000);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        Thread.Sleep(1000);
    }

    public void Dispose()
    {
        if (Logger.Instance != null)
        {
            try { Logger.Shutdown(); } catch { }
        }
    }

    #region TestCase Structures
    public struct LogLevelTestCase
    {
        public string Name;
        public global::Log.LogLevel GlobalLevel;
        public string MethodToCall;
        public bool ShouldLog;
    }
    #endregion

    [Fact]
    public void Constructor_WithNoGlobalTags_InitializesProperly()
    {
        var logger = new TaskLogger();

        Assert.NotNull(logger);
    }

    [Fact]
    public void Constructor_WithGlobalTags_IncludesTagsInPrefix()
    {
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
    public void WithFields_SingleField_ReturnsNewLoggerWithField()
    {
        var logger = new TaskLogger();
        var field = Field.String("key"u8.ToArray(), "value");

        var newLogger = logger.WithFields(field);

        Assert.NotNull(newLogger);
        Assert.NotSame(logger, newLogger);
    }

    [Fact]
    public void WithFields_MultipleOverloads_ReturnNewLoggers()
    {
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
    public void WithFields_MoreOverloads_ReturnNewLoggers()
    {
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
    public void WithFields_HighFieldCount_ReturnNewLoggers()
    {
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
    public void Info_SingleField_LogsWithoutException()
    {
        var logger = new TaskLogger();

        // Should not throw
        logger.Info(Field.String("msg"u8.ToArray(), "test message"));
    }

    [Fact]
    public void Info_MultipleFields_LogsWithoutException()
    {
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
    public void Debug_SingleField_LogsWithoutException()
    {
        var logger = new TaskLogger();

        logger.Debug(Field.String("debug_msg"u8.ToArray(), "debug test"));
    }

    [Fact]
    public void Debug_MultipleFields_LogsWithoutException()
    {
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
    public void Warn_SingleField_LogsWithoutException()
    {
        var logger = new TaskLogger();

        logger.Warn(Field.String("warning"u8.ToArray(), "this is a warning"));
    }

    [Fact]
    public void Warn_MultipleFields_LogsWithoutException()
    {
        var logger = new TaskLogger();

        logger.Warn(
            Field.String("msg"u8.ToArray(), "warning"),
            Field.Int64("code"u8.ToArray(), 500)
        );
    }

    [Fact]
    public void Error_SingleField_LogsWithoutException()
    {
        var logger = new TaskLogger();

        logger.Error(Field.String("error"u8.ToArray(), "error occurred"));
    }

    [Fact]
    public void Error_MultipleFields_LogsWithoutException()
    {
        var logger = new TaskLogger();

        logger.Error(
            Field.String("msg"u8.ToArray(), "error"),
            Field.String("stack"u8.ToArray(), "stack trace here")
        );
    }

    [Fact]
    public void Fatal_SingleField_LogsWithoutException()
    {
        var logger = new TaskLogger();

        logger.Fatal(Field.String("fatal"u8.ToArray(), "fatal error"));
    }

    [Fact]
    public void Fatal_MultipleFields_LogsWithoutException()
    {
        var logger = new TaskLogger();

        logger.Fatal(
            Field.String("msg"u8.ToArray(), "fatal"),
            Field.Int64("exit_code"u8.ToArray(), 1)
        );
    }

    [Fact]
    public void LogLevel_InfoFiltering_SkipsWhenLevelTooLow()
    {
        // Reinitialize with Error level (lower than Info)
        Logger.Shutdown();
        Logger.Init(level: LogLevel.Warn);

        var logger = new TaskLogger();

        // This should be filtered out at Info level when global is Warn
        logger.Info(Field.String("msg"u8.ToArray(), "should not log"));

        // No exception should occur
    }

    [Fact]
    public void LogLevel_DebugFiltering_SkipsWhenLevelTooLow()
    {
        Logger.Shutdown();
        Logger.Init(level: LogLevel.Info);

        var logger = new TaskLogger();

        // Debug is higher than Info, so it should be filtered
        logger.Debug(Field.String("msg"u8.ToArray(), "should not log"));
    }

    [Fact]
    public void AllFieldTypes_InSingleLogCall()
    {
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
    public void WithFields_ChainedCalls_BuildsPrefix()
    {
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
    public void Info_HighFieldCount_LogsWithoutException()
    {
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
    public void SpecialCharacters_InFieldValues_AreEscaped()
    {
        var logger = new TaskLogger();

        // Test with special characters that need JSON escaping
        logger.Info(
            Field.String("msg"u8.ToArray(), "line1\nline2\ttab\"quote\\backslash")
        );
    }

    [Fact]
    public void Utf8String_InFields_WorksCorrectly()
    {
        var logger = new TaskLogger();

        logger.Info(
            Field.Utf8String("utf8msg"u8.ToArray(), "hello utf8"u8.ToArray())
        );
    }

    [Fact]
    public void RawJson_InFields_WorksCorrectly()
    {
        var logger = new TaskLogger();

        logger.Info(
            Field.RawJson("data"u8.ToArray(), "{\"key\":\"value\",\"num\":123}")
        );

        logger.Info(
            Field.RawJson("arr"u8.ToArray(), "[1,2,3,4,5]"u8.ToArray())
        );
    }
}
