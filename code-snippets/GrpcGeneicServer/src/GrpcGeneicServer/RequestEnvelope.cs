using System.Buffers;

namespace GrpcGeneicServer;

/// <summary>
/// 原始请求信封，包含从请求体解析出的 service/method 与原始 payload。
/// </summary>
internal readonly struct RequestEnvelope {
    /// <summary>
    /// 业务服务名称，用于路由键的 service 部分。
    /// </summary>
    public string ServiceName { get; }

    /// <summary>
    /// 业务方法名称，用于路由键的 method 部分。
    /// </summary>
    public string MethodName { get; }

    /// <summary>
    /// 业务 payload 原始字节，保持未 decode 状态。
    /// </summary>
    public ReadOnlySequence<byte> Payload { get; }

    /// <summary>
    /// 初始化请求信封。
    /// </summary>
    /// <param name="serviceName">业务服务名称。</param>
    /// <param name="methodName">业务方法名称。</param>
    /// <param name="payload">业务 payload 原始字节。</param>
    public RequestEnvelope(string serviceName, string methodName, ReadOnlySequence<byte> payload) {
        ServiceName = serviceName;
        MethodName = methodName;
        Payload = payload;
    }
}
