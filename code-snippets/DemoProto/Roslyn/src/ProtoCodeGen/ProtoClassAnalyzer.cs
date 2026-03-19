using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenCode.ProtoCodeGen;

/// <summary>
/// 使用 Roslyn 解析 C# 源代码，找出继承自 global::ProtoBuf.IExtensible 的类，
/// 并提取其中带有 ProtoMember 属性的成员。
/// </summary>
public class ProtoClassAnalyzer
{
    /// <summary>已解析的语法树。</summary>
    private readonly SyntaxTree _syntaxTree;

    /// <summary>
    /// 构造函数，接受待分析的 C# 源代码字符串。
    /// </summary>
    /// <param name="sourceCode">C# 源代码内容。</param>
    public ProtoClassAnalyzer(string sourceCode)
    {
        _syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
    }

    /// <summary>
    /// 遍历语法树，找出所有 ProtoBuf 类并返回其信息列表。
    /// 采用两次遍历：第一次收集所有 IExtensible 类名，第二次分析成员，
    /// 以便在生成重置语句时识别嵌套的 ProtoBuf 类型。
    /// </summary>
    /// <returns>ProtoClassInfo 列表，每项对应一个 IExtensible 实现类。</returns>
    public List<ProtoClassInfo> FindProtoClasses()
    {
        var root = _syntaxTree.GetRoot();

        // 第一次遍历：收集所有继承自 IExtensible 的类名
        var protoClassNames = root.DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .Where(IsProtoExtensible)
            .Select(c => c.Identifier.Text)
            .ToHashSet();

        // 第二次遍历：分析每个 ProtoBuf 类的成员
        var result = new List<ProtoClassInfo>();

        foreach (var classDecl in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
        {
            if (!IsProtoExtensible(classDecl))
                continue;

            string ns = GetNamespace(classDecl);
            var members = FindProtoMembers(classDecl, protoClassNames);

            result.Add(new ProtoClassInfo
            {
                ClassName = classDecl.Identifier.Text,
                Namespace = ns,
                Members = members,
            });
        }

        return result;
    }

    /// <summary>
    /// 判断类是否在基类列表中包含 global::ProtoBuf.IExtensible。
    /// </summary>
    private static bool IsProtoExtensible(ClassDeclarationSyntax classDecl)
    {
        if (classDecl.BaseList == null)
            return false;

        return classDecl.BaseList.Types.Any(t =>
        {
            string typeName = t.Type.ToString();
            return typeName == "global::ProtoBuf.IExtensible"
                || typeName == "ProtoBuf.IExtensible";
        });
    }

    /// <summary>
    /// 获取类所在的命名空间名称。
    /// </summary>
    private static string GetNamespace(ClassDeclarationSyntax classDecl)
    {
        // 兼容 file-scoped namespace 和 block namespace 两种写法
        var blockNs = classDecl.Ancestors()
            .OfType<NamespaceDeclarationSyntax>()
            .FirstOrDefault();
        if (blockNs != null)
            return blockNs.Name.ToString();

        var fileNs = classDecl.Ancestors()
            .OfType<FileScopedNamespaceDeclarationSyntax>()
            .FirstOrDefault();
        return fileNs?.Name.ToString() ?? "";
    }

    /// <summary>
    /// 遍历类成员，找出所有带有 ProtoMember 属性的属性声明，并生成对应的 ProtoMemberInfo。
    /// </summary>
    /// <param name="classDecl">类声明语法节点。</param>
    /// <param name="protoClassNames">已知的所有 ProtoBuf 类名集合，用于识别嵌套类型。</param>
    private static List<ProtoMemberInfo> FindProtoMembers(
        ClassDeclarationSyntax classDecl,
        IReadOnlySet<string> protoClassNames)
    {
        var result = new List<ProtoMemberInfo>();

        foreach (var prop in classDecl.Members.OfType<PropertyDeclarationSyntax>())
        {
            bool hasProtoMember = prop.AttributeLists
                .SelectMany(al => al.Attributes)
                .Any(a => a.Name.ToString().Contains("ProtoMember"));

            if (!hasProtoMember)
                continue;

            string typeName = prop.Type.ToString();
            string resetStmt = TypeResetResolver.GetResetStatement(prop.Identifier.Text, typeName, protoClassNames);

            result.Add(new ProtoMemberInfo
            {
                Name = prop.Identifier.Text,
                TypeName = typeName,
                ResetStatement = resetStmt,
            });
        }

        return result;
    }
}
