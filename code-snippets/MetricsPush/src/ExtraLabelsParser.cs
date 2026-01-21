using System.Collections.Generic;

namespace MetricsPush;

internal static class ExtraLabelsParser
{
    public static Dictionary<string, string> Parse(string? raw)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        if (string.IsNullOrWhiteSpace(raw))
        {
            return result;
        }

        ReadOnlySpan<char> span = raw.AsSpan();
        while (!span.IsEmpty)
        {
            int ampIndex = span.IndexOf('&');
            ReadOnlySpan<char> token = ampIndex >= 0 ? span[..ampIndex] : span;
            if (!token.IsEmpty)
            {
                int eqIndex = token.IndexOf('=');
                ReadOnlySpan<char> key = eqIndex >= 0 ? token[..eqIndex] : token;
                ReadOnlySpan<char> value = eqIndex >= 0 && eqIndex + 1 < token.Length
                    ? token[(eqIndex + 1)..]
                    : ReadOnlySpan<char>.Empty;
                if (!key.IsEmpty)
                {
                    result[key.ToString()] = value.ToString();
                }
            }

            span = ampIndex >= 0 ? span[(ampIndex + 1)..] : ReadOnlySpan<char>.Empty;
        }

        return result;
    }
}
