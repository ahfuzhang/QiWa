using System.Text.Json;
using System.Text.Json.Serialization;
using Demo.Protos;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DemoProto;

internal static class Program
{
    private const string OutputDir = "../../build/code-snippets/DemoProto";

    private static readonly JsonSerializerOptions JsonWriteOptions = new() { WriteIndented = true };

    private static readonly JsonSerializerOptions JsonReadOptions = new()
    {
        // 先调用 getter 拿到已有对象，再向里面填充数据，不需要 setter
        PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate,
        // YAML key 是 camelCase，属性名是 PascalCase，忽略大小写
        PropertyNameCaseInsensitive = true,
        // YamlDotNet 无类型反序列化时所有标量都是字符串，需允许从字符串读取数字
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters =
        {
            // 枚举按名称字符串处理（YAML 中 status: StatusActive）
            new JsonStringEnumConverter(),
            // bool 标量在 YAML 无类型反序列化后也是字符串（"true"/"false"）
            new StringBoolConverter(),
        },
    };

    // YamlDotNet 默认把 byte[] 序列化为整数序列，
    // 但 System.Text.Json 期望 Base64 字符串，用此转换器统一格式。
    private static readonly IYamlTypeConverter ByteArrayConverter = new Base64ByteArrayConverter();

    public static void Main()
    {
        Directory.CreateDirectory(OutputDir);

        var msg = BuildMessage();
        Serialize(msg);
        Deserialize();
    }

    private static AllTypesMessage BuildMessage()
    {
        var msg = new AllTypesMessage
        {
            DoubleValue = 1.1,
            FloatValue = 2.2f,
            Int32Value = 3,
            Int64Value = 4L,
            Uint32Value = 5u,
            Uint64Value = 6ul,
            Sint32Value = -7,
            Sint64Value = -8L,
            Fixed32Value = 9u,
            Fixed64Value = 10ul,
            Sfixed32Value = -11,
            Sfixed64Value = -12L,
            BoolValue = true,
            StringValue = "hello proto",
            BytesValue = new byte[] { 0x01, 0x02, 0x03, 0x04 },
            Status = Status.StatusActive,
            Child = new ChildMessage { ChildId = 42, ChildName = "child" },
            Numbers = new int[] { 1, 2, 3, 4, 5 },
        };
        msg.Tags.AddRange(new[] { "tag1", "tag2", "tag3" });
        msg.Scores["alice"] = 100;
        msg.Scores["bob"] = 200;
        return msg;
    }

    private static void Serialize(AllTypesMessage msg)
    {
        // protobuf binary
        var binPath = Path.Combine(OutputDir, "demo.bin");
        using (var fs = File.Create(binPath))
        {
            ProtoBuf.Serializer.Serialize(fs, msg);
        }
        Console.WriteLine($"[protobuf] saved to {binPath}");

        // json
        var jsonPath = Path.Combine(OutputDir, "demo.json");
        File.WriteAllText(jsonPath, JsonSerializer.Serialize(msg, JsonWriteOptions));
        Console.WriteLine($"[json]     saved to {jsonPath}");

        // yaml — byte[] 序列化为 Base64 字符串，便于反序列化时走 JSON 管道
        var yamlPath = Path.Combine(OutputDir, "demo.yaml");
        var serializer = new SerializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .WithTypeConverter(ByteArrayConverter)
            .Build();
        File.WriteAllText(yamlPath, serializer.Serialize(msg));
        Console.WriteLine($"[yaml]     saved to {yamlPath}");
    }

    private static void Deserialize()
    {
        // protobuf binary
        var binPath = Path.Combine(OutputDir, "demo.bin");
        AllTypesMessage fromBin;
        using (var fs = File.OpenRead(binPath))
        {
            fromBin = ProtoBuf.Serializer.Deserialize<AllTypesMessage>(fs);
        }
        Console.WriteLine("[protobuf] deserialized OK");

        // json
        var jsonPath = Path.Combine(OutputDir, "demo.json");
        var fromJson = JsonSerializer.Deserialize<AllTypesMessage>(File.ReadAllText(jsonPath), JsonReadOptions)!;
        Console.WriteLine("[json]     deserialized OK");

        // yaml — 先用 YamlDotNet 解析为无类型字典，再序列化为 JSON 字符串，
        //         最后用 System.Text.Json (Populate 模式) 反序列化为目标类型。
        //         这样完全复用 System.Text.Json 的 Populate 能力，无需定义额外的 DTO 类型。
        var yamlPath = Path.Combine(OutputDir, "demo.yaml");
        var yamlDeserializer = new DeserializerBuilder().Build();
        var untypedObj = yamlDeserializer.Deserialize(new StringReader(File.ReadAllText(yamlPath)));
        var jsonFromYaml = JsonSerializer.Serialize(untypedObj);
        var fromYaml = JsonSerializer.Deserialize<AllTypesMessage>(jsonFromYaml, JsonReadOptions)!;
        Console.WriteLine("[yaml]     deserialized OK");

        // compare
        Compare(fromBin, fromJson, fromYaml);
        Console.WriteLine("[compare]  all three results are identical. PASS");
    }

