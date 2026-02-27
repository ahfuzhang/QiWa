using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace GrpcSimpleServer;

/// <summary>
/// Process entry point that hosts a minimal unary gRPC server.
/// Intent from the prompt: keep one-file code while showing custom routing,
/// custom request envelope deserialization, and an echo handler.
/// </summary>
internal static class Program {
    /// <summary>
    /// Strict UTF-8 decoder used by custom envelope decoding.
    /// </summary>
    private static readonly UTF8Encoding StrictUtf8 = new(false, true);

    /// <summary>
    /// Route table used by the custom router.
    /// </summary>
    private static readonly Dictionary<string, Func<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>>> Routes = new(StringComparer.Ordinal) {
        ["echo/raw"] = static payload => payload,
        ["greet.Greeter/SayHello"] = static payload => payload
    };

    /// <summary>
    /// Classic Main() startup entry.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    public static async Task Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.WebHost.ConfigureKestrel(options => {
            options.ListenAnyIP(5000, listenOptions => {
                listenOptions.Protocols = HttpProtocols.Http2;
            });
        });

        var app = builder.Build();
        app.MapPost("/{service}/{method}", HandleUnaryAsync);
        await app.RunAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Handles a unary gRPC request with custom frame decode, custom route dispatch,
    /// and custom frame encode.
    /// </summary>
    /// <param name="context">HTTP request context.</param>
    private static async Task HandleUnaryAsync(HttpContext context) {
        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "application/grpc";

        byte[] frameBytes = await ReadAllBytesAsync(context.Request.Body).ConfigureAwait(false);
        if (!TryReadGrpcPayload(frameBytes, out ReadOnlyMemory<byte> grpcPayload, out string frameError)) {
            await WriteGrpcErrorAsync(context, "3", frameError).ConfigureAwait(false);
            return;
        }

        string routeKey;
        ReadOnlyMemory<byte> handlerPayload;

        if (TryDecodeEnvelope(grpcPayload, out string envelopeRoute, out ReadOnlyMemory<byte> envelopePayload)) {
            routeKey = envelopeRoute;
            handlerPayload = envelopePayload;
        }
        else if (TryBuildRouteKeyFromPath(context, out string pathRoute)) {
            routeKey = pathRoute;
            handlerPayload = grpcPayload;
        }
        else {
            await WriteGrpcErrorAsync(context, "3", "No route found in envelope or path.").ConfigureAwait(false);
            return;
        }

        if (!Routes.TryGetValue(routeKey, out Func<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>>? handler)) {
            await WriteGrpcErrorAsync(context, "12", $"Route not found: {routeKey}").ConfigureAwait(false);
            return;
        }

        ReadOnlyMemory<byte> responsePayload = handler(handlerPayload);
        byte[] responseFrame = BuildGrpcFrame(responsePayload.Span);
        await context.Response.Body.WriteAsync(responseFrame, context.RequestAborted).ConfigureAwait(false);
        context.Response.AppendTrailer("grpc-status", "0");
    }

    /// <summary>
    /// Reads a complete stream into a byte array.
    /// </summary>
    /// <param name="stream">Input stream.</param>
    /// <returns>All bytes from the stream.</returns>
    private static async Task<byte[]> ReadAllBytesAsync(Stream stream) {
        using var buffer = new MemoryStream();
        await stream.CopyToAsync(buffer).ConfigureAwait(false);
        return buffer.ToArray();
    }

    /// <summary>
    /// Parses a unary gRPC frame: 1-byte compression flag + 4-byte big-endian payload length.
    /// </summary>
    /// <param name="frame">Raw frame bytes from HTTP body.</param>
    /// <param name="payload">Decoded payload bytes.</param>
    /// <param name="error">Error message when decoding fails.</param>
    /// <returns>True when decoding succeeds.</returns>
    private static bool TryReadGrpcPayload(byte[] frame, out ReadOnlyMemory<byte> payload, out string error) {
        payload = default;
        error = string.Empty;

        if (frame.Length < 5) {
            error = "Invalid gRPC frame: body is too short.";
            return false;
        }

        if (frame[0] != 0) {
            error = "Invalid gRPC frame: only uncompressed payload is supported.";
            return false;
        }

        int payloadLength = BinaryPrimitives.ReadInt32BigEndian(frame.AsSpan(1, 4));
        if (payloadLength < 0 || frame.Length != payloadLength + 5) {
            error = "Invalid gRPC frame: payload length mismatch.";
            return false;
        }

        payload = frame.AsMemory(5, payloadLength);
        return true;
    }

    /// <summary>
    /// Decodes custom envelope layout: [u16 serviceLen][service][u16 methodLen][method][payload].
    /// </summary>
    /// <param name="requestPayload">Payload bytes from gRPC frame.</param>
    /// <param name="routeKey">Decoded route key in service/method form.</param>
    /// <param name="businessPayload">Decoded business payload after route fields.</param>
    /// <returns>True when envelope is decoded successfully.</returns>
    private static bool TryDecodeEnvelope(ReadOnlyMemory<byte> requestPayload, out string routeKey, out ReadOnlyMemory<byte> businessPayload) {
        routeKey = string.Empty;
        businessPayload = default;

        ReadOnlySpan<byte> span = requestPayload.Span;
        if (span.Length < 4) {
            return false;
        }

        int cursor = 0;
        if (!TryReadToken(span, ref cursor, out string service)) {
            return false;
        }

        if (!TryReadToken(span, ref cursor, out string method)) {
            return false;
        }

        if (string.IsNullOrWhiteSpace(service) || string.IsNullOrWhiteSpace(method)) {
            return false;
        }

        routeKey = $"{service}/{method}";
        businessPayload = requestPayload.Slice(cursor);
        return true;
    }

    /// <summary>
    /// Reads one UTF-8 token with a two-byte big-endian length prefix.
    /// </summary>
    /// <param name="buffer">Input byte span.</param>
    /// <param name="cursor">Current read offset, advanced on success.</param>
    /// <param name="token">Decoded token value.</param>
    /// <returns>True when token is valid.</returns>
    private static bool TryReadToken(ReadOnlySpan<byte> buffer, ref int cursor, out string token) {
        token = string.Empty;

        if (buffer.Length - cursor < 2) {
            return false;
        }

        ushort length = BinaryPrimitives.ReadUInt16BigEndian(buffer.Slice(cursor, 2));
        cursor += 2;
        if (buffer.Length - cursor < length) {
            return false;
        }

        try {
            token = StrictUtf8.GetString(buffer.Slice(cursor, length));
        }
        catch (DecoderFallbackException) {
            return false;
        }

        cursor += length;
        return true;
    }

    /// <summary>
    /// Builds route key from URL path /{service}/{method}.
    /// </summary>
    /// <param name="context">HTTP request context.</param>
    /// <param name="routeKey">Built route key.</param>
    /// <returns>True when service and method values exist.</returns>
    private static bool TryBuildRouteKeyFromPath(HttpContext context, out string routeKey) {
        routeKey = string.Empty;
        string? service = context.Request.RouteValues["service"]?.ToString();
        string? method = context.Request.RouteValues["method"]?.ToString();

        if (string.IsNullOrWhiteSpace(service) || string.IsNullOrWhiteSpace(method)) {
            return false;
        }

        routeKey = $"{service}/{method}";
        return true;
    }

    /// <summary>
    /// Encodes unary payload into gRPC frame format.
    /// </summary>
    /// <param name="payload">Business payload bytes.</param>
    /// <returns>Encoded frame bytes.</returns>
    private static byte[] BuildGrpcFrame(ReadOnlySpan<byte> payload) {
        byte[] frame = new byte[payload.Length + 5];
        frame[0] = 0;
        BinaryPrimitives.WriteInt32BigEndian(frame.AsSpan(1, 4), payload.Length);
        payload.CopyTo(frame.AsSpan(5));
        return frame;
    }

    /// <summary>
    /// Writes a gRPC-style error trailer while keeping HTTP 200.
    /// </summary>
    /// <param name="context">HTTP request context.</param>
    /// <param name="status">gRPC status code string.</param>
    /// <param name="message">Error message.</param>
    private static async Task WriteGrpcErrorAsync(HttpContext context, string status, string message) {
        context.Response.AppendTrailer("grpc-status", status);
        context.Response.AppendTrailer("grpc-message", Uri.EscapeDataString(message));
        await context.Response.Body.FlushAsync(context.RequestAborted).ConfigureAwait(false);
    }
}
