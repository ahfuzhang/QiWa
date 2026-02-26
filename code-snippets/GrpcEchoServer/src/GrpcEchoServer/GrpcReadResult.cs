namespace GrpcEchoServer;

/// <summary>
/// gRPC 读取结果，用于描述解析是否成功及对应数据。
/// </summary>
internal sealed class GrpcReadResult {
    /// <summary>
    /// 指示解析是否成功。
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// 成功时返回的 protobuf 负载。
    /// </summary>
    public byte[] Payload { get; }

    /// <summary>
    /// 失败时返回的错误信息。
    /// </summary>
    public string ErrorMessage { get; }

    /// <summary>
    /// 初始化读取结果。
    /// </summary>
    /// <param name="isSuccess">是否成功。</param>
    /// <param name="payload">负载数据。</param>
    /// <param name="errorMessage">错误信息。</param>
    private GrpcReadResult(bool isSuccess, byte[] payload, string errorMessage) {
        IsSuccess = isSuccess;
        Payload = payload;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// 创建成功结果。
    /// </summary>
    /// <param name="payload">解析出的负载。</param>
    /// <returns>成功读取结果。</returns>
    public static GrpcReadResult Success(byte[] payload) {
        return new GrpcReadResult(true, payload, string.Empty);
    }

    /// <summary>
    /// 创建失败结果。
    /// </summary>
    /// <param name="errorMessage">失败原因。</param>
    /// <returns>失败读取结果。</returns>
    public static GrpcReadResult Fail(string errorMessage) {
        return new GrpcReadResult(false, [], errorMessage);
    }
}
