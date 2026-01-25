using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Common;

namespace Log {
    public class TaskLogger {
        private readonly Common.RentedBuffer _prefix;
        const int defaultPrefixLen = 512;
        const int minLenOfFirstTag = 6;

        public TaskLogger() {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            _prefix = new Common.RentedBuffer(defaultPrefixLen);
            if (Logger.Instance.TagPrefix.Length > 0) {
                _prefix.Append(Logger.Instance.TagPrefix);
            }
            else {
                _prefix.Append((byte)'{');
            }
        }

        ~TaskLogger() {
            _prefix.Dispose();
        }

        private TaskLogger(Common.RentedBuffer prefix) {
            _prefix = prefix;
        }

        public TaskLogger WithFields(Field field) {
            Common.RentedBuffer cloned = _prefix.Clone();
            if (_prefix.Length >= minLenOfFirstTag) {
                cloned.Append((byte)',');
            }
            field.WriteTo(ref cloned);
            return new TaskLogger(cloned);
        }

        #region WithFields
        public TaskLogger WithFields(Field field1, Field field2) {
            Common.RentedBuffer cloned = _prefix.Clone();
            if (_prefix.Length >= minLenOfFirstTag) {
                cloned.Append((byte)',');
            }
            field1.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field2.WriteTo(ref cloned);
            return new TaskLogger(cloned);
        }

        public TaskLogger WithFields(Field field1, Field field2, Field field3) {
            Common.RentedBuffer cloned = _prefix.Clone();
            if (_prefix.Length >= minLenOfFirstTag) {
                cloned.Append((byte)',');
            }
            field1.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field2.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field3.WriteTo(ref cloned);
            return new TaskLogger(cloned);
        }

        public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4) {
            Common.RentedBuffer cloned = _prefix.Clone();
            if (_prefix.Length >= minLenOfFirstTag) {
                cloned.Append((byte)',');
            }
            field1.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field2.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field3.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field4.WriteTo(ref cloned);
            return new TaskLogger(cloned);
        }

