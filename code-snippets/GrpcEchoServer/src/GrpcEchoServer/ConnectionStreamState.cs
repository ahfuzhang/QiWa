namespace GrpcEchoServer;

/// <summary>
/// 连接级别的 stream 状态，记录最大 stream id 并执行并发安全校验。
/// </summary>
internal sealed class ConnectionStreamState {
    /// <summary>
    /// 并发访问保护锁。
    /// </summary>
    private readonly object _guard = new();

    /// <summary>
    /// 当前连接已接收的最大 stream id。
    /// </summary>
    private int _maxStreamId;

    /// <summary>
    /// 校验并推进最大 stream id。
    /// </summary>
    /// <param name="streamId">当前 stream id。</param>
    /// <returns>stream id 是否符合奇数且递增约束。</returns>
    public bool TryAdvance(int streamId) {
        lock (_guard) {
            if (streamId % 2 != 1 || streamId <= _maxStreamId) {
                return false;
            }

            _maxStreamId = streamId;
            return true;
        }
    }
}
