using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Globalization;
using OpenTelemetry.Metrics;

namespace MetricsPush;

internal static class MetricTextFormatter {
    public static void Format(IBufferWriter<byte> writer, IReadOnlyList<Metric> metrics, IReadOnlyDictionary<string, string> extraLabels) {
        foreach (var metric in metrics) {
            string metricName = SanitizeMetricName(metric.Name);
            if (string.IsNullOrEmpty(metricName)) {
                continue;
            }

            switch (metric.MetricType) {
                case MetricType.Histogram:
                    foreach (var point in metric.GetMetricPoints()) {
                        AppendHistogramLines(writer, metricName, extraLabels, point);
                    }
                    break;
                default:
                    foreach (var point in metric.GetMetricPoints()) {
                        if (!TryGetValue(metric, point, out bool isDouble, out long longValue, out double doubleValue)) {
                            continue;
                        }

                        AppendMetricLine(writer, metricName, extraLabels, point, isDouble, longValue, doubleValue);
                    }
                    break;
            }
        }
    }

    private static void AppendMetricLine(
        IBufferWriter<byte> writer,
        string name,
        IReadOnlyDictionary<string, string> extraLabels,
        in MetricPoint point,
        bool isDouble,
        long longValue,
        double doubleValue) {
        Utf8BufferWriter.AppendString(writer, name);

        AppendLabels(writer, extraLabels, point, null, null);

        Utf8BufferWriter.AppendByte(writer, (byte)' ');
        if (isDouble) {
            AppendDouble(writer, doubleValue);
        }
        else {
            AppendLong(writer, longValue);
        }
        Utf8BufferWriter.AppendByte(writer, (byte)'\n');
    }

    private static void AppendLabel(IBufferWriter<byte> writer, string key, string value, ref bool wroteAny) {
        if (string.IsNullOrEmpty(key)) {
            return;
        }

        if (wroteAny) {
            Utf8BufferWriter.AppendByte(writer, (byte)',');
        }

        Utf8BufferWriter.AppendString(writer, key);
        Utf8BufferWriter.AppendByte(writer, (byte)'=');
        Utf8BufferWriter.AppendByte(writer, (byte)'"');
        Utf8LabelWriter.AppendEscaped(writer, value);
        Utf8BufferWriter.AppendByte(writer, (byte)'"');
        wroteAny = true;
    }

    private static void AppendLabels(
        IBufferWriter<byte> writer,
        IReadOnlyDictionary<string, string> extraLabels,
        in MetricPoint point,
        string? additionalKey,
        string? additionalValue) {
        bool hasLabels = extraLabels.Count > 0 || point.Tags.Count > 0 || !string.IsNullOrEmpty(additionalKey);
        if (!hasLabels) {
            return;
        }

        Utf8BufferWriter.AppendByte(writer, (byte)'{');
        bool wroteAny = false;

        foreach (var label in extraLabels) {
            string key = SanitizeLabelName(label.Key);
            if (string.IsNullOrEmpty(key)) {
                continue;
            }

            AppendLabel(writer, key, label.Value, ref wroteAny);
        }

        foreach (var tag in point.Tags) {
            if (string.IsNullOrEmpty(tag.Key)) {
                continue;
            }

            string key = SanitizeLabelName(tag.Key);
            if (string.IsNullOrEmpty(key)) {
                continue;
            }

            string value = Convert.ToString(tag.Value, CultureInfo.InvariantCulture) ?? string.Empty;
            AppendLabel(writer, key, value, ref wroteAny);
        }

        if (!string.IsNullOrEmpty(additionalKey)) {
            string key = SanitizeLabelName(additionalKey);
            if (!string.IsNullOrEmpty(key)) {
                AppendLabel(writer, key, additionalValue ?? string.Empty, ref wroteAny);
            }
        }

        Utf8BufferWriter.AppendByte(writer, (byte)'}');
    }

    private static void AppendLong(IBufferWriter<byte> writer, long value) {
        Span<byte> span = writer.GetSpan(32);
        if (!Utf8Formatter.TryFormat(value, span, out int written)) {
            Utf8BufferWriter.AppendString(writer, value.ToString(CultureInfo.InvariantCulture));
            return;
        }
        writer.Advance(written);
    }

    private static void AppendDouble(IBufferWriter<byte> writer, double value) {
        Span<byte> span = writer.GetSpan(64);
        if (!Utf8Formatter.TryFormat(value, span, out int written)) {
            Utf8BufferWriter.AppendString(writer, value.ToString(CultureInfo.InvariantCulture));
            return;
        }
        writer.Advance(written);
    }

