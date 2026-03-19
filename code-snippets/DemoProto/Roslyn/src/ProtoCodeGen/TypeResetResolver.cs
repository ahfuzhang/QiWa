namespace GenCode.ProtoCodeGen;

/// <summary>
/// 根据 C# 属性的类型名称，生成对应的重置语句。
/// 规则：整数→0，浮点数→0.0，bool→false，string→""，数组→null，
/// Dictionary/List→.Clear()，其他类型→default。
/// </summary>
public static class TypeResetResolver
{
    /// <summary>整数基元类型集合。</summary>
    private static readonly HashSet<string> IntegerTypes = new()
    {
        "int", "long", "uint", "ulong", "short", "ushort", "byte", "sbyte"
    };

    /// <summary>浮点基元类型集合。</summary>
    private static readonly HashSet<string> FloatTypes = new()
    {
        "float", "double", "decimal"
    };

    /// <summary>
    /// 根据属性名和类型名，返回对应的重置语句（不含前导缩进）。
    /// </summary>
    /// <param name="propertyName">属性名称。</param>
    /// <param name="typeName">属性类型的完整字符串。</param>
    /// <param name="protoClassNames">已知的 ProtoBuf 类名集合，用于识别嵌套的 ProtoBuf 对象。</param>
    /// <returns>重置语句，例如 this.Code = 0; 或 if (this.Child != null) this.Child.Reset();</returns>
    public static string GetResetStatement(string propertyName, string typeName, IReadOnlySet<string> protoClassNames)
    {
        string simplified = SimplifyTypeName(typeName);

        if (IntegerTypes.Contains(simplified))
            return $"this.{propertyName} = 0;";

        if (FloatTypes.Contains(simplified))
            return $"this.{propertyName} = 0.0;";

        if (simplified == "bool")
            return $"this.{propertyName} = false;";

        if (simplified == "string")
            return $"this.{propertyName} = \"\";";

        // 数组类型（如 int[]、byte[]）
        if (typeName.TrimEnd().EndsWith("[]"))
            return $"this.{propertyName} = null;";

        // Dictionary 类型使用 Clear()
        if (IsDictionaryType(typeName))
            return $"this.{propertyName}.Clear();";

        // List 类型使用 Clear()
        if (IsListType(typeName))
            return $"this.{propertyName}.Clear();";

        // 另一个 ProtoBuf 类：null 检查后调用其 Reset() 方法
        if (protoClassNames.Contains(simplified))
            return $"if (this.{propertyName} != null) this.{propertyName}.Reset();";

        // 其他类型（枚举、自定义类等）使用 default，兼容值类型和引用类型
        return $"this.{propertyName} = default;";
    }

    /// <summary>
    /// 去掉 global:: 前缀和命名空间部分，提取最简类型名。
    /// 例如 global::System.Collections.Generic.List&lt;string&gt; → List&lt;string&gt;
    /// </summary>
    private static string SimplifyTypeName(string typeName)
    {
        string t = typeName;
        if (t.StartsWith("global::"))
            t = t["global::".Length..];

        // 只取泛型左尖括号之前的最后一段命名空间
        int genericBracket = t.IndexOf('<');
        string prefix = genericBracket >= 0 ? t[..genericBracket] : t;

        int lastDot = prefix.LastIndexOf('.');
        if (lastDot >= 0)
        {
            string simple = prefix[(lastDot + 1)..];
            return genericBracket >= 0 ? simple + t[genericBracket..] : simple;
        }

        return t;
    }

    /// <summary>判断类型名是否为 Dictionary 类型。</summary>
    private static bool IsDictionaryType(string typeName) =>
        typeName.Contains("Dictionary<") || typeName.Contains("IDictionary<");

    /// <summary>判断类型名是否为 List 类型。</summary>
    private static bool IsListType(string typeName) =>
        typeName.Contains("List<") || typeName.Contains("IList<");
}
