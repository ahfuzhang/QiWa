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
using System.Threading.Tasks;
using Greet;
using Grpc.Net.Client;

/// <summary>
/// 演示 gRPC 客户端调用流程的程序入口类型。
/// </summary>
internal static class Program
{
    /// <summary>
    /// 按提示词意图将入口改为经典 Main() 形式，并支持通过命令行 -addr= 指定服务地址。
    /// </summary>
    /// <param name="args">命令行参数。</param>
    private static async Task Main(string[] args)
    {
        string address = GetAddress(args);
        using var channel = GrpcChannel.ForAddress(address);
        var client = new Greeter.GreeterClient(channel);

        var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
        Console.WriteLine("Greeting: " + reply.Message);

        Console.WriteLine("Shutting down");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
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
