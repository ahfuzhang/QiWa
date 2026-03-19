// 根据 prompt.md 的目标：
// 基于 Roslyn 解析 demo.cs，为继承自 global::ProtoBuf.IExtensible 的类
// 生成 Reset() 方法，将所有 ProtoMember 成员重置为类型默认值。

using GenCode.ProtoCodeGen;

namespace GenCode;

/// <summary>
/// 程序入口类，读取 .cs 文件并为 ProtoBuf 类注入 Reset() 方法。
/// </summary>
static class Program
{
    /// <summary>
    /// 主函数。接受 .cs 文件路径作为参数；若未提供，则使用默认路径 ../gen/demo.cs。
    /// </summary>
    static void Main(string[] args)
    {
        string inputFile = args.Length > 0 ? args[0] : "../gen/demo.cs";

        if (!File.Exists(inputFile))
        {
            Console.Error.WriteLine($"error: file not found: {inputFile}");
            Environment.Exit(1);
        }

        Console.WriteLine($"Processing: {Path.GetFullPath(inputFile)}");

        string sourceCode = File.ReadAllText(inputFile);

        var analyzer = new ProtoClassAnalyzer(sourceCode);
        var classes = analyzer.FindProtoClasses();

        if (classes.Count == 0)
        {
            Console.WriteLine("No ProtoBuf classes found.");
            return;
        }

        Console.WriteLine($"Found {classes.Count} ProtoBuf class(es):");
        foreach (var cls in classes)
        {
            Console.WriteLine($"  {cls.Namespace}.{cls.ClassName} ({cls.Members.Count} members)");
        }

        var rewriter = new FileRewriter(sourceCode);
        string newSource = rewriter.InsertResetMethods(classes);

        File.WriteAllText(inputFile, newSource);
        Console.WriteLine($"Done. Updated file: {inputFile}");
    }
}