    private static void Compare(AllTypesMessage a, AllTypesMessage b, AllTypesMessage c)
    {
        AssertEqual(a.DoubleValue, b.DoubleValue, c.DoubleValue, nameof(AllTypesMessage.DoubleValue));
        AssertEqual(a.FloatValue, b.FloatValue, c.FloatValue, nameof(AllTypesMessage.FloatValue));
        AssertEqual(a.Int32Value, b.Int32Value, c.Int32Value, nameof(AllTypesMessage.Int32Value));
        AssertEqual(a.Int64Value, b.Int64Value, c.Int64Value, nameof(AllTypesMessage.Int64Value));
        AssertEqual(a.Uint32Value, b.Uint32Value, c.Uint32Value, nameof(AllTypesMessage.Uint32Value));
        AssertEqual(a.Uint64Value, b.Uint64Value, c.Uint64Value, nameof(AllTypesMessage.Uint64Value));
        AssertEqual(a.Sint32Value, b.Sint32Value, c.Sint32Value, nameof(AllTypesMessage.Sint32Value));
        AssertEqual(a.Sint64Value, b.Sint64Value, c.Sint64Value, nameof(AllTypesMessage.Sint64Value));
        AssertEqual(a.Fixed32Value, b.Fixed32Value, c.Fixed32Value, nameof(AllTypesMessage.Fixed32Value));
        AssertEqual(a.Fixed64Value, b.Fixed64Value, c.Fixed64Value, nameof(AllTypesMessage.Fixed64Value));
        AssertEqual(a.Sfixed32Value, b.Sfixed32Value, c.Sfixed32Value, nameof(AllTypesMessage.Sfixed32Value));
        AssertEqual(a.Sfixed64Value, b.Sfixed64Value, c.Sfixed64Value, nameof(AllTypesMessage.Sfixed64Value));
        AssertEqual(a.BoolValue, b.BoolValue, c.BoolValue, nameof(AllTypesMessage.BoolValue));
        AssertEqual(a.StringValue, b.StringValue, c.StringValue, nameof(AllTypesMessage.StringValue));
        AssertEqualBytes(a.BytesValue, b.BytesValue, c.BytesValue, nameof(AllTypesMessage.BytesValue));
        AssertEqual((int)a.Status, (int)b.Status, (int)c.Status, nameof(AllTypesMessage.Status));

        AssertEqual(a.Child?.ChildId ?? 0, b.Child?.ChildId ?? 0, c.Child?.ChildId ?? 0, "Child.ChildId");
        AssertEqual(a.Child?.ChildName ?? "", b.Child?.ChildName ?? "", c.Child?.ChildName ?? "", "Child.ChildName");

        AssertEqualSequence(a.Numbers, b.Numbers, c.Numbers, nameof(AllTypesMessage.Numbers));
        AssertEqualSequence(a.Tags.ToArray(), b.Tags.ToArray(), c.Tags.ToArray(), nameof(AllTypesMessage.Tags));

        var aScores = a.Scores.OrderBy(x => x.Key).ToList();
        var bScores = b.Scores.OrderBy(x => x.Key).ToList();
        var cScores = c.Scores.OrderBy(x => x.Key).ToList();
        if (!aScores.SequenceEqual(bScores) || !aScores.SequenceEqual(cScores))
        {
            throw new Exception($"Field Scores mismatch: bin={FormatDict(a.Scores)}, json={FormatDict(b.Scores)}, yaml={FormatDict(c.Scores)}");
        }
    }

    private static void AssertEqual<T>(T a, T b, T c, string field) where T : IEquatable<T>
    {
        if (!a.Equals(b) || !a.Equals(c))
            throw new Exception($"Field {field} mismatch: bin={a}, json={b}, yaml={c}");
    }

    private static void AssertEqualBytes(byte[]? a, byte[]? b, byte[]? c, string field)
    {
        var sa = a ?? [];
        var sb = b ?? [];
        var sc = c ?? [];
        if (!sa.SequenceEqual(sb) || !sa.SequenceEqual(sc))
            throw new Exception($"Field {field} mismatch");
    }

    private static void AssertEqualSequence<T>(T[]? a, T[]? b, T[]? c, string field) where T : IEquatable<T>
    {
        var sa = a ?? [];
        var sb = b ?? [];
        var sc = c ?? [];
        if (!sa.SequenceEqual(sb) || !sa.SequenceEqual(sc))
            throw new Exception($"Field {field} mismatch: bin=[{string.Join(",", sa)}], json=[{string.Join(",", sb)}], yaml=[{string.Join(",", sc)}]");
    }

    private static string FormatDict(Dictionary<string, int> d) =>
        "{" + string.Join(", ", d.OrderBy(x => x.Key).Select(x => $"{x.Key}:{x.Value}")) + "}";
}

/// <summary>
/// YamlDotNet 无类型反序列化时 bool 标量是字符串（"true"/"false"），需要自定义转换。
/// </summary>
internal sealed class StringBoolConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.True) return true;
        if (reader.TokenType == JsonTokenType.False) return false;
        if (reader.TokenType == JsonTokenType.String && bool.TryParse(reader.GetString(), out var result))
            return result;
        throw new JsonException($"Cannot convert to bool: token={reader.TokenType}");
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        => writer.WriteBooleanValue(value);
}

/// <summary>
/// YamlDotNet 的 byte[] 类型转换器：序列化为 Base64 字符串。
/// 默认行为是整数序列，与 System.Text.Json 不兼容，需要统一格式。
/// </summary>
internal sealed class Base64ByteArrayConverter : IYamlTypeConverter
{
    public bool Accepts(Type type) => type == typeof(byte[]);

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        => Convert.FromBase64String(parser.Consume<Scalar>().Value);

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
        => emitter.Emit(new Scalar(Convert.ToBase64String((byte[])value!)));
}
