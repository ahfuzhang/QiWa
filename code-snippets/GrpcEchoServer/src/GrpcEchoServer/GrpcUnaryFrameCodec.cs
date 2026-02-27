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
    /// 读取请求体时使用的缓冲区大小。
    /// </summary>
    private const int ReadBufferSize = 16 * 1024;

    /// <summary>
    /// 读取并解析一个 unary 请求消息。
    /// </summary>
    /// <param name="body">请求体流。</param>
    /// <param name="contentLength">请求头声明的请求体长度。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>解析结果。</returns>
    public static async Task<GrpcReadResult> TryReadSingleMessageAsync(Stream body, int contentLength, CancellationToken cancellationToken) {
        if (contentLength < 0) {
            return GrpcReadResult.Fail("content-length must be non-negative.");
        }

        byte[] frameBytes = new byte[contentLength];
        bool readSucceeded = await TryReadByContentLengthAsync(body, frameBytes, cancellationToken);
        if (!readSucceeded) {
            return GrpcReadResult.Fail("invalid grpc frame: request body ended before content-length was satisfied.");
        }

        return TryDecodeSingleMessage(frameBytes);
    }

    /// <summary>
    /// 将完整帧字节解码为 protobuf 负载。
    /// </summary>
    /// <param name="frameBytes">完整 gRPC 帧字节。</param>
    /// <returns>解析结果。</returns>
    private static GrpcReadResult TryDecodeSingleMessage(byte[] frameBytes) {
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
    /// 按 Content-Length 读取请求体，避免额外的中转内存拷贝。
    /// </summary>
    /// <param name="body">请求体流。</param>
    /// <param name="bodyBytes">目标请求体缓冲区。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>是否完整读取到 Content-Length 指定的字节数。</returns>
    private static async Task<bool> TryReadByContentLengthAsync(Stream body, byte[] bodyBytes, CancellationToken cancellationToken) {
        // 根据提示词意图：调用方已拿到 Content-Length，这里按精确长度读取，减少不必要的复杂分支。
        int totalRead = 0;
        while (totalRead < bodyBytes.Length) {
            int remainingBytes = bodyBytes.Length - totalRead;
            int bytesToRead = Math.Min(ReadBufferSize, remainingBytes);
            int bytesRead = await body.ReadAsync(bodyBytes.AsMemory(totalRead, bytesToRead), cancellationToken);
            if (bytesRead == 0) {
                return false;
            }

            totalRead += bytesRead;
        }

        return true;
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
