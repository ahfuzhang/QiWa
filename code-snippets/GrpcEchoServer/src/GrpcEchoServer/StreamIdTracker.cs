using System.Collections.Concurrent;

namespace GrpcEchoServer;

/// <summary>
/// StreamId 跟踪器，按连接维度维护已见到的最大 stream id。
/// </summary>
internal sealed class StreamIdTracker {
    /// <summary>
    /// 每个连接对应的 stream 状态集合。
    /// </summary>
    private readonly ConcurrentDictionary<string, ConnectionStreamState> _connectionStates = new();

    /// <summary>
    /// 校验并更新某个连接的 stream id。
    /// </summary>
    /// <param name="connectionId">连接标识。</param>
    /// <param name="streamId">当前请求 stream id。</param>
    /// <returns>stream id 是否合法。</returns>
    public bool ValidateAndUpdate(string connectionId, int streamId) {
        ConnectionStreamState state = _connectionStates.GetOrAdd(connectionId, _ => new ConnectionStreamState());
        return state.TryAdvance(streamId);
    }
}
