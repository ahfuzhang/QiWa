using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Text;

namespace GrpcGeneicServer;

/// <summary>
/// 请求体解包器，按自定义二进制协议提取 service/method 并保留原始 payload。
/// </summary>
internal sealed class RequestEnvelopeDecoder {
    /// <summary>
    /// 严格 UTF-8 编码器，遇到非法字节时抛出异常，避免错误路由。
    /// </summary>
    private static readonly UTF8Encoding StrictUtf8 = new(false, true);

    /// <summary>
    /// 尝试将原始请求体解包为路由信息与 payload。
    /// </summary>
    /// <param name="request">原始请求字节。</param>
    /// <param name="envelope">解包结果。</param>
    /// <param name="errorMessage">失败时的错误信息。</param>
    /// <returns>解包成功返回 true。</returns>
    public bool TryDecode(ReadOnlySequence<byte> request, out RequestEnvelope envelope, out string? errorMessage) {
        var reader = new SequenceReader<byte>(request);

        if (!TryReadUtf8Token(ref reader, out string serviceName, out errorMessage)) {
            envelope = default;
            return false;
        }

        if (!TryReadUtf8Token(ref reader, out string methodName, out errorMessage)) {
            envelope = default;
            return false;
        }

        if (string.IsNullOrWhiteSpace(serviceName) || string.IsNullOrWhiteSpace(methodName)) {
            envelope = default;
            errorMessage = "Service and method names cannot be empty.";
            return false;
        }

        ReadOnlySequence<byte> payload = request.Slice(reader.Position);
        envelope = new RequestEnvelope(serviceName, methodName, payload);
        errorMessage = null;
        return true;
    }

    /// <summary>
    /// 读取一个长度前缀 UTF-8 字段。
    /// </summary>
    /// <param name="reader">序列读取器。</param>
    /// <param name="token">解析出的字符串值。</param>
    /// <param name="errorMessage">解析失败时的错误信息。</param>
    /// <returns>读取成功返回 true。</returns>
    private static bool TryReadUtf8Token(ref SequenceReader<byte> reader, out string token, out string? errorMessage) {
        if (!TryReadUInt16BigEndian(ref reader, out ushort tokenLength)) {
            token = string.Empty;
            errorMessage = "Invalid request envelope: missing length prefix.";
            return false;
        }

        if (reader.Remaining < tokenLength) {
            token = string.Empty;
            errorMessage = "Invalid request envelope: token bytes are incomplete.";
            return false;
        }

        ReadOnlySequence<byte> slice = reader.Sequence.Slice(reader.Position, tokenLength);
        try {
            token = DecodeUtf8(slice);
        }
        catch (DecoderFallbackException) {
            token = string.Empty;
            errorMessage = "Invalid request envelope: token is not valid UTF-8.";
            return false;
        }

        reader.Advance(tokenLength);
        errorMessage = null;
        return true;
    }

    /// <summary>
    /// 读取两个字节的大端无符号整数。
    /// </summary>
    /// <param name="reader">序列读取器。</param>
    /// <param name="value">解析后的长度值。</param>
    /// <returns>读取成功返回 true。</returns>
    private static bool TryReadUInt16BigEndian(ref SequenceReader<byte> reader, out ushort value) {
        Span<byte> raw = stackalloc byte[2];
        if (!reader.TryCopyTo(raw)) {
            value = 0;
            return false;
        }

        reader.Advance(2);
        value = BinaryPrimitives.ReadUInt16BigEndian(raw);
        return true;
    }

    /// <summary>
    /// 将 UTF-8 字节序列解码为字符串。
    /// </summary>
    /// <param name="sequence">待解码字节序列。</param>
    /// <returns>解码后的字符串。</returns>
    private static string DecodeUtf8(ReadOnlySequence<byte> sequence) {
        if (sequence.IsSingleSegment) {
            return StrictUtf8.GetString(sequence.First.Span);
        }

        byte[] bytes = sequence.ToArray();
        return StrictUtf8.GetString(bytes);
    }
}
