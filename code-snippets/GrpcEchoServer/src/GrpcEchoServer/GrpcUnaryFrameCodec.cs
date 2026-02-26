using System;
using System.Buffers.Binary;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GrpcEchoServer;

/// <summary>
/// gRPC unary 帧编解码器，负责解析和编码长度前缀消息。
/// </summary>
internal static class GrpcUnaryFrameCodec {
    /// <summary>
    /// gRPC 帧头长度：1 字节压缩标志 + 4 字节消息长度。
    /// </summary>
    private const int GrpcHeaderLength = 5;

    /// <summary>
    /// 读取并解析一个 unary 请求消息。
    /// </summary>
    /// <param name="body">请求体流。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>解析结果。</returns>
    public static async Task<GrpcReadResult> TryReadSingleMessageAsync(Stream body, CancellationToken cancellationToken) {
        using var ms = new MemoryStream();
        await body.CopyToAsync(ms, cancellationToken);
        byte[] frameBytes = ms.ToArray();

        if (frameBytes.Length < GrpcHeaderLength) {
            return GrpcReadResult.Fail("invalid grpc frame: header length is too short.");
        }

        if (frameBytes[0] != 0) {
            return GrpcReadResult.Fail("compressed grpc message is unsupported.");
        }

        int messageLength = BinaryPrimitives.ReadInt32BigEndian(frameBytes.AsSpan(1, 4));
        if (messageLength < 0) {
            return GrpcReadResult.Fail("invalid grpc frame: negative message length.");
        }

        int payloadLength = frameBytes.Length - GrpcHeaderLength;
        if (messageLength != payloadLength) {
            return GrpcReadResult.Fail("invalid grpc frame: payload length mismatch.");
        }

        byte[] payload = frameBytes.AsSpan(GrpcHeaderLength, messageLength).ToArray();
        return GrpcReadResult.Success(payload);
    }

    /// <summary>
    /// 将 protobuf 负载编码成 gRPC unary 帧。
    /// </summary>
    /// <param name="payload">protobuf 序列化负载。</param>
    /// <returns>编码后的 gRPC 帧。</returns>
    public static byte[] EncodeSingleMessage(byte[] payload) {
        var frame = new byte[GrpcHeaderLength + payload.Length];
        frame[0] = 0;
        BinaryPrimitives.WriteInt32BigEndian(frame.AsSpan(1, 4), payload.Length);
        payload.CopyTo(frame.AsSpan(GrpcHeaderLength));
        return frame;
    }
}
