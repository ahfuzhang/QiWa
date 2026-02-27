namespace GrpcGeneicServer;

/// <summary>
/// 路由键格式化器，用于统一 service/method 的组合规则。
/// </summary>
internal static class RouteKeyFormatter {
    /// <summary>
    /// 生成服务方法路由键。
    /// </summary>
    /// <param name="serviceName">服务名称。</param>
    /// <param name="methodName">方法名称。</param>
    /// <returns>组合后的路由键。</returns>
    public static string Build(string serviceName, string methodName) {
        return serviceName + "/" + methodName;
    }

    /// <summary>
    /// 尝试从 gRPC 路径提取路由键。
    /// </summary>
    /// <param name="grpcMethodPath">gRPC 路径，格式通常为 /service/method。</param>
    /// <param name="routeKey">提取后的 service/method 路由键。</param>
    /// <returns>提取成功返回 true。</returns>
    public static bool TryParseGrpcPath(string grpcMethodPath, out string routeKey) {
        if (string.IsNullOrWhiteSpace(grpcMethodPath)) {
            routeKey = string.Empty;
            return false;
        }

        string normalized = grpcMethodPath[0] == '/' ? grpcMethodPath[1..] : grpcMethodPath;
        int splitIndex = normalized.IndexOf('/');
        if (splitIndex <= 0 || splitIndex == normalized.Length - 1) {
            routeKey = string.Empty;
            return false;
        }

        routeKey = normalized;
        return true;
    }
}
