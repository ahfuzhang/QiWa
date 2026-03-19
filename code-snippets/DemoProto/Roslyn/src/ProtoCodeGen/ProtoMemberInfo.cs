namespace GenCode.ProtoCodeGen;

/// <summary>
/// 记录带有 ProtoMember 属性的单个成员信息。
/// </summary>
public class ProtoMemberInfo
{
    /// <summary>属性名称，例如 ChildId、StringValue。</summary>
    public string Name { get; init; } = "";

    /// <summary>属性类型的字符串表示，例如 int、string、global::System.Collections.Generic.List&lt;string&gt;。</summary>
    public string TypeName { get; init; } = "";

    /// <summary>生成的重置语句，例如 this.ChildId = 0; 或 this.Tags.Clear();。</summary>
    public string ResetStatement { get; init; } = "";
}