    private static void AppendHistogramLines(
        IBufferWriter<byte> writer,
        string name,
        IReadOnlyDictionary<string, string> extraLabels,
        in MetricPoint point) {
        long cumulative = 0;
        foreach (var bucket in point.GetHistogramBuckets()) {
            cumulative += bucket.BucketCount;
            AppendHistogramBucketLine(writer, name, extraLabels, point, bucket.ExplicitBound, cumulative);
        }

        AppendMetricLine(writer, name + "_sum", extraLabels, point, true, default, point.GetHistogramSum());
        AppendMetricLine(writer, name + "_count", extraLabels, point, false, point.GetHistogramCount(), default);
    }

    private static void AppendHistogramBucketLine(
        IBufferWriter<byte> writer,
        string name,
        IReadOnlyDictionary<string, string> extraLabels,
        in MetricPoint point,
        double bound,
        long cumulativeCount) {
        Utf8BufferWriter.AppendString(writer, name);
        Utf8BufferWriter.AppendString(writer, "_bucket");

        string boundValue = double.IsPositiveInfinity(bound)
            ? "+Inf"
            : bound.ToString(CultureInfo.InvariantCulture);
        AppendLabels(writer, extraLabels, point, "le", boundValue);

        Utf8BufferWriter.AppendByte(writer, (byte)' ');
        AppendLong(writer, cumulativeCount);
        Utf8BufferWriter.AppendByte(writer, (byte)'\n');
    }

    private static bool TryGetValue(
        Metric metric,
        in MetricPoint point,
        out bool isDouble,
        out long longValue,
        out double doubleValue) {
        switch (metric.MetricType) {
            case MetricType.LongSum:
            case MetricType.LongSumNonMonotonic:
                longValue = point.GetSumLong();
                doubleValue = default;
                isDouble = false;
                return true;
            case MetricType.DoubleSum:
            case MetricType.DoubleSumNonMonotonic:
                longValue = default;
                doubleValue = point.GetSumDouble();
                isDouble = true;
                return true;
            case MetricType.LongGauge:
                longValue = point.GetGaugeLastValueLong();
                doubleValue = default;
                isDouble = false;
                return true;
            case MetricType.DoubleGauge:
                longValue = default;
                doubleValue = point.GetGaugeLastValueDouble();
                isDouble = true;
                return true;
            default:
                longValue = default;
                doubleValue = default;
                isDouble = false;
                return false;
        }
    }

    private static string SanitizeMetricName(string name) {
        return SanitizeName(name, allowColon: true, allowDot: false);
    }

    private static string SanitizeLabelName(string name) {
        return SanitizeName(name, allowColon: false, allowDot: false);
    }

    private static string SanitizeName(string name, bool allowColon, bool allowDot) {
        if (string.IsNullOrEmpty(name)) {
            return string.Empty;
        }

        bool needsSanitize = !IsValidFirstChar(name[0], allowColon, allowDot);
        if (!needsSanitize) {
            for (int i = 0; i < name.Length; i++) {
                if (!IsValidNameChar(name[i], allowColon, allowDot)) {
                    needsSanitize = true;
                    break;
                }
            }
        }

        if (!needsSanitize) {
            return name;
        }

        Span<char> buffer = name.Length <= 256 ? stackalloc char[name.Length + 1] : new char[name.Length + 1];
        int index = 0;

        for (int i = 0; i < name.Length; i++) {
            char normalized = IsValidNameChar(name[i], allowColon, allowDot) ? name[i] : '_';
            if (index == 0 && !IsValidFirstChar(normalized, allowColon, allowDot)) {
                buffer[index++] = '_';
            }

            buffer[index++] = normalized;
        }

        return new string(buffer[..index]);
    }

    private static bool IsValidFirstChar(char value, bool allowColon, bool allowDot) {
        return value == '_' || IsAsciiLetter(value) || (allowColon && value == ':') || (allowDot && value == '.');
    }

    private static bool IsValidNameChar(char value, bool allowColon, bool allowDot) {
        return IsAsciiLetter(value) || IsAsciiDigit(value) || value == '_' || (allowColon && value == ':')
            || (allowDot && value == '.');
    }

    private static bool IsAsciiLetter(char value) {
        return (value >= 'A' && value <= 'Z') || (value >= 'a' && value <= 'z');
    }

    private static bool IsAsciiDigit(char value) {
        return (value >= '0' && value <= '9');
    }
}
