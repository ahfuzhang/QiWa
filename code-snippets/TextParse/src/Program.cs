using System;
using System.Buffers;
using System.CommandLine;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Security.AccessControl;
using System.Text;
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

    public static byte[] parseTextUtf8(Span<byte> src, Dictionary<string, string> tags) {
        /*
        to dear AI's prompt:
        1 tags 是 promethues 中的 metric 的 tag
        2 src 是来自某个 promethues exporter 的文本
        3 不要把 src 转换为 string，直接基于字节流来处理
        4 搜索 src 中的 \n，逐行处理字节流
        5 每行中，如果以 # 开头，丢弃这行
        6 如果匹配到 metric_name value 这样的行，变成 metric_name{tags1="value1", tags2="value2"}
        7 如果匹配到 metric_name{sometag="value"} 这样的格式，把 tags 插入到前部: metric_name{tags1="value1", tags2="value2", sometag="value"}
        8 把处理后的内容转换为 byte[] 数组返回
        */
        if (src.Length == 0) {
            return Array.Empty<byte>();
        }

        byte[] tagPrefixBytes = BuildTagPrefixBytes(tags);
        bool hasTags = tagPrefixBytes.Length > 0;

        var output = new ArrayBufferWriter<byte>(src.Length);
        bool wroteLine = false;
        int lineStart = 0;

        for (int i = 0; i <= src.Length; i++) {
            bool atEnd = i == src.Length;
            if (!atEnd && src[i] != (byte)'\n') {
                continue;
            }

            int lineLength = i - lineStart;
            ReadOnlySpan<byte> line = src.Slice(lineStart, lineLength);
            if (line.Length > 0 && line[^1] == (byte)'\r') {
                line = line[..^1];
            }

            if (line.Length == 0) {
                if (wroteLine) {
                    AppendByte(output, (byte)'\n');
                }
                wroteLine = true;
                lineStart = i + 1;
                continue;
            }

            int firstNonWhitespace = IndexOfNonWhitespace(line);
            if (firstNonWhitespace >= 0 && line[firstNonWhitespace] == (byte)'#') {
                lineStart = i + 1;
                continue;
            }

            ReadOnlySpan<byte> leading = ReadOnlySpan<byte>.Empty;
            ReadOnlySpan<byte> metricPart = ReadOnlySpan<byte>.Empty;
            ReadOnlySpan<byte> rest = ReadOnlySpan<byte>.Empty;
            bool modified = false;

            if (hasTags && firstNonWhitespace >= 0) {
                int splitIndex = IndexOfWhitespaceAfter(line, firstNonWhitespace);
                if (splitIndex > firstNonWhitespace) {
                    leading = line[..firstNonWhitespace];
                    metricPart = line.Slice(firstNonWhitespace, splitIndex - firstNonWhitespace);
                    rest = line.Slice(splitIndex);
                    modified = true;
                }
            }

            if (wroteLine) {
                AppendByte(output, (byte)'\n');
            }
            wroteLine = true;

            if (modified) {
                AppendBytes(output, leading);
                AppendMetricWithTags(output, metricPart, tagPrefixBytes);
                AppendBytes(output, rest);
            }
            else {
                AppendBytes(output, line);
            }

            lineStart = i + 1;
        }

        return output.WrittenSpan.ToArray();
    }

    public static byte[] parseText(Span<byte> src, Dictionary<string, string> tags) {
        /*
        to dear AI's prompt:
        1 把 src 转换为 string
        2 遍历每行
        3 字符串如果以 # 开头，丢弃这行
        4 如果匹配到 metric_name value 这样的行，变成 metric_name{tags1="value1", tags2="value2"}
        5 如果匹配到 metric_name{sometag="value"} 这样的格式，把 tags 插入到前部: metric_name{tags1="value1", tags2="value2", sometag="value"}
        6 所有转换后的内容，转换为 utf-8 格式，返回 byte[] 数组
        */
        if (src.Length == 0) {
            return Array.Empty<byte>();
        }

        string input = Encoding.UTF8.GetString(src);
        string tagPrefix = BuildTagPrefix(tags);
        bool hasTags = tagPrefix.Length > 0;

        var sb = new StringBuilder(input.Length);
        using var reader = new StringReader(input);
        string? line;
        bool wroteLine = false;
        while ((line = reader.ReadLine()) != null) {
            if (line.Length == 0) {
                if (wroteLine) {
                    sb.Append('\n');
                }
                wroteLine = true;
                continue;
            }

            int firstNonWhitespace = IndexOfNonWhitespace(line);
            if (firstNonWhitespace >= 0 && line[firstNonWhitespace] == '#') {
                continue;
            }

            string outputLine = line;
            if (hasTags) {
                int splitIndex = IndexOfWhitespaceAfter(line, firstNonWhitespace);
                if (splitIndex > firstNonWhitespace) {
                    string leading = line.Substring(0, firstNonWhitespace);
                    string metricPart = line.Substring(firstNonWhitespace, splitIndex - firstNonWhitespace);
                    string rest = line.Substring(splitIndex);
                    outputLine = leading + ApplyTags(metricPart, tagPrefix) + rest;
                }
            }

            if (wroteLine) {
                sb.Append('\n');
            }
            sb.Append(outputLine);
            wroteLine = true;
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static string BuildTagPrefix(Dictionary<string, string> tags) {
        if (tags == null || tags.Count == 0) {
            return string.Empty;
        }

        var sb = new StringBuilder();
        bool first = true;
        foreach (var kvp in tags.OrderBy(kv => kv.Key, StringComparer.Ordinal)) {
            if (!first) {
                sb.Append(",");
            }
            sb.Append(kvp.Key);
            sb.Append("=\"");
            sb.Append(EscapeLabelValue(kvp.Value));
            sb.Append('"');
            first = false;
        }
        return sb.ToString();
    }

    private static string EscapeLabelValue(string value) {
        if (string.IsNullOrEmpty(value)) {
            return string.Empty;
        }
        return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    private static byte[] BuildTagPrefixBytes(Dictionary<string, string> tags) {
        if (tags == null || tags.Count == 0) {
            return Array.Empty<byte>();
        }

        var output = new ArrayBufferWriter<byte>();
        bool first = true;
        foreach (var kvp in tags.OrderBy(kv => kv.Key, StringComparer.Ordinal)) {
            if (!first) {
                AppendByte(output, (byte)',');
            }
            AppendBytes(output, Encoding.UTF8.GetBytes(kvp.Key));
            AppendByte(output, (byte)'=');
            AppendByte(output, (byte)'"');
            AppendEscapedUtf8(output, kvp.Value);
            AppendByte(output, (byte)'"');
            first = false;
        }

        return output.WrittenSpan.ToArray();
    }

    private static void AppendEscapedUtf8(ArrayBufferWriter<byte> output, string value) {
        if (string.IsNullOrEmpty(value)) {
            return;
        }

        byte[] bytes = Encoding.UTF8.GetBytes(value);
        for (int i = 0; i < bytes.Length; i++) {
            byte b = bytes[i];
            if (b == (byte)'\\' || b == (byte)'"') {
                AppendByte(output, (byte)'\\');
            }
            AppendByte(output, b);
        }
    }

    private static string ApplyTags(string metricPart, string tagPrefix) {
        int openIndex = metricPart.IndexOf('{');
        if (openIndex < 0) {
            return $"{metricPart}{{{tagPrefix}}}";
        }

        int closeIndex = metricPart.LastIndexOf('}');
        if (closeIndex <= openIndex) {
            return metricPart;
        }

        string metricName = metricPart.Substring(0, openIndex);
        string existing = metricPart.Substring(openIndex + 1, closeIndex - openIndex - 1).Trim();
        string merged = string.IsNullOrEmpty(existing) ? tagPrefix : $"{tagPrefix}, {existing}";
        string suffix = metricPart.Substring(closeIndex + 1);
        return $"{metricName}{{{merged}}}{suffix}";
    }

    private static int IndexOfNonWhitespace(string value) {
        for (int i = 0; i < value.Length; i++) {
            if (!char.IsWhiteSpace(value[i])) {
                return i;
            }
        }
        return -1;
    }

    private static int IndexOfWhitespaceAfter(string value, int startIndex) {
        if (startIndex < 0) {
            return -1;
        }
        for (int i = startIndex; i < value.Length; i++) {
            if (char.IsWhiteSpace(value[i])) {
                return i;
            }
        }
        return -1;
    }

    private static void AppendMetricWithTags(ArrayBufferWriter<byte> output, ReadOnlySpan<byte> metricPart, ReadOnlySpan<byte> tagPrefix) {
        int openIndex = metricPart.IndexOf((byte)'{');
        if (openIndex < 0) {
            AppendBytes(output, metricPart);
            AppendByte(output, (byte)'{');
            AppendBytes(output, tagPrefix);
            AppendByte(output, (byte)'}');
            return;
        }

        int closeIndex = metricPart.LastIndexOf((byte)'}');
        if (closeIndex <= openIndex) {
            AppendBytes(output, metricPart);
            return;
        }

        ReadOnlySpan<byte> metricName = metricPart[..openIndex];
        ReadOnlySpan<byte> existing = metricPart.Slice(openIndex + 1, closeIndex - openIndex - 1);
        existing = TrimAsciiWhitespace(existing);
        ReadOnlySpan<byte> suffix = metricPart.Slice(closeIndex + 1);

        AppendBytes(output, metricName);
        AppendByte(output, (byte)'{');
        AppendBytes(output, tagPrefix);
        if (!existing.IsEmpty) {
            AppendByte(output, (byte)',');
            AppendByte(output, (byte)' ');
            AppendBytes(output, existing);
        }
        AppendByte(output, (byte)'}');
        AppendBytes(output, suffix);
    }

    private static int IndexOfNonWhitespace(ReadOnlySpan<byte> value) {
        for (int i = 0; i < value.Length; i++) {
            if (!IsWhitespace(value[i])) {
                return i;
            }
        }
        return -1;
    }

    private static int IndexOfWhitespaceAfter(ReadOnlySpan<byte> value, int startIndex) {
        if (startIndex < 0) {
            return -1;
        }
        for (int i = startIndex; i < value.Length; i++) {
            if (IsWhitespace(value[i])) {
                return i;
            }
        }
        return -1;
    }

    private static ReadOnlySpan<byte> TrimAsciiWhitespace(ReadOnlySpan<byte> value) {
        int start = 0;
        int end = value.Length - 1;
        while (start <= end && IsWhitespace(value[start])) {
            start++;
        }
        while (end >= start && IsWhitespace(value[end])) {
            end--;
        }
        if (start > end) {
            return ReadOnlySpan<byte>.Empty;
        }
        return value.Slice(start, end - start + 1);
    }

    private static bool IsWhitespace(byte value) {
        return value == (byte)' ' || value == (byte)'\t';
    }

    private static void AppendBytes(ArrayBufferWriter<byte> output, ReadOnlySpan<byte> value) {
        if (value.IsEmpty) {
            return;
        }
        Span<byte> buffer = output.GetSpan(value.Length);
        value.CopyTo(buffer);
        output.Advance(value.Length);
    }

    private static void AppendByte(ArrayBufferWriter<byte> output, byte value) {
        Span<byte> buffer = output.GetSpan(1);
        buffer[0] = value;
        output.Advance(1);
    }

    public static byte[] parseTextUtf8ByNeon(Span<byte> src, Dictionary<string, string> tags) {
        /*
          to dear AI's prompt:
          * 目标：生成一个解析 prometheus 格式的文本的函数，去掉无用的注释行，为每行添加公共 tag
          * 参数：
            - src: prometheus 格式的文本，utf-8 编码
            - tags: 以 tag name -> tag value 格式保存公共 tag
          * 返回：
            - 处理后的文本的 buffer，使用 utf-8 编码
          * 约束：
            - 不要把 utf-8 转换为 utf-16，基于字节流来处理
            - 使用 arm64 的 simd 指令集来优化
          * 步骤：
            - 1 从 src 中，以非对齐的方式，加载 16 字节，也就是 128bit 到 Vector<byte> 中
            - 2 构造一个由字符 \n 组成的 vector
            - 3 比较两个 vector，如果全部为 0，则继续往后读 16 字节，回到第一步。
            - 4 如果存在非 0， 说明找到了 \n 字符
            - 5 在当前行中，如果以 # 或者空白字符开头，则丢弃这一行
            - 6 以 simd 的方式搜索字符 '{' 和 '}'，但是又不能是包含在双引号中的这两个字符。
            - 7 如果是不存在 tag 部分的 metric，增加 {public_tag1="value1",public_tag2="value2"} 这样公共 tag
            - 8 如果存在 tag 部分，则在 '{' 字符之后插入公共 tag
            - 9 处理完所有行后，返回 byte[] 数组      
        */
        if (src.Length == 0) {
            return Array.Empty<byte>();
        }

        byte[] tagPrefixBytes = BuildTagPrefixBytes(tags);
        bool hasTags = tagPrefixBytes.Length > 0;

        var output = new ArrayBufferWriter<byte>(src.Length);
        bool wroteLine = false;
        int lineStart = 0;
        int length = src.Length;

        while (lineStart <= length) {
            int newlineIndex = FindNextNewlineNeon(src, lineStart);
            if (newlineIndex < 0) {
                newlineIndex = length;
            }

            ReadOnlySpan<byte> line = src.Slice(lineStart, newlineIndex - lineStart);
            if (line.Length > 0 && line[^1] == (byte)'\r') {
                line = line[..^1];
            }

            if (line.Length == 0) {
                if (wroteLine) {
                    AppendByte(output, (byte)'\n');
                }
                wroteLine = true;
                if (newlineIndex == length) {
                    break;
                }
                lineStart = newlineIndex + 1;
                continue;
            }

            if (line[0] == (byte)'#' || IsWhitespace(line[0])) {
                if (newlineIndex == length) {
                    break;
                }
                lineStart = newlineIndex + 1;
                continue;
            }

            ReadOnlySpan<byte> metricPart = ReadOnlySpan<byte>.Empty;
            ReadOnlySpan<byte> rest = ReadOnlySpan<byte>.Empty;
            bool modified = false;

            if (hasTags) {
                int splitIndex = IndexOfWhitespaceAfter(line, 0);
                if (splitIndex > 0) {
                    metricPart = line[..splitIndex];
                    rest = line[splitIndex..];
                    modified = true;
                }
            }

            if (wroteLine) {
                AppendByte(output, (byte)'\n');
            }
            wroteLine = true;

            if (modified) {
                AppendMetricWithTagsNeon(output, metricPart, tagPrefixBytes);
                AppendBytes(output, rest);
            }
            else {
                AppendBytes(output, line);
            }

            if (newlineIndex == length) {
                break;
            }
            lineStart = newlineIndex + 1;
        }

        return output.WrittenSpan.ToArray();
    }

    private static int FindNextNewlineNeon(ReadOnlySpan<byte> src, int startIndex) {
        int length = src.Length;
        int i = startIndex;

        if (AdvSimd.Arm64.IsSupported) {
            int remaining = length - i;
            int simdBytes = remaining - (remaining & (Vector128<byte>.Count - 1));
            if (simdBytes >= Vector128<byte>.Count) {
                int last = i + simdBytes - Vector128<byte>.Count;
                Vector128<byte> needle = Vector128.Create((byte)'\n');
                ref byte start = ref MemoryMarshal.GetReference(src);
                for (; i <= last; i += Vector128<byte>.Count) {
                    Vector128<byte> chunk = Unsafe.ReadUnaligned<Vector128<byte>>(ref Unsafe.Add(ref start, i));
                    Vector128<byte> cmp = AdvSimd.CompareEqual(chunk, needle);
                    if (AdvSimd.Arm64.MaxAcross(cmp).ToScalar() != 0) {
                        for (int j = 0; j < Vector128<byte>.Count; j++) {
                            if (src[i + j] == (byte)'\n') {
                                return i + j;
                            }
                        }
                    }
                }
            }
        }

        for (; i < length; i++) {
            if (src[i] == (byte)'\n') {
                return i;
            }
        }
        return -1;
    }

    private static void AppendMetricWithTagsNeon(ArrayBufferWriter<byte> output, ReadOnlySpan<byte> metricPart, ReadOnlySpan<byte> tagPrefix) {
        if (!TryFindLabelBracesNeon(metricPart, out int openIndex, out int closeIndex)) {
            AppendBytes(output, metricPart);
            AppendByte(output, (byte)'{');
            AppendBytes(output, tagPrefix);
            AppendByte(output, (byte)'}');
            return;
        }

        if (closeIndex <= openIndex) {
            AppendBytes(output, metricPart);
            return;
        }

        ReadOnlySpan<byte> metricName = metricPart[..openIndex];
        ReadOnlySpan<byte> existing = metricPart.Slice(openIndex + 1, closeIndex - openIndex - 1);
        existing = TrimAsciiWhitespace(existing);
        ReadOnlySpan<byte> suffix = metricPart.Slice(closeIndex + 1);

        AppendBytes(output, metricName);
        AppendByte(output, (byte)'{');
        AppendBytes(output, tagPrefix);
        if (!existing.IsEmpty) {
            AppendByte(output, (byte)',');
            AppendByte(output, (byte)' ');
            AppendBytes(output, existing);
        }
        AppendByte(output, (byte)'}');
        AppendBytes(output, suffix);
    }

    private static bool TryFindLabelBracesNeon(ReadOnlySpan<byte> metricPart, out int openIndex, out int closeIndex) {
        openIndex = -1;
        closeIndex = -1;
        bool inQuotes = false;
        int backslashRun = 0;
        int i = 0;
        int length = metricPart.Length;

        if (AdvSimd.Arm64.IsSupported && length >= Vector128<byte>.Count) {
            Vector128<byte> openNeedle = Vector128.Create((byte)'{');
            Vector128<byte> closeNeedle = Vector128.Create((byte)'}');
            Vector128<byte> quoteNeedle = Vector128.Create((byte)'"');
            Vector128<byte> slashNeedle = Vector128.Create((byte)'\\');
            int simdBytes = length - (length & (Vector128<byte>.Count - 1));
            int last = simdBytes - Vector128<byte>.Count;
            ref byte start = ref MemoryMarshal.GetReference(metricPart);

            while (i <= last) {
                Vector128<byte> chunk = Unsafe.ReadUnaligned<Vector128<byte>>(ref Unsafe.Add(ref start, i));
                Vector128<byte> cmpOpen = AdvSimd.CompareEqual(chunk, openNeedle);
                Vector128<byte> cmpClose = AdvSimd.CompareEqual(chunk, closeNeedle);
                Vector128<byte> cmpQuote = AdvSimd.CompareEqual(chunk, quoteNeedle);
                Vector128<byte> cmpSlash = AdvSimd.CompareEqual(chunk, slashNeedle);
                Vector128<byte> any = AdvSimd.Or(AdvSimd.Or(cmpOpen, cmpClose), AdvSimd.Or(cmpQuote, cmpSlash));
                if (AdvSimd.Arm64.MaxAcross(any).ToScalar() == 0) {
                    backslashRun = 0;
                    i += Vector128<byte>.Count;
                    continue;
                }
                break;
            }
        }

        for (; i < length; i++) {
            byte b = metricPart[i];
            if (b == (byte)'\\') {
                backslashRun++;
                continue;
            }

            bool escaped = (backslashRun & 1) != 0;
            backslashRun = 0;

            if (b == (byte)'"' && !escaped) {
                inQuotes = !inQuotes;
                continue;
            }

            if (!inQuotes) {
                if (b == (byte)'{' && openIndex < 0) {
                    openIndex = i;
                }
                else if (b == (byte)'}') {
                    closeIndex = i;
                }
            }
        }

        return openIndex >= 0;
    }
}
