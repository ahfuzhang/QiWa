using System.Runtime.CompilerServices;

namespace ConsoleLogger
{
    public partial class ThreadLocalLogger
    {
        public void Fatal(Field field1,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            write1(Logger.Instance.TagPrefix, ref field1, "fatal", file, member, line);
        }

        public void Fatal(Field field1, Field field2,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            write2(Logger.Instance.TagPrefix, ref field1, ref field2, "fatal", file, member, line);
        }

        public void Fatal(Field field1, Field field2, Field field3,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            write3(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, "fatal", file, member, line);
        }

        public void Fatal(Field field1, Field field2, Field field3, Field field4,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            write4(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, "fatal", file, member, line);
        }

        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            write5(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, "fatal", file, member, line);
        }

        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            write6(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, "fatal", file, member, line);
        }

        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            write7(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, "fatal", file, member, line);
        }

        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            write8(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, "fatal", file, member, line);
        }

        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            write9(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, "fatal", file, member, line);
        }

        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            write10(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, "fatal", file, member, line);
        }

        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            write11(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, "fatal", file, member, line);
        }

        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            write12(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, "fatal", file, member, line);
        }

        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            write13(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, "fatal", file, member, line);
        }

        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            write14(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, "fatal", file, member, line);
        }

        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            write15(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, "fatal", file, member, line);
        }

        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            write16(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, "fatal", file, member, line);
        }

        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            write17(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, "fatal", file, member, line);
        }

        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            write18(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, ref field18, "fatal", file, member, line);
        }

        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18, Field field19,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            write19(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, ref field18, ref field19, "fatal", file, member, line);
        }

        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18, Field field19, Field field20,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            write20(Logger.Instance.TagPrefix, ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, ref field18, ref field19, ref field20, "fatal", file, member, line);
        }
    }
}
