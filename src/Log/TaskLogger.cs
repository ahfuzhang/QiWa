using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Common;

namespace Log {
    public ref struct TaskLogger {
        private readonly Common.RentedBuffer _prefix;
        const int defaultPrefixLen = 512;
        const int minLenOfFirstTag = 6;

        public TaskLogger() {
            _prefix = new Common.RentedBuffer(defaultPrefixLen);
            if (Logger.Instance.tagPrefix.Length > 0) {
                _prefix.Append(Logger.Instance.tagPrefix);
            } else {
                _prefix.Append((byte)'{');
            }
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

        public TaskLogger WithFields(ref Field field1, ref Field field2) {
            Common.RentedBuffer cloned = _prefix.Clone();
            if (_prefix.Length >= minLenOfFirstTag) {
                cloned.Append((byte)',');
            }
            field1.WriteTo(ref cloned);
            cloned.Append((byte)',');
            field2.WriteTo(ref cloned);
            return new TaskLogger(cloned);
        }

        public TaskLogger WithFields(ref Field field1, ref Field field2, ref Field field3) {
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

        public TaskLogger WithFields(ref Field field1, ref Field field2, ref Field field3, ref Field field4) {
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

        public TaskLogger WithFields(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5) {
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

        public TaskLogger WithFields(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6) {
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

        public TaskLogger WithFields(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7) {
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

        public TaskLogger WithFields(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8) {
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

        public TaskLogger WithFields(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9) {
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

        public TaskLogger WithFields(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10) {
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

        public TaskLogger WithFields(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11) {
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

        public TaskLogger WithFields(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, ref Field field12) {
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

        public TaskLogger WithFields(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, ref Field field12, ref Field field13) {
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

        public TaskLogger WithFields(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, ref Field field12, ref Field field13, ref Field field14) {
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

        public TaskLogger WithFields(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, ref Field field12, ref Field field13, ref Field field14, ref Field field15) {
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

        public TaskLogger WithFields(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, ref Field field12, ref Field field13, ref Field field14, ref Field field15, ref Field field16) {
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

        public TaskLogger WithFields(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, ref Field field12, ref Field field13, ref Field field14, ref Field field15, ref Field field16, ref Field field17) {
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

        public TaskLogger WithFields(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, ref Field field12, ref Field field13, ref Field field14, ref Field field15, ref Field field16, ref Field field17, ref Field field18) {
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

        public TaskLogger WithFields(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, ref Field field12, ref Field field13, ref Field field14, ref Field field15, ref Field field16, ref Field field17, ref Field field18, ref Field field19) {
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

        public TaskLogger WithFields(ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, ref Field field12, ref Field field13, ref Field field14, ref Field field15, ref Field field16, ref Field field17, ref Field field18, ref Field field19, ref Field field20) {
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
        
        public void Info(Field field1,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) {
            if (Logger.Instance.Level < LogLevel.Info) {
                return;
            }
            _write1(ref field1, LogLevel.Info, file, member, line);
        }

        private void _write1(ref Field field1, LogLevel level, string file, string member, int line) {
            var logger = ThreadLocalLogger.Current;
            using (var _ = new ScopeGuard(() => {
                logger.UnLock();
            })){
                logger.Lock();
                ref Common.RentedBuffer buf = ref logger.Buffer;
                //
                buf.Append(_prefix.Data.AsSpan<byte>(0, _prefix.Length));
                if (_prefix.Length >= minLenOfFirstTag) {
                    buf.Append((byte)',');
                }
                field1.WriteTo(ref buf);
                // todo: 陆续追加 field2 ... field20
                _writeTail(ref buf, level, file, member, line);
                logger.Flush();
            }
        }

        private void _writeTail(ref Common.RentedBuffer buf, LogLevel level, string file, string member, int line) {
            // 写入公共字段
            buf.Append((byte)',');
            Field.UtcDateTime("_time"u8, System.DateTime.Now).WriteTo(ref buf);
            buf.Append((byte)',');
            string levelStr = "";
            switch (level) {
                case LogLevel.Fatal:
                    levelStr = "fatal";
                    break;
                case LogLevel.Error:    
                    levelStr = "error";
                    break;
                case LogLevel.Warn:    
                    levelStr = "warn";
                    break;    
                case LogLevel.Info:    
                    levelStr = "info";
                    break;  
                case LogLevel.Debug:    
                    levelStr = "debug";
                    break;    
            }
            Field.String("level"u8, levelStr).WriteTo(ref buf);
            buf.Append((byte)',');
            Field.String("_file"u8, file).WriteTo(ref buf);
            buf.Append((byte)',');
            Field.String("_member"u8, member).WriteTo(ref buf);
            buf.Append((byte)',');
            Field.Int64("_line"u8, line).WriteTo(ref buf);
            buf.Append("}\n"u8);
        }
    }
}