        public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5) {
            Common.RentedBuffer cloned = _prefix.Clone();
            if (_prefix.Length >= minLenOfFirstTag) {
                cloned.Append((byte)',');
            }
            field1.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field2.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field3.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field4.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field5.WriteTo(ref cloned);
            return new TaskLogger(cloned);
        }

        public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6) {
            Common.RentedBuffer cloned = _prefix.Clone();
            if (_prefix.Length >= minLenOfFirstTag) {
                cloned.Append((byte)',');
            }
            field1.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field2.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field3.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field4.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field5.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field6.WriteTo(ref cloned);
            return new TaskLogger(cloned);
        }

        public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7) {
            Common.RentedBuffer cloned = _prefix.Clone();
            if (_prefix.Length >= minLenOfFirstTag) {
                cloned.Append((byte)',');
            }
            field1.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field2.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field3.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field4.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field5.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field6.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field7.WriteTo(ref cloned);
            return new TaskLogger(cloned);
        }

        public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8) {
            Common.RentedBuffer cloned = _prefix.Clone();
            if (_prefix.Length >= minLenOfFirstTag) {
                cloned.Append((byte)',');
            }
            field1.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field2.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field3.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field4.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field5.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field6.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field7.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field8.WriteTo(ref cloned);
            return new TaskLogger(cloned);
        }

        public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9) {
            Common.RentedBuffer cloned = _prefix.Clone();
            if (_prefix.Length >= minLenOfFirstTag) {
                cloned.Append((byte)',');
            }
            field1.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field2.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field3.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field4.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field5.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field6.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field7.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field8.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field9.WriteTo(ref cloned);
            return new TaskLogger(cloned);
        }

        public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10) {
            Common.RentedBuffer cloned = _prefix.Clone();
            if (_prefix.Length >= minLenOfFirstTag) {
                cloned.Append((byte)',');
            }
            field1.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field2.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field3.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field4.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field5.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field6.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field7.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field8.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field9.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field10.WriteTo(ref cloned);
            return new TaskLogger(cloned);
        }

        public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11) {
            Common.RentedBuffer cloned = _prefix.Clone();
            if (_prefix.Length >= minLenOfFirstTag) {
                cloned.Append((byte)',');
            }
            field1.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field2.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field3.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field4.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field5.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field6.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field7.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field8.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field9.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field10.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field11.WriteTo(ref cloned);
            return new TaskLogger(cloned);
        }

        public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12) {
            Common.RentedBuffer cloned = _prefix.Clone();
            if (_prefix.Length >= minLenOfFirstTag) {
                cloned.Append((byte)',');
            }
            field1.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field2.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field3.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field4.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field5.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field6.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field7.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field8.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field9.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field10.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field11.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field12.WriteTo(ref cloned);
            return new TaskLogger(cloned);
        }

        public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13) {
            Common.RentedBuffer cloned = _prefix.Clone();
            if (_prefix.Length >= minLenOfFirstTag) {
                cloned.Append((byte)',');
            }
            field1.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field2.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field3.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field4.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field5.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field6.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field7.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field8.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field9.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field10.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field11.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field12.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field13.WriteTo(ref cloned);
            return new TaskLogger(cloned);
        }

        public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14) {
            Common.RentedBuffer cloned = _prefix.Clone();
            if (_prefix.Length >= minLenOfFirstTag) {
                cloned.Append((byte)',');
            }
            field1.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field2.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field3.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field4.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field5.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field6.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field7.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field8.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field9.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field10.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field11.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field12.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field13.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field14.WriteTo(ref cloned);
            return new TaskLogger(cloned);
        }

        public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15) {
            Common.RentedBuffer cloned = _prefix.Clone();
            if (_prefix.Length >= minLenOfFirstTag) {
                cloned.Append((byte)',');
            }
            field1.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field2.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field3.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field4.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field5.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field6.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field7.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field8.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field9.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field10.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field11.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field12.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field13.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field14.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field15.WriteTo(ref cloned);
            return new TaskLogger(cloned);
        }

        public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16) {
            Common.RentedBuffer cloned = _prefix.Clone();
            if (_prefix.Length >= minLenOfFirstTag) {
                cloned.Append((byte)',');
            }
            field1.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field2.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field3.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field4.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field5.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field6.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field7.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field8.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field9.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field10.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field11.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field12.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field13.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field14.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field15.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field16.WriteTo(ref cloned);
            return new TaskLogger(cloned);
        }

        public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17) {
            Common.RentedBuffer cloned = _prefix.Clone();
            if (_prefix.Length >= minLenOfFirstTag) {
                cloned.Append((byte)',');
            }
            field1.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field2.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field3.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field4.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field5.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field6.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field7.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field8.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field9.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field10.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field11.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field12.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field13.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field14.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field15.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field16.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field17.WriteTo(ref cloned);
            return new TaskLogger(cloned);
        }

        public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18) {
            Common.RentedBuffer cloned = _prefix.Clone();
            if (_prefix.Length >= minLenOfFirstTag) {
                cloned.Append((byte)',');
            }
            field1.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field2.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field3.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field4.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field5.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field6.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field7.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field8.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field9.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field10.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field11.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field12.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field13.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field14.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field15.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field16.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field17.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field18.WriteTo(ref cloned);
            return new TaskLogger(cloned);
        }

        public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18, Field field19) {
            Common.RentedBuffer cloned = _prefix.Clone();
            if (_prefix.Length >= minLenOfFirstTag) {
                cloned.Append((byte)',');
            }
            field1.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field2.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field3.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field4.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field5.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field6.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field7.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field8.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field9.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field10.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field11.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field12.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field13.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field14.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field15.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field16.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field17.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field18.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field19.WriteTo(ref cloned);
            return new TaskLogger(cloned);
        }

        public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18, Field field19, Field field20) {
            Common.RentedBuffer cloned = _prefix.Clone();
            if (_prefix.Length >= minLenOfFirstTag) {
                cloned.Append((byte)',');
            }
            field1.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field2.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field3.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field4.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field5.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field6.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field7.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field8.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field9.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field10.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field11.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field12.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field13.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field14.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field15.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field16.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field17.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field18.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field19.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field20.WriteTo(ref cloned);
            return new TaskLogger(cloned);
        }
        #endregion

        public void Info(Field field1,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Info) {
                return;
            }
            _write1(ref field1, "info", file, member, line);
        }

        #region Info
        public void Info(Field field1, Field field2,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Info) {
                return;
            }
            _write2(ref field1, ref field2, "info", file, member, line);
        }
        public void Info(Field field1, Field field2, Field field3,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Info) {
                return;
            }
            _write3(ref field1, ref field2, ref field3, "info", file, member, line);
        }
        public void Info(Field field1, Field field2, Field field3, Field field4,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Info) {
                return;
            }
            _write4(ref field1, ref field2, ref field3, ref field4, "info", file, member, line);
        }
        public void Info(Field field1, Field field2, Field field3, Field field4, Field field5,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Info) {
                return;
            }
            _write5(ref field1, ref field2, ref field3, ref field4, ref field5, "info", file, member, line);
        }
        public void Info(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Info) {
                return;
            }
            _write6(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, "info", file, member, line);
        }
        public void Info(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Info) {
                return;
            }
            _write7(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, "info", file, member, line);
        }
        public void Info(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Info) {
                return;
            }
            _write8(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, "info", file, member, line);
        }
        public void Info(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Info) {
                return;
            }
            _write9(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, "info", file, member, line);
        }
        public void Info(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Info) {
                return;
            }
            _write10(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, "info", file, member, line);
        }
        public void Info(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Info) {
                return;
            }
            _write11(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, "info", file, member, line);
        }
        public void Info(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Info) {
                return;
            }
            _write12(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, "info", file, member, line);
        }
        public void Info(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Info) {
                return;
            }
            _write13(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, "info", file, member, line);
        }
        public void Info(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Info) {
                return;
            }
            _write14(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, "info", file, member, line);
        }
        public void Info(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Info) {
                return;
            }
            _write15(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, "info", file, member, line);
        }
        public void Info(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Info) {
                return;
            }
            _write16(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, "info", file, member, line);
        }
        public void Info(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Info) {
                return;
            }
            _write17(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, "info", file, member, line);
        }
        public void Info(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Info) {
                return;
            }
            _write18(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, ref field18, "info", file, member, line);
        }
        public void Info(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18, Field field19,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Info) {
                return;
            }
            _write19(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, ref field18, ref field19, "info", file, member, line);
        }
        public void Info(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18, Field field19, Field field20,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Info) {
                return;
            }
            _write20(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, ref field18, ref field19, ref field20, "info", file, member, line);
        }
        #endregion


        #region Debug
        public void Debug(Field field1,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Debug) {
                return;
            }
            _write1(ref field1, "debug", file, member, line);
        }
        public void Debug(Field field1, Field field2,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Debug) {
                return;
            }
            _write2(ref field1, ref field2, "debug", file, member, line);
        }
        public void Debug(Field field1, Field field2, Field field3,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Debug) {
                return;
            }
            _write3(ref field1, ref field2, ref field3, "debug", file, member, line);
        }
        public void Debug(Field field1, Field field2, Field field3, Field field4,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Debug) {
                return;
            }
            _write4(ref field1, ref field2, ref field3, ref field4, "debug", file, member, line);
        }
        public void Debug(Field field1, Field field2, Field field3, Field field4, Field field5,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Debug) {
                return;
            }
            _write5(ref field1, ref field2, ref field3, ref field4, ref field5, "debug", file, member, line);
        }
        public void Debug(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Debug) {
                return;
            }
            _write6(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, "debug", file, member, line);
        }
        public void Debug(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Debug) {
                return;
            }
            _write7(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, "debug", file, member, line);
        }
        public void Debug(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Debug) {
                return;
            }
            _write8(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, "debug", file, member, line);
        }
        public void Debug(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Debug) {
                return;
            }
            _write9(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, "debug", file, member, line);
        }
        public void Debug(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Debug) {
                return;
            }
            _write10(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, "debug", file, member, line);
        }
        public void Debug(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Debug) {
                return;
            }
            _write11(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, "debug", file, member, line);
        }
        public void Debug(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Debug) {
                return;
            }
            _write12(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, "debug", file, member, line);
        }
        public void Debug(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Debug) {
                return;
            }
            _write13(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, "debug", file, member, line);
        }
        public void Debug(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Debug) {
                return;
            }
            _write14(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, "debug", file, member, line);
        }
        public void Debug(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Debug) {
                return;
            }
            _write15(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, "debug", file, member, line);
        }
        public void Debug(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Debug) {
                return;
            }
            _write16(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, "debug", file, member, line);
        }
        public void Debug(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Debug) {
                return;
            }
            _write17(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, "debug", file, member, line);
        }
        public void Debug(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Debug) {
                return;
            }
            _write18(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, ref field18, "debug", file, member, line);
        }
        public void Debug(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18, Field field19,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Debug) {
                return;
            }
            _write19(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, ref field18, ref field19, "debug", file, member, line);
        }
        public void Debug(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18, Field field19, Field field20,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Debug) {
                return;
            }
            _write20(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, ref field18, ref field19, ref field20, "debug", file, member, line);
        }
        #endregion
        #region Warn
        public void Warn(Field field1,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Warn) {
                return;
            }
            _write1(ref field1, "warn", file, member, line);
        }
        public void Warn(Field field1, Field field2,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Warn) {
                return;
            }
            _write2(ref field1, ref field2, "warn", file, member, line);
        }
        public void Warn(Field field1, Field field2, Field field3,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Warn) {
                return;
            }
            _write3(ref field1, ref field2, ref field3, "warn", file, member, line);
        }
        public void Warn(Field field1, Field field2, Field field3, Field field4,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Warn) {
                return;
            }
            _write4(ref field1, ref field2, ref field3, ref field4, "warn", file, member, line);
        }
        public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Warn) {
                return;
            }
            _write5(ref field1, ref field2, ref field3, ref field4, ref field5, "warn", file, member, line);
        }
        public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Warn) {
                return;
            }
            _write6(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, "warn", file, member, line);
        }
        public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Warn) {
                return;
            }
            _write7(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, "warn", file, member, line);
        }
        public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Warn) {
                return;
            }
            _write8(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, "warn", file, member, line);
        }
        public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Warn) {
                return;
            }
            _write9(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, "warn", file, member, line);
        }
        public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Warn) {
                return;
            }
            _write10(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, "warn", file, member, line);
        }
        public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Warn) {
                return;
            }
            _write11(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, "warn", file, member, line);
        }
        public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Warn) {
                return;
            }
            _write12(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, "warn", file, member, line);
        }
        public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Warn) {
                return;
            }
            _write13(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, "warn", file, member, line);
        }
        public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Warn) {
                return;
            }
            _write14(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, "warn", file, member, line);
        }
        public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Warn) {
                return;
            }
            _write15(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, "warn", file, member, line);
        }
        public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Warn) {
                return;
            }
            _write16(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, "warn", file, member, line);
        }
        public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Warn) {
                return;
            }
            _write17(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, "warn", file, member, line);
        }
        public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Warn) {
                return;
            }
            _write18(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, ref field18, "warn", file, member, line);
        }
        public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18, Field field19,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Warn) {
                return;
            }
            _write19(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, ref field18, ref field19, "warn", file, member, line);
        }
        public void Warn(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18, Field field19, Field field20,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Warn) {
                return;
            }
            _write20(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, ref field18, ref field19, ref field20, "warn", file, member, line);
        }
        #endregion
        #region Error
        public void Error(Field field1,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Error) {
                return;
            }
            _write1(ref field1, "error", file, member, line);
        }
        public void Error(Field field1, Field field2,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Error) {
                return;
            }
            _write2(ref field1, ref field2, "error", file, member, line);
        }
        public void Error(Field field1, Field field2, Field field3,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Error) {
                return;
            }
            _write3(ref field1, ref field2, ref field3, "error", file, member, line);
        }
        public void Error(Field field1, Field field2, Field field3, Field field4,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Error) {
                return;
            }
            _write4(ref field1, ref field2, ref field3, ref field4, "error", file, member, line);
        }
        public void Error(Field field1, Field field2, Field field3, Field field4, Field field5,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Error) {
                return;
            }
            _write5(ref field1, ref field2, ref field3, ref field4, ref field5, "error", file, member, line);
        }
        public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Error) {
                return;
            }
            _write6(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, "error", file, member, line);
        }
        public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Error) {
                return;
            }
            _write7(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, "error", file, member, line);
        }
        public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Error) {
                return;
            }
            _write8(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, "error", file, member, line);
        }
        public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Error) {
                return;
            }
            _write9(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, "error", file, member, line);
        }
        public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Error) {
                return;
            }
            _write10(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, "error", file, member, line);
        }
        public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Error) {
                return;
            }
            _write11(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, "error", file, member, line);
        }
        public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Error) {
                return;
            }
            _write12(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, "error", file, member, line);
        }
        public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Error) {
                return;
            }
            _write13(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, "error", file, member, line);
        }
        public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Error) {
                return;
            }
            _write14(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, "error", file, member, line);
        }
        public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Error) {
                return;
            }
            _write15(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, "error", file, member, line);
        }
        public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Error) {
                return;
            }
            _write16(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, "error", file, member, line);
        }
        public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Error) {
                return;
            }
            _write17(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, "error", file, member, line);
        }
        public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Error) {
                return;
            }
            _write18(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, ref field18, "error", file, member, line);
        }
        public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18, Field field19,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Error) {
                return;
            }
            _write19(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, ref field18, ref field19, "error", file, member, line);
        }
        public void Error(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18, Field field19, Field field20,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            if (Logger.Instance.Level < LogLevel.Error) {
                return;
            }
            _write20(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, ref field18, ref field19, ref field20, "error", file, member, line);
        }
        #endregion
        #region Fatal
        public void Fatal(Field field1,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            _write1(ref field1, "fatal", file, member, line);
        }
        public void Fatal(Field field1, Field field2,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            _write2(ref field1, ref field2, "fatal", file, member, line);
        }
        public void Fatal(Field field1, Field field2, Field field3,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            _write3(ref field1, ref field2, ref field3, "fatal", file, member, line);
        }
        public void Fatal(Field field1, Field field2, Field field3, Field field4,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            _write4(ref field1, ref field2, ref field3, ref field4, "fatal", file, member, line);
        }
        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            _write5(ref field1, ref field2, ref field3, ref field4, ref field5, "fatal", file, member, line);
        }
        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            _write6(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, "fatal", file, member, line);
        }
        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            _write7(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, "fatal", file, member, line);
        }
        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            _write8(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, "fatal", file, member, line);
        }
        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            _write9(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, "fatal", file, member, line);
        }
        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            _write10(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, "fatal", file, member, line);
        }
        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            _write11(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, "fatal", file, member, line);
        }
        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            _write12(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, "fatal", file, member, line);
        }
        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            _write13(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, "fatal", file, member, line);
        }
        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            _write14(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, "fatal", file, member, line);
        }
        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            _write15(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, "fatal", file, member, line);
        }
        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            _write16(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, "fatal", file, member, line);
        }
        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            _write17(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, "fatal", file, member, line);
        }
        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            _write18(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, ref field18, "fatal", file, member, line);
        }
        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18, Field field19,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            _write19(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, ref field18, ref field19, "fatal", file, member, line);
        }
        public void Fatal(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18, Field field19, Field field20,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            System.Diagnostics.Debug.Assert(Logger.Instance != null);
            _write20(ref field1, ref field2, ref field3, ref field4, ref field5, ref field6, ref field7, ref field8, ref field9, ref field10, ref field11, ref field12, ref field13, ref field14, ref field15, ref field16, ref field17, ref field18, ref field19, ref field20, "fatal", file, member, line);
        }
        #endregion

        private void _write1(ref Field field1, string levelStr, string file, string member, int line) {
            var logger = ThreadLocalLogger.Current;
            using (var _ = new ScopeGuard(() => {
                logger.UnLock();
            })) {
                logger.Lock();
                ref Common.RentedBuffer buf = ref logger.Buffer;
                //
                buf.Append(_prefix.Data.AsSpan<byte>(0, _prefix.Length));
                if (_prefix.Length >= minLenOfFirstTag) {
                    buf.Append((byte)',');
                }
                field1.WriteTo(ref buf);
                // todo: 陆续追加 field2 ... field20
                _writeTail(ref buf, levelStr, file, member, line);
                logger.Flush();
            }
        }

        #region _writeN
        private void _write2(ref Field field1, ref Field field2, string levelStr, string file, string member, int line) {
            var logger = ThreadLocalLogger.Current;
            using (var _ = new ScopeGuard(() => {
                logger.UnLock();
            })) {
                logger.Lock();
                ref Common.RentedBuffer buf = ref logger.Buffer;
                //
                buf.Append(_prefix.Data.AsSpan<byte>(0, _prefix.Length));
                if (_prefix.Length >= minLenOfFirstTag) {
                    buf.Append((byte)',');
                }
                field1.WriteTo(ref buf);
                buf.Append((byte)',');
                field2.WriteTo(ref buf);
                _writeTail(ref buf, levelStr, file, member, line);
                logger.Flush();
            }
        }
        private void _write3(ref Field field1, ref Field field2, ref Field field3, string levelStr, string file, string member, int line) {
            var logger = ThreadLocalLogger.Current;
            using (var _ = new ScopeGuard(() => {
                logger.UnLock();
            })) {
                logger.Lock();
                ref Common.RentedBuffer buf = ref logger.Buffer;
                //
                buf.Append(_prefix.Data.AsSpan<byte>(0, _prefix.Length));
                if (_prefix.Length >= minLenOfFirstTag) {
                    buf.Append((byte)',');
                }
                field1.WriteTo(ref buf);
                buf.Append((byte)',');
                field2.WriteTo(ref buf);
                buf.Append((byte)',');
                field3.WriteTo(ref buf);
                _writeTail(ref buf, levelStr, file, member, line);
                logger.Flush();
            }
        }
        private void _write4(ref Field field1, ref Field field2, ref Field field3, ref Field field4, string levelStr, string file, string member, int line) {
            var logger = ThreadLocalLogger.Current;
            using (var _ = new ScopeGuard(() => {
                logger.UnLock();
            })) {
                logger.Lock();
                ref Common.RentedBuffer buf = ref logger.Buffer;
                //
                buf.Append(_prefix.Data.AsSpan<byte>(0, _prefix.Length));
                if (_prefix.Length >= minLenOfFirstTag) {
                    buf.Append((byte)',');
                }
                field1.WriteTo(ref buf);
                buf.Append((byte)',');
                field2.WriteTo(ref buf);
                buf.Append((byte)',');
                field3.WriteTo(ref buf);
                buf.Append((byte)',');
                field4.WriteTo(ref buf);
                _writeTail(ref buf, levelStr, file, member, line);
                logger.Flush();
            }
        }
        private void _write5(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, string levelStr, string file, string member, int line) {
            var logger = ThreadLocalLogger.Current;
            using (var _ = new ScopeGuard(() => {
                logger.UnLock();
            })) {
                logger.Lock();
                ref Common.RentedBuffer buf = ref logger.Buffer;
                //
                buf.Append(_prefix.Data.AsSpan<byte>(0, _prefix.Length));
                if (_prefix.Length >= minLenOfFirstTag) {
                    buf.Append((byte)',');
                }
                field1.WriteTo(ref buf);
                buf.Append((byte)',');
                field2.WriteTo(ref buf);
                buf.Append((byte)',');
                field3.WriteTo(ref buf);
                buf.Append((byte)',');
                field4.WriteTo(ref buf);
                buf.Append((byte)',');
                field5.WriteTo(ref buf);
                _writeTail(ref buf, levelStr, file, member, line);
                logger.Flush();
            }
        }
        private void _write6(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, string levelStr, string file, string member, int line) {
            var logger = ThreadLocalLogger.Current;
            using (var _ = new ScopeGuard(() => {
                logger.UnLock();
            })) {
                logger.Lock();
                ref Common.RentedBuffer buf = ref logger.Buffer;
                //
                buf.Append(_prefix.Data.AsSpan<byte>(0, _prefix.Length));
                if (_prefix.Length >= minLenOfFirstTag) {
                    buf.Append((byte)',');
                }
                field1.WriteTo(ref buf);
                buf.Append((byte)',');
                field2.WriteTo(ref buf);
                buf.Append((byte)',');
                field3.WriteTo(ref buf);
                buf.Append((byte)',');
                field4.WriteTo(ref buf);
                buf.Append((byte)',');
                field5.WriteTo(ref buf);
                buf.Append((byte)',');
                field6.WriteTo(ref buf);
                _writeTail(ref buf, levelStr, file, member, line);
                logger.Flush();
            }
        }
        private void _write7(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, string levelStr, string file, string member, int line) {
            var logger = ThreadLocalLogger.Current;
            using (var _ = new ScopeGuard(() => {
                logger.UnLock();
            })) {
                logger.Lock();
                ref Common.RentedBuffer buf = ref logger.Buffer;
                //
                buf.Append(_prefix.Data.AsSpan<byte>(0, _prefix.Length));
                if (_prefix.Length >= minLenOfFirstTag) {
                    buf.Append((byte)',');
                }
                field1.WriteTo(ref buf);
                buf.Append((byte)',');
                field2.WriteTo(ref buf);
                buf.Append((byte)',');
                field3.WriteTo(ref buf);
                buf.Append((byte)',');
                field4.WriteTo(ref buf);
                buf.Append((byte)',');
                field5.WriteTo(ref buf);
                buf.Append((byte)',');
                field6.WriteTo(ref buf);
                buf.Append((byte)',');
                field7.WriteTo(ref buf);
                _writeTail(ref buf, levelStr, file, member, line);
                logger.Flush();
            }
        }
        private void _write8(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, string levelStr, string file, string member, int line) {
            var logger = ThreadLocalLogger.Current;
            using (var _ = new ScopeGuard(() => {
                logger.UnLock();
            })) {
                logger.Lock();
                ref Common.RentedBuffer buf = ref logger.Buffer;
                //
                buf.Append(_prefix.Data.AsSpan<byte>(0, _prefix.Length));
                if (_prefix.Length >= minLenOfFirstTag) {
                    buf.Append((byte)',');
                }
                field1.WriteTo(ref buf);
                buf.Append((byte)',');
                field2.WriteTo(ref buf);
                buf.Append((byte)',');
                field3.WriteTo(ref buf);
                buf.Append((byte)',');
                field4.WriteTo(ref buf);
                buf.Append((byte)',');
                field5.WriteTo(ref buf);
                buf.Append((byte)',');
                field6.WriteTo(ref buf);
                buf.Append((byte)',');
                field7.WriteTo(ref buf);
                buf.Append((byte)',');
                field8.WriteTo(ref buf);
                _writeTail(ref buf, levelStr, file, member, line);
                logger.Flush();
            }
        }
        private void _write9(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, string levelStr, string file, string member, int line) {
            var logger = ThreadLocalLogger.Current;
            using (var _ = new ScopeGuard(() => {
                logger.UnLock();
            })) {
                logger.Lock();
                ref Common.RentedBuffer buf = ref logger.Buffer;
                //
                buf.Append(_prefix.Data.AsSpan<byte>(0, _prefix.Length));
                if (_prefix.Length >= minLenOfFirstTag) {
                    buf.Append((byte)',');
                }
                field1.WriteTo(ref buf);
                buf.Append((byte)',');
                field2.WriteTo(ref buf);
                buf.Append((byte)',');
                field3.WriteTo(ref buf);
                buf.Append((byte)',');
                field4.WriteTo(ref buf);
                buf.Append((byte)',');
                field5.WriteTo(ref buf);
                buf.Append((byte)',');
                field6.WriteTo(ref buf);
                buf.Append((byte)',');
                field7.WriteTo(ref buf);
                buf.Append((byte)',');
                field8.WriteTo(ref buf);
                buf.Append((byte)',');
                field9.WriteTo(ref buf);
                _writeTail(ref buf, levelStr, file, member, line);
                logger.Flush();
            }
        }
        private void _write10(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, string levelStr, string file, string member, int line) {
            var logger = ThreadLocalLogger.Current;
            using (var _ = new ScopeGuard(() => {
                logger.UnLock();
            })) {
                logger.Lock();
                ref Common.RentedBuffer buf = ref logger.Buffer;
                //
                buf.Append(_prefix.Data.AsSpan<byte>(0, _prefix.Length));
                if (_prefix.Length >= minLenOfFirstTag) {
                    buf.Append((byte)',');
                }
                field1.WriteTo(ref buf);
                buf.Append((byte)',');
                field2.WriteTo(ref buf);
                buf.Append((byte)',');
                field3.WriteTo(ref buf);
                buf.Append((byte)',');
                field4.WriteTo(ref buf);
                buf.Append((byte)',');
                field5.WriteTo(ref buf);
                buf.Append((byte)',');
                field6.WriteTo(ref buf);
                buf.Append((byte)',');
                field7.WriteTo(ref buf);
                buf.Append((byte)',');
                field8.WriteTo(ref buf);
                buf.Append((byte)',');
                field9.WriteTo(ref buf);
                buf.Append((byte)',');
                field10.WriteTo(ref buf);
                _writeTail(ref buf, levelStr, file, member, line);
                logger.Flush();
            }
        }
        private void _write11(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, string levelStr, string file, string member, int line) {
            var logger = ThreadLocalLogger.Current;
            using (var _ = new ScopeGuard(() => {
                logger.UnLock();
            })) {
                logger.Lock();
                ref Common.RentedBuffer buf = ref logger.Buffer;
                //
                buf.Append(_prefix.Data.AsSpan<byte>(0, _prefix.Length));
                if (_prefix.Length >= minLenOfFirstTag) {
                    buf.Append((byte)',');
                }
                field1.WriteTo(ref buf);
                buf.Append((byte)',');
                field2.WriteTo(ref buf);
                buf.Append((byte)',');
                field3.WriteTo(ref buf);
                buf.Append((byte)',');
                field4.WriteTo(ref buf);
                buf.Append((byte)',');
                field5.WriteTo(ref buf);
                buf.Append((byte)',');
                field6.WriteTo(ref buf);
                buf.Append((byte)',');
                field7.WriteTo(ref buf);
                buf.Append((byte)',');
                field8.WriteTo(ref buf);
                buf.Append((byte)',');
                field9.WriteTo(ref buf);
                buf.Append((byte)',');
                field10.WriteTo(ref buf);
                buf.Append((byte)',');
                field11.WriteTo(ref buf);
                _writeTail(ref buf, levelStr, file, member, line);
                logger.Flush();
            }
        }
        private void _write12(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, ref Field field12, string levelStr, string file, string member, int line) {
            var logger = ThreadLocalLogger.Current;
            using (var _ = new ScopeGuard(() => {
                logger.UnLock();
            })) {
                logger.Lock();
                ref Common.RentedBuffer buf = ref logger.Buffer;
                //
                buf.Append(_prefix.Data.AsSpan<byte>(0, _prefix.Length));
                if (_prefix.Length >= minLenOfFirstTag) {
                    buf.Append((byte)',');
                }
                field1.WriteTo(ref buf);
                buf.Append((byte)',');
                field2.WriteTo(ref buf);
                buf.Append((byte)',');
                field3.WriteTo(ref buf);
                buf.Append((byte)',');
                field4.WriteTo(ref buf);
                buf.Append((byte)',');
                field5.WriteTo(ref buf);
                buf.Append((byte)',');
                field6.WriteTo(ref buf);
                buf.Append((byte)',');
                field7.WriteTo(ref buf);
                buf.Append((byte)',');
                field8.WriteTo(ref buf);
                buf.Append((byte)',');
                field9.WriteTo(ref buf);
                buf.Append((byte)',');
                field10.WriteTo(ref buf);
                buf.Append((byte)',');
                field11.WriteTo(ref buf);
                buf.Append((byte)',');
                field12.WriteTo(ref buf);
                _writeTail(ref buf, levelStr, file, member, line);
                logger.Flush();
            }
        }
        private void _write13(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, ref Field field12, ref Field field13, string levelStr, string file, string member, int line) {
            var logger = ThreadLocalLogger.Current;
            using (var _ = new ScopeGuard(() => {
                logger.UnLock();
            })) {
                logger.Lock();
                ref Common.RentedBuffer buf = ref logger.Buffer;
                //
                buf.Append(_prefix.Data.AsSpan<byte>(0, _prefix.Length));
                if (_prefix.Length >= minLenOfFirstTag) {
                    buf.Append((byte)',');
                }
                field1.WriteTo(ref buf);
                buf.Append((byte)',');
                field2.WriteTo(ref buf);
                buf.Append((byte)',');
                field3.WriteTo(ref buf);
                buf.Append((byte)',');
                field4.WriteTo(ref buf);
                buf.Append((byte)',');
                field5.WriteTo(ref buf);
                buf.Append((byte)',');
                field6.WriteTo(ref buf);
                buf.Append((byte)',');
                field7.WriteTo(ref buf);
                buf.Append((byte)',');
                field8.WriteTo(ref buf);
                buf.Append((byte)',');
                field9.WriteTo(ref buf);
                buf.Append((byte)',');
                field10.WriteTo(ref buf);
                buf.Append((byte)',');
                field11.WriteTo(ref buf);
                buf.Append((byte)',');
                field12.WriteTo(ref buf);
                buf.Append((byte)',');
                field13.WriteTo(ref buf);
                _writeTail(ref buf, levelStr, file, member, line);
                logger.Flush();
            }
        }
        private void _write14(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, ref Field field12, ref Field field13, ref Field field14, string levelStr, string file, string member, int line) {
            var logger = ThreadLocalLogger.Current;
            using (var _ = new ScopeGuard(() => {
                logger.UnLock();
            })) {
                logger.Lock();
                ref Common.RentedBuffer buf = ref logger.Buffer;
                //
                buf.Append(_prefix.Data.AsSpan<byte>(0, _prefix.Length));
                if (_prefix.Length >= minLenOfFirstTag) {
                    buf.Append((byte)',');
                }
                field1.WriteTo(ref buf);
                buf.Append((byte)',');
                field2.WriteTo(ref buf);
                buf.Append((byte)',');
                field3.WriteTo(ref buf);
                buf.Append((byte)',');
                field4.WriteTo(ref buf);
                buf.Append((byte)',');
                field5.WriteTo(ref buf);
                buf.Append((byte)',');
                field6.WriteTo(ref buf);
                buf.Append((byte)',');
                field7.WriteTo(ref buf);
                buf.Append((byte)',');
                field8.WriteTo(ref buf);
                buf.Append((byte)',');
                field9.WriteTo(ref buf);
                buf.Append((byte)',');
                field10.WriteTo(ref buf);
                buf.Append((byte)',');
                field11.WriteTo(ref buf);
                buf.Append((byte)',');
                field12.WriteTo(ref buf);
                buf.Append((byte)',');
                field13.WriteTo(ref buf);
                buf.Append((byte)',');
                field14.WriteTo(ref buf);
                _writeTail(ref buf, levelStr, file, member, line);
                logger.Flush();
            }
        }
        private void _write15(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, ref Field field12, ref Field field13, ref Field field14, ref Field field15, string levelStr, string file, string member, int line) {
            var logger = ThreadLocalLogger.Current;
            using (var _ = new ScopeGuard(() => {
                logger.UnLock();
            })) {
                logger.Lock();
                ref Common.RentedBuffer buf = ref logger.Buffer;
                //
                buf.Append(_prefix.Data.AsSpan<byte>(0, _prefix.Length));
                if (_prefix.Length >= minLenOfFirstTag) {
                    buf.Append((byte)',');
                }
                field1.WriteTo(ref buf);
                buf.Append((byte)',');
                field2.WriteTo(ref buf);
                buf.Append((byte)',');
                field3.WriteTo(ref buf);
                buf.Append((byte)',');
                field4.WriteTo(ref buf);
                buf.Append((byte)',');
                field5.WriteTo(ref buf);
                buf.Append((byte)',');
                field6.WriteTo(ref buf);
                buf.Append((byte)',');
                field7.WriteTo(ref buf);
                buf.Append((byte)',');
                field8.WriteTo(ref buf);
                buf.Append((byte)',');
                field9.WriteTo(ref buf);
                buf.Append((byte)',');
                field10.WriteTo(ref buf);
                buf.Append((byte)',');
                field11.WriteTo(ref buf);
                buf.Append((byte)',');
                field12.WriteTo(ref buf);
                buf.Append((byte)',');
                field13.WriteTo(ref buf);
                buf.Append((byte)',');
                field14.WriteTo(ref buf);
                buf.Append((byte)',');
                field15.WriteTo(ref buf);
                _writeTail(ref buf, levelStr, file, member, line);
                logger.Flush();
            }
        }
        private void _write16(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, ref Field field12, ref Field field13, ref Field field14, ref Field field15, ref Field field16, string levelStr, string file, string member, int line) {
            var logger = ThreadLocalLogger.Current;
            using (var _ = new ScopeGuard(() => {
                logger.UnLock();
            })) {
                logger.Lock();
                ref Common.RentedBuffer buf = ref logger.Buffer;
                //
                buf.Append(_prefix.Data.AsSpan<byte>(0, _prefix.Length));
                if (_prefix.Length >= minLenOfFirstTag) {
                    buf.Append((byte)',');
                }
                field1.WriteTo(ref buf);
                buf.Append((byte)',');
                field2.WriteTo(ref buf);
                buf.Append((byte)',');
                field3.WriteTo(ref buf);
                buf.Append((byte)',');
                field4.WriteTo(ref buf);
                buf.Append((byte)',');
                field5.WriteTo(ref buf);
                buf.Append((byte)',');
                field6.WriteTo(ref buf);
                buf.Append((byte)',');
                field7.WriteTo(ref buf);
                buf.Append((byte)',');
                field8.WriteTo(ref buf);
                buf.Append((byte)',');
                field9.WriteTo(ref buf);
                buf.Append((byte)',');
                field10.WriteTo(ref buf);
                buf.Append((byte)',');
                field11.WriteTo(ref buf);
                buf.Append((byte)',');
                field12.WriteTo(ref buf);
                buf.Append((byte)',');
                field13.WriteTo(ref buf);
                buf.Append((byte)',');
                field14.WriteTo(ref buf);
                buf.Append((byte)',');
                field15.WriteTo(ref buf);
                buf.Append((byte)',');
                field16.WriteTo(ref buf);
                _writeTail(ref buf, levelStr, file, member, line);
                logger.Flush();
            }
        }
        private void _write17(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, ref Field field12, ref Field field13, ref Field field14, ref Field field15, ref Field field16, ref Field field17, string levelStr, string file, string member, int line) {
            var logger = ThreadLocalLogger.Current;
            using (var _ = new ScopeGuard(() => {
                logger.UnLock();
            })) {
                logger.Lock();
                ref Common.RentedBuffer buf = ref logger.Buffer;
                //
                buf.Append(_prefix.Data.AsSpan<byte>(0, _prefix.Length));
                if (_prefix.Length >= minLenOfFirstTag) {
                    buf.Append((byte)',');
                }
                field1.WriteTo(ref buf);
                buf.Append((byte)',');
                field2.WriteTo(ref buf);
                buf.Append((byte)',');
                field3.WriteTo(ref buf);
                buf.Append((byte)',');
                field4.WriteTo(ref buf);
                buf.Append((byte)',');
                field5.WriteTo(ref buf);
                buf.Append((byte)',');
                field6.WriteTo(ref buf);
                buf.Append((byte)',');
                field7.WriteTo(ref buf);
                buf.Append((byte)',');
                field8.WriteTo(ref buf);
                buf.Append((byte)',');
                field9.WriteTo(ref buf);
                buf.Append((byte)',');
                field10.WriteTo(ref buf);
                buf.Append((byte)',');
                field11.WriteTo(ref buf);
                buf.Append((byte)',');
                field12.WriteTo(ref buf);
                buf.Append((byte)',');
                field13.WriteTo(ref buf);
                buf.Append((byte)',');
                field14.WriteTo(ref buf);
                buf.Append((byte)',');
                field15.WriteTo(ref buf);
                buf.Append((byte)',');
                field16.WriteTo(ref buf);
                buf.Append((byte)',');
                field17.WriteTo(ref buf);
                _writeTail(ref buf, levelStr, file, member, line);
                logger.Flush();
            }
        }
        private void _write18(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, ref Field field12, ref Field field13, ref Field field14, ref Field field15, ref Field field16, ref Field field17, ref Field field18, string levelStr, string file, string member, int line) {
            var logger = ThreadLocalLogger.Current;
            using (var _ = new ScopeGuard(() => {
                logger.UnLock();
            })) {
                logger.Lock();
                ref Common.RentedBuffer buf = ref logger.Buffer;
                //
                buf.Append(_prefix.Data.AsSpan<byte>(0, _prefix.Length));
                if (_prefix.Length >= minLenOfFirstTag) {
                    buf.Append((byte)',');
                }
                field1.WriteTo(ref buf);
                buf.Append((byte)',');
                field2.WriteTo(ref buf);
                buf.Append((byte)',');
                field3.WriteTo(ref buf);
                buf.Append((byte)',');
                field4.WriteTo(ref buf);
                buf.Append((byte)',');
                field5.WriteTo(ref buf);
                buf.Append((byte)',');
                field6.WriteTo(ref buf);
                buf.Append((byte)',');
                field7.WriteTo(ref buf);
                buf.Append((byte)',');
                field8.WriteTo(ref buf);
                buf.Append((byte)',');
                field9.WriteTo(ref buf);
                buf.Append((byte)',');
                field10.WriteTo(ref buf);
                buf.Append((byte)',');
                field11.WriteTo(ref buf);
                buf.Append((byte)',');
                field12.WriteTo(ref buf);
                buf.Append((byte)',');
                field13.WriteTo(ref buf);
                buf.Append((byte)',');
                field14.WriteTo(ref buf);
                buf.Append((byte)',');
                field15.WriteTo(ref buf);
                buf.Append((byte)',');
                field16.WriteTo(ref buf);
                buf.Append((byte)',');
                field17.WriteTo(ref buf);
                buf.Append((byte)',');
                field18.WriteTo(ref buf);
                _writeTail(ref buf, levelStr, file, member, line);
                logger.Flush();
            }
        }
        private void _write19(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, ref Field field12, ref Field field13, ref Field field14, ref Field field15, ref Field field16, ref Field field17, ref Field field18, ref Field field19, string levelStr, string file, string member, int line) {
            var logger = ThreadLocalLogger.Current;
            using (var _ = new ScopeGuard(() => {
                logger.UnLock();
            })) {
                logger.Lock();
                ref Common.RentedBuffer buf = ref logger.Buffer;
                //
                buf.Append(_prefix.Data.AsSpan<byte>(0, _prefix.Length));
                if (_prefix.Length >= minLenOfFirstTag) {
                    buf.Append((byte)',');
                }
                field1.WriteTo(ref buf);
                buf.Append((byte)',');
                field2.WriteTo(ref buf);
                buf.Append((byte)',');
                field3.WriteTo(ref buf);
                buf.Append((byte)',');
                field4.WriteTo(ref buf);
                buf.Append((byte)',');
                field5.WriteTo(ref buf);
                buf.Append((byte)',');
                field6.WriteTo(ref buf);
                buf.Append((byte)',');
                field7.WriteTo(ref buf);
                buf.Append((byte)',');
                field8.WriteTo(ref buf);
                buf.Append((byte)',');
                field9.WriteTo(ref buf);
                buf.Append((byte)',');
                field10.WriteTo(ref buf);
                buf.Append((byte)',');
                field11.WriteTo(ref buf);
                buf.Append((byte)',');
                field12.WriteTo(ref buf);
                buf.Append((byte)',');
                field13.WriteTo(ref buf);
                buf.Append((byte)',');
                field14.WriteTo(ref buf);
                buf.Append((byte)',');
                field15.WriteTo(ref buf);
                buf.Append((byte)',');
                field16.WriteTo(ref buf);
                buf.Append((byte)',');
                field17.WriteTo(ref buf);
                buf.Append((byte)',');
                field18.WriteTo(ref buf);
                buf.Append((byte)',');
                field19.WriteTo(ref buf);
                _writeTail(ref buf, levelStr, file, member, line);
                logger.Flush();
            }
        }
        private void _write20(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, ref Field field12, ref Field field13, ref Field field14, ref Field field15, ref Field field16, ref Field field17, ref Field field18, ref Field field19, ref Field field20, string levelStr, string file, string member, int line) {
            var logger = ThreadLocalLogger.Current;
            using (var _ = new ScopeGuard(() => {
                logger.UnLock();
            })) {
                logger.Lock();
                ref Common.RentedBuffer buf = ref logger.Buffer;
                //
                buf.Append(_prefix.Data.AsSpan<byte>(0, _prefix.Length));
                if (_prefix.Length >= minLenOfFirstTag) {
                    buf.Append((byte)',');
                }
                field1.WriteTo(ref buf);
                buf.Append((byte)',');
                field2.WriteTo(ref buf);
                buf.Append((byte)',');
                field3.WriteTo(ref buf);
                buf.Append((byte)',');
                field4.WriteTo(ref buf);
                buf.Append((byte)',');
                field5.WriteTo(ref buf);
                buf.Append((byte)',');
                field6.WriteTo(ref buf);
                buf.Append((byte)',');
                field7.WriteTo(ref buf);
                buf.Append((byte)',');
                field8.WriteTo(ref buf);
                buf.Append((byte)',');
                field9.WriteTo(ref buf);
                buf.Append((byte)',');
                field10.WriteTo(ref buf);
                buf.Append((byte)',');
                field11.WriteTo(ref buf);
                buf.Append((byte)',');
                field12.WriteTo(ref buf);
                buf.Append((byte)',');
                field13.WriteTo(ref buf);
                buf.Append((byte)',');
                field14.WriteTo(ref buf);
                buf.Append((byte)',');
                field15.WriteTo(ref buf);
                buf.Append((byte)',');
                field16.WriteTo(ref buf);
                buf.Append((byte)',');
                field17.WriteTo(ref buf);
                buf.Append((byte)',');
                field18.WriteTo(ref buf);
                buf.Append((byte)',');
                field19.WriteTo(ref buf);
                buf.Append((byte)',');
                field20.WriteTo(ref buf);
                _writeTail(ref buf, levelStr, file, member, line);
                logger.Flush();
            }
        }
        #endregion

        private void _writeTail(ref Common.RentedBuffer buf, string levelStr, string file, string member, int line) {
            // 写入公共字段
            buf.Append((byte)',');
            Field.UtcDateTime("_time"u8, System.DateTime.Now).WriteTo(ref buf);
            buf.Append((byte)',');
            Field.String("level"u8, levelStr).WriteTo(ref buf);
            buf.Append((byte)',');
            Field.String("_file"u8, file).WriteTo(ref buf);  // todo: 完整路径太长了，应该做截断
            buf.Append((byte)',');
            Field.String("_member"u8, member).WriteTo(ref buf);
            buf.Append((byte)',');
            Field.Int64("_line"u8, line).WriteTo(ref buf);
            buf.Append("}\n"u8);
        }
    }
}
