using System.Buffers;
using System.Text;

namespace MetricsPush;

internal static class Utf8LabelWriter
{
    private static readonly byte[] EscapedBackslash = new byte[] { (byte)'\\', (byte)'\\' };
    private static readonly byte[] EscapedQuote = new byte[] { (byte)'\\', (byte)'"' };
    private static readonly byte[] EscapedNewline = new byte[] { (byte)'\\', (byte)'n' };
    private static readonly byte[] EscapedCarriageReturn = new byte[] { (byte)'\\', (byte)'r' };
    private static readonly byte[] EscapedTab = new byte[] { (byte)'\\', (byte)'t' };

    public static void AppendEscaped(ArrayBufferWriter<byte> writer, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        byte[] utf8 = Encoding.UTF8.GetBytes(value);
        foreach (byte b in utf8)
        {
            switch (b)
            {
                case (byte)'\\':
                    Utf8BufferWriter.AppendBytes(writer, EscapedBackslash);
                    break;
                case (byte)'"':
                    Utf8BufferWriter.AppendBytes(writer, EscapedQuote);
                    break;
                case (byte)'\n':
                    Utf8BufferWriter.AppendBytes(writer, EscapedNewline);
                    break;
                case (byte)'\r':
                    Utf8BufferWriter.AppendBytes(writer, EscapedCarriageReturn);
                    break;
                case (byte)'\t':
                    Utf8BufferWriter.AppendBytes(writer, EscapedTab);
                    break;
                default:
                    Utf8BufferWriter.AppendByte(writer, b);
                    break;
            }
        }
    }
}
