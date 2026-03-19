using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenCode.ProtoCodeGen;

/// <summary>
/// 使用 Roslyn 定位各 ProtoBuf 类的位置，
/// 通过字符串插入将 Reset() 方法写入源文件，保留原始格式不变。
/// </summary>
public class FileRewriter
{
    /// <summary>原始源代码字符串。</summary>
    private readonly string _sourceCode;

    /// <summary>
    /// 构造函数，接受待改写的原始源代码。
    /// </summary>
    /// <param name="sourceCode">原始 C# 源代码。</param>
    public FileRewriter(string sourceCode)
    {
        _sourceCode = sourceCode;
    }

    /// <summary>
    /// 向每个 ProtoBuf 类插入 Reset() 方法，返回改写后的完整源代码。
    /// 若某个类已有 Reset() 方法，则跳过不重复插入。
    /// </summary>
    /// <param name="classes">ProtoClassAnalyzer 返回的类信息列表。</param>
    /// <returns>插入 Reset() 方法后的完整源代码。</returns>
    public string InsertResetMethods(List<ProtoClassInfo> classes)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(_sourceCode);
        var root = syntaxTree.GetRoot();

        var classMap = classes.ToDictionary(c => c.ClassName);

        // 按位置倒序处理，确保前面的插入不影响后面类的偏移量
        var classDeclarations = root.DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .Where(c => classMap.ContainsKey(c.Identifier.Text))
            .OrderByDescending(c => c.SpanStart)
            .ToList();

        var sb = new StringBuilder(_sourceCode);

        foreach (var classDecl in classDeclarations)
        {
            var classInfo = classMap[classDecl.Identifier.Text];

            // 如果 Reset() 已存在则跳过
            bool hasReset = classDecl.Members
                .OfType<MethodDeclarationSyntax>()
                .Any(m => m.Identifier.Text == "Reset");
            if (hasReset)
            {
                Console.WriteLine($"  Skipping {classInfo.ClassName}: Reset() already exists.");
                continue;
            }

            // 在类的 } 之前插入 Reset() 方法
            int insertPos = classDecl.CloseBraceToken.SpanStart;
            string methodText = BuildResetMethodText(classInfo);
            sb.Insert(insertPos, methodText);
        }

        return sb.ToString();
    }

    /// <summary>
    /// 构建 Reset() 方法的文本，使用 8 个空格缩进（与 proto-net 生成代码一致）。
    /// </summary>
    /// <param name="classInfo">类信息，包含需要重置的成员列表。</param>
    /// <returns>格式化好的 Reset() 方法文本，末尾带换行。</returns>
    private static string BuildResetMethodText(ProtoClassInfo classInfo)
    {
        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine("        /// <summary>将所有 ProtoBuf 成员重置为默认值，可用于对象池复用。</summary>");
        sb.AppendLine("        public void Reset()");
        sb.AppendLine("        {");

        foreach (var member in classInfo.Members)
        {
            sb.AppendLine(System.Globalization.CultureInfo.InvariantCulture, $"            {member.ResetStatement}");
        }

        sb.AppendLine("        }");
        sb.AppendLine();
        return sb.ToString();
    }
}
