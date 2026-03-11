#region Copyright notice and license

// Copyright 2019 The gRPC Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using Greet;
using Grpc.Core;
using Grpc.Net.Client;
using System.Threading.Tasks;
using Grpc.Net.Compression;

/// <summary>
/// 演示 gRPC 客户端调用流程的程序入口类型。
/// </summary>
internal static class Program
{
    /// <summary>
    /// 按提示词意图将入口改为经典 Main() 形式，并支持通过命令行 -addr= 指定服务地址，
    /// 以及 -use.gzip=true/false、-use.zstd=true/false 控制请求压缩算法。
    /// </summary>
    /// <param name="args">命令行参数。</param>
    private static async Task Main(string[] args)
    {
        string address = GetAddress(args);
        bool useGzip = GetUseGzip(args);
        bool useZstd = GetUseZstd(args);

        var channelOptions = new GrpcChannelOptions();
        if (useGzip || useZstd)
        {
            channelOptions.CompressionProviders = new List<ICompressionProvider>
            {
                new GzipCompressionProvider(CompressionLevel.Optimal),
                new ZstdCompressionProvider(),
            };
        }

        using var channel = GrpcChannel.ForAddress(address, channelOptions);
        var client = new Greeter.GreeterClient(channel);

        CallOptions callOptions = default;
        string? encodingName = useZstd ? "zstd" : useGzip ? "gzip" : null;
        if (encodingName is not null)
        {
            var headers = new Metadata
            {
                { "grpc-internal-encoding-request", encodingName },
            };
            callOptions = new CallOptions(headers: headers);
        }

        // 生成约 1KB 的重复字符串，用于测试 gzip 压缩效果
        string longName = string.Concat(Enumerable.Repeat("GreeterClient_TestGzipCompression_0123456789ABCDEF_", 21));

        var reply = await client.SayHelloAsync(new HelloRequest { Name = longName }, callOptions);
        Console.WriteLine("Greeting: " + reply.Message);

        Console.WriteLine("Shutting down");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    /// <summary>
    /// 解析命令行中的 -use.gzip= 参数，未提供时默认不启用压缩。
    /// </summary>
    private static bool GetUseGzip(string[] args) => GetBoolArg(args, "-use.gzip=");

    /// <summary>
    /// 解析命令行中的 -use.zstd= 参数，未提供时默认不启用压缩。
    /// </summary>
    private static bool GetUseZstd(string[] args) => GetBoolArg(args, "-use.zstd=");

    private static bool GetBoolArg(string[] args, string prefix)
    {
        foreach (string arg in args)
        {
            if (arg.StartsWith(prefix, StringComparison.Ordinal))
            {
                return string.Equals(arg[prefix.Length..], "true", StringComparison.OrdinalIgnoreCase);
            }
        }

        return false;
    }

    /// <summary>
    /// 按提示词意图解析命令行中的 -addr= 参数，未提供时使用默认地址。
    /// </summary>
    /// <param name="args">命令行参数。</param>
    /// <returns>用于连接 gRPC 服务的地址。</returns>
    private static string GetAddress(string[] args)
    {
        const string AddressPrefix = "-addr=";

        foreach (string arg in args)
        {
            if (arg.StartsWith(AddressPrefix, StringComparison.Ordinal))
            {
                string value = arg[AddressPrefix.Length..];
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }
        }

        return "https://localhost:5001";
    }
}
