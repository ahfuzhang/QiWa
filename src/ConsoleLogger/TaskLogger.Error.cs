namespace ConsoleLogger;

using System.Runtime.CompilerServices;

/// <summary>
/// 提供 Error 级别的日志输出能力。这个分部文件用于实现提示词要求的 TaskLogger.Error 重载。
/// </summary>
public partial class TaskLogger
{
    /// <summary>
    /// 输出包含 1 个字段的 Error 级别日志。
    /// </summary>
    public void Error(Field field1,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Error)
        {
            return;
        }
        ThreadLocalLogger.Current.write1(prefix.Bytes(), ref field1, "error", file, member, line);
    }

    /// <summary>
    /// 输出包含 2 个字段的 Error 级别日志。
    /// </summary>
    public void Error(Field field1, Field field2,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Error)
        {
            return;
        }
        ThreadLocalLogger.Current.write2(prefix.Bytes(), ref field1, ref field2, "error", file, member, line);
    }

    /// <summary>
    /// 输出包含 3 个字段的 Error 级别日志。
    /// </summary>
    public void Error(Field field1, Field field2, Field field3,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Error)
        {
            return;
        }
        ThreadLocalLogger.Current.write3(prefix.Bytes(), ref field1, ref field2, ref field3, "error", file, member, line);
    }

    /// <summary>
    /// 输出包含 4 个字段的 Error 级别日志。
    /// </summary>
    public void Error(Field field1, Field field2, Field field3, Field field4,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Error)
        {
            return;
        }
        ThreadLocalLogger.Current.write4(prefix.Bytes(), ref field1, ref field2, ref field3, ref field4, "error", file, member, line);
    }

    /// <summary>
    /// 输出包含 5 个字段的 Error 级别日志。
    /// </summary>
    public void Error(Field field1, Field field2, Field field3, Field field4, Field field5,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Error)
        {
            return;
        }
        ThreadLocalLogger.Current.write5(prefix.Bytes(), ref field1, ref field2, ref field3, ref field4, ref field5, "error", file, member, line);
    }

    /// <summary>
    /// 输出包含 6 个字段的 Error 级别日志。
    /// </summary>
    public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Error)
        {
            return;
        }
        ThreadLocalLogger.Current.write6(prefix.Bytes(), ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, "error", file, member, line);
    }

    /// <summary>
    /// 输出包含 7 个字段的 Error 级别日志。
    /// </summary>
    public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Error)
        {
            return;
        }
        ThreadLocalLogger.Current.write7(prefix.Bytes(), ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, "error", file, member, line);
    }

    /// <summary>
    /// 输出包含 8 个字段的 Error 级别日志。
    /// </summary>
    public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Error)
        {
            return;
        }
        ThreadLocalLogger.Current.write8(prefix.Bytes(), ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, "error", file, member, line);
    }

    /// <summary>
    /// 输出包含 9 个字段的 Error 级别日志。
    /// </summary>
    public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Error)
        {
            return;
        }
        ThreadLocalLogger.Current.write9(prefix.Bytes(), ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, "error", file, member, line);
    }

    /// <summary>
    /// 输出包含 10 个字段的 Error 级别日志。
    /// </summary>
    public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Error)
        {
            return;
        }
        ThreadLocalLogger.Current.write10(prefix.Bytes(), ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, "error", file, member, line);
    }

    /// <summary>
    /// 输出包含 11 个字段的 Error 级别日志。
    /// </summary>
    public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Error)
        {
            return;
        }
        ThreadLocalLogger.Current.write11(prefix.Bytes(), ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, "error", file, member, line);
    }

    /// <summary>
    /// 输出包含 12 个字段的 Error 级别日志。
    /// </summary>
    public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Error)
        {
            return;
        }
        ThreadLocalLogger.Current.write12(prefix.Bytes(), ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, "error", file, member, line);
    }

    /// <summary>
    /// 输出包含 13 个字段的 Error 级别日志。
    /// </summary>
    public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Error)
        {
            return;
        }
        ThreadLocalLogger.Current.write13(prefix.Bytes(), ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, "error", file, member, line);
    }

    /// <summary>
    /// 输出包含 14 个字段的 Error 级别日志。
    /// </summary>
    public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Error)
        {
            return;
        }
        ThreadLocalLogger.Current.write14(prefix.Bytes(), ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, "error", file, member, line);
    }

    /// <summary>
    /// 输出包含 15 个字段的 Error 级别日志。
    /// </summary>
    public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Error)
        {
            return;
        }
        ThreadLocalLogger.Current.write15(prefix.Bytes(), ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, "error", file, member, line);
    }

    /// <summary>
    /// 输出包含 16 个字段的 Error 级别日志。
    /// </summary>
    public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Error)
        {
            return;
        }
        ThreadLocalLogger.Current.write16(prefix.Bytes(), ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, "error", file, member, line);
    }

    /// <summary>
    /// 输出包含 17 个字段的 Error 级别日志。
    /// </summary>
    public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Error)
        {
            return;
        }
        ThreadLocalLogger.Current.write17(prefix.Bytes(), ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, "error", file, member, line);
    }

    /// <summary>
    /// 输出包含 18 个字段的 Error 级别日志。
    /// </summary>
    public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Error)
        {
            return;
        }
        ThreadLocalLogger.Current.write18(prefix.Bytes(), ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, ref field18, "error", file, member, line);
    }

    /// <summary>
    /// 输出包含 19 个字段的 Error 级别日志。
    /// </summary>
    public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18, Field field19,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Error)
        {
            return;
        }
        ThreadLocalLogger.Current.write19(prefix.Bytes(), ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, ref field18, ref field19, "error", file, member, line);
    }

    /// <summary>
    /// 输出包含 20 个字段的 Error 级别日志。
    /// </summary>
    public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18, Field field19, Field field20,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Error)
        {
            return;
        }
        ThreadLocalLogger.Current.write20(prefix.Bytes(), ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, ref field18, ref field19, ref field20, "error", file, member, line);
    }
}
