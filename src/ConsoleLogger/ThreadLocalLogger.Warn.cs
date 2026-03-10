using System.Runtime.CompilerServices;

namespace ConsoleLogger;

public partial class ThreadLocalLogger
{
    public void Warn(Field field1,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Warn)
        {
            return;
        }
        write1(Logger.Instance.TagPrefix, ref field1, "warn", file, member, line);
    }

    public void Warn(Field field1, Field field2,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Warn)
        {
            return;
        }
        write2(Logger.Instance.TagPrefix, ref field1, ref field2, "warn", file, member, line);
    }

    public void Warn(Field field1, Field field2, Field field3,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Warn)
        {
            return;
        }
        write3(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, "warn", file, member, line);
    }

    public void Warn(Field field1, Field field2, Field field3, Field field4,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Warn)
        {
            return;
        }
        write4(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, "warn", file, member, line);
    }

    public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Warn)
        {
            return;
        }
        write5(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, "warn", file, member, line);
    }

    public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Warn)
        {
            return;
        }
        write6(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, "warn", file, member, line);
    }

    public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Warn)
        {
            return;
        }
        write7(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, "warn", file, member, line);
    }

    public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Warn)
        {
            return;
        }
        write8(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, "warn", file, member, line);
    }

    public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Warn)
        {
            return;
        }
        write9(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, "warn", file, member, line);
    }

    public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Warn)
        {
            return;
        }
        write10(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, "warn", file, member, line);
    }

    public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Warn)
        {
            return;
        }
        write11(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, "warn", file, member, line);
    }

    public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Warn)
        {
            return;
        }
        write12(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, "warn", file, member, line);
    }

    public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Warn)
        {
            return;
        }
        write13(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, "warn", file, member, line);
    }

    public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Warn)
        {
            return;
        }
        write14(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, "warn", file, member, line);
    }

    public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Warn)
        {
            return;
        }
        write15(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, "warn", file, member, line);
    }

    public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Warn)
        {
            return;
        }
        write16(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, "warn", file, member, line);
    }

    public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Warn)
        {
            return;
        }
        write17(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, "warn", file, member, line);
    }

    public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Warn)
        {
            return;
        }
        write18(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, ref field18, "warn", file, member, line);
    }

    public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18, Field field19,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Warn)
        {
            return;
        }
        write19(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, ref field18, ref field19, "warn", file, member, line);
    }

    public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18, Field field19, Field field20,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
    {
        System.Diagnostics.Debug.Assert(Logger.Instance != null);
        if (Logger.Instance.Level < LogLevel.Warn)
        {
            return;
        }
        write20(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, ref field18, ref field19, ref field20, "warn", file, member, line);
    }
}
