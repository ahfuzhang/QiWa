using System;
using System.Buffers;
using System.CommandLine;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Security.AccessControl;
using System.Threading.Tasks;

internal static class Program {
    public static async Task<int> Main(string[] args) {
        var input = new Option<string?>("--input", "Set input file.");
        input.AddAlias("-input");
        var rootCommand = new RootCommand("configuration CLI.");
        rootCommand.AddOption(input);
        rootCommand.SetHandler(async (string? inputPath) => {
            // read file
            if (string.IsNullOrWhiteSpace(inputPath)) {
                Console.WriteLine("Input file is required.");
                return;
            }

            if (!await FileUtils.Utils.FileExistsAndNotEmptyAsync(inputPath)) {
                Console.WriteLine("Input file does not exist: {0}", inputPath);
                return;
            }

            var (data, error) = await FileUtils.Utils.ReadAllAndRentAync(inputPath);
            if (error.Err()) {
                Console.WriteLine($"read file error: {error.Message}");
                return;
            }
            // using (var _ = new Common.ScopeGuard(() => ArrayPool<byte>.Shared.Return(data!))) {
            //     Console.WriteLine("Loaded {0} bytes.", data!.Length);
            // }
            using (data) {
                Console.WriteLine("Loaded {0} bytes.", data.Length);
                // 开始做解析
            }
            Console.WriteLine("OK");
        }, input);
        return await rootCommand.InvokeAsync(args);
    }
}
