namespace GenCode.ProtoCodeGen;

/// <summary>
/// 记录继承自 global::ProtoBuf.IExtensible 的 ProtoBuf 类信息。
/// </summary>
public class ProtoClassInfo
{
    /// <summary>类名，例如 ChildMessage、AllTypesMessage。</summary>
    public string ClassName { get; init; } = "";

    /// <summary>类所在的命名空间，例如 Demo.Protos。</summary>
    public string Namespace { get; init; } = "";

    /// <summary>类中带有 ProtoMember 属性的成员列表。</summary>
    public List<ProtoMemberInfo> Members { get; init; } = new();
}
