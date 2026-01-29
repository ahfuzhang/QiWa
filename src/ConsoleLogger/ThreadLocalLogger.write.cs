namespace ConsoleLogger
{
    public partial class ThreadLocalLogger
    {
        internal void write1(ReadOnlySpan<byte> prefix, ref Field field1, string levelStr, string file, string member, int line)
        {
            lock (locker)
            {
                ref Common.RentedBuffer rented = ref GetBuffer();
                if (prefix.Length == 0)
                {
                    rented.Append((byte)'{');
                } else
                {
                    rented.Append(prefix);
                    rented.Append((byte)',');
                }
                // todo: 陆续追加 field2 ... field20
                field1.WriteTo(ref rented);
                writeTail(ref rented, levelStr, file, member, line);
                Flush(ref rented);
            }
        }

        internal void write2(ReadOnlySpan<byte> prefix, ref Field field1, ref Field field2, string levelStr, string file, string member, int line)
        {
            lock (locker)
            {
                ref Common.RentedBuffer rented = ref GetBuffer();
                if (prefix.Length == 0)
                {
                    rented.Append((byte)'{');
                } else
                {
                    rented.Append(prefix);
                    rented.Append((byte)',');
                }
                field1.WriteTo(ref rented);
                rented.Append((byte)',');
                field2.WriteTo(ref rented);
                writeTail(ref rented, levelStr, file, member, line);
                Flush(ref rented);
            }
        }

        internal void write3(ReadOnlySpan<byte> prefix, ref Field field1, ref Field field2, ref Field field3, string levelStr, string file, string member, int line)
        {
            lock (locker)
            {
                ref Common.RentedBuffer rented = ref GetBuffer();
                if (prefix.Length == 0)
                {
                    rented.Append((byte)'{');
                } else
                {
                    rented.Append(prefix);
                    rented.Append((byte)',');
                }
                field1.WriteTo(ref rented);
                rented.Append((byte)',');
                field2.WriteTo(ref rented);
                rented.Append((byte)',');
                field3.WriteTo(ref rented);
                writeTail(ref rented, levelStr, file, member, line);
                Flush(ref rented);
            }
        }

        internal void write4(ReadOnlySpan<byte> prefix, ref Field field1, ref Field field2, ref Field field3, ref Field field4, string levelStr, string file, string member, int line)
        {
            lock (locker)
            {
                ref Common.RentedBuffer rented = ref GetBuffer();
                if (prefix.Length == 0)
                {
                    rented.Append((byte)'{');
                } else
                {
                    rented.Append(prefix);
                    rented.Append((byte)',');
                }
                field1.WriteTo(ref rented);
                rented.Append((byte)',');
                field2.WriteTo(ref rented);
                rented.Append((byte)',');
                field3.WriteTo(ref rented);
                rented.Append((byte)',');
                field4.WriteTo(ref rented);
                writeTail(ref rented, levelStr, file, member, line);
                Flush(ref rented);
            }
        }

        internal void write5(ReadOnlySpan<byte> prefix, ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, string levelStr, string file, string member, int line)
        {
            lock (locker)
            {
                ref Common.RentedBuffer rented = ref GetBuffer();
                if (prefix.Length == 0)
                {
                    rented.Append((byte)'{');
                } else
                {
                    rented.Append(prefix);
                    rented.Append((byte)',');
                }
                field1.WriteTo(ref rented);
                rented.Append((byte)',');
                field2.WriteTo(ref rented);
                rented.Append((byte)',');
                field3.WriteTo(ref rented);
                rented.Append((byte)',');
                field4.WriteTo(ref rented);
                rented.Append((byte)',');
                field5.WriteTo(ref rented);
                writeTail(ref rented, levelStr, file, member, line);
                Flush(ref rented);
            }
        }

        internal void write6(ReadOnlySpan<byte> prefix, ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, string levelStr, string file, string member, int line)
        {
            lock (locker)
            {
                ref Common.RentedBuffer rented = ref GetBuffer();
                if (prefix.Length == 0)
                {
                    rented.Append((byte)'{');
                } else
                {
                    rented.Append(prefix);
                    rented.Append((byte)',');
                }
                field1.WriteTo(ref rented);
                rented.Append((byte)',');
                field2.WriteTo(ref rented);
                rented.Append((byte)',');
                field3.WriteTo(ref rented);
                rented.Append((byte)',');
                field4.WriteTo(ref rented);
                rented.Append((byte)',');
                field5.WriteTo(ref rented);
                rented.Append((byte)',');
                field6.WriteTo(ref rented);
                writeTail(ref rented, levelStr, file, member, line);
                Flush(ref rented);
            }
        }

        internal void write7(ReadOnlySpan<byte> prefix, ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, string levelStr, string file, string member, int line)
        {
            lock (locker)
            {
                ref Common.RentedBuffer rented = ref GetBuffer();
                if (prefix.Length == 0)
                {
                    rented.Append((byte)'{');
                } else
                {
                    rented.Append(prefix);
                    rented.Append((byte)',');
                }
                field1.WriteTo(ref rented);
                rented.Append((byte)',');
                field2.WriteTo(ref rented);
                rented.Append((byte)',');
                field3.WriteTo(ref rented);
                rented.Append((byte)',');
                field4.WriteTo(ref rented);
                rented.Append((byte)',');
                field5.WriteTo(ref rented);
                rented.Append((byte)',');
                field6.WriteTo(ref rented);
                rented.Append((byte)',');
                field7.WriteTo(ref rented);
                writeTail(ref rented, levelStr, file, member, line);
                Flush(ref rented);
            }
        }

        internal void write8(ReadOnlySpan<byte> prefix, ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, string levelStr, string file, string member, int line)
        {
            lock (locker)
            {
                ref Common.RentedBuffer rented = ref GetBuffer();
                if (prefix.Length == 0)
                {
                    rented.Append((byte)'{');
                } else
                {
                    rented.Append(prefix);
                    rented.Append((byte)',');
                }
                field1.WriteTo(ref rented);
                rented.Append((byte)',');
                field2.WriteTo(ref rented);
                rented.Append((byte)',');
                field3.WriteTo(ref rented);
                rented.Append((byte)',');
                field4.WriteTo(ref rented);
                rented.Append((byte)',');
                field5.WriteTo(ref rented);
                rented.Append((byte)',');
                field6.WriteTo(ref rented);
                rented.Append((byte)',');
                field7.WriteTo(ref rented);
                rented.Append((byte)',');
                field8.WriteTo(ref rented);
                writeTail(ref rented, levelStr, file, member, line);
                Flush(ref rented);
            }
        }

        internal void write9(ReadOnlySpan<byte> prefix, ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, string levelStr, string file, string member, int line)
        {
            lock (locker)
            {
                ref Common.RentedBuffer rented = ref GetBuffer();
                if (prefix.Length == 0)
                {
                    rented.Append((byte)'{');
                } else
                {
                    rented.Append(prefix);
                    rented.Append((byte)',');
                }
                field1.WriteTo(ref rented);
                rented.Append((byte)',');
                field2.WriteTo(ref rented);
                rented.Append((byte)',');
                field3.WriteTo(ref rented);
                rented.Append((byte)',');
                field4.WriteTo(ref rented);
                rented.Append((byte)',');
                field5.WriteTo(ref rented);
                rented.Append((byte)',');
                field6.WriteTo(ref rented);
                rented.Append((byte)',');
                field7.WriteTo(ref rented);
                rented.Append((byte)',');
                field8.WriteTo(ref rented);
                rented.Append((byte)',');
                field9.WriteTo(ref rented);
                writeTail(ref rented, levelStr, file, member, line);
                Flush(ref rented);
            }
        }

        internal void write10(ReadOnlySpan<byte> prefix, ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, string levelStr, string file, string member, int line)
        {
            lock (locker)
            {
                ref Common.RentedBuffer rented = ref GetBuffer();
                if (prefix.Length == 0)
                {
                    rented.Append((byte)'{');
                } else
                {
                    rented.Append(prefix);
                    rented.Append((byte)',');
                }
                field1.WriteTo(ref rented);
                rented.Append((byte)',');
                field2.WriteTo(ref rented);
                rented.Append((byte)',');
                field3.WriteTo(ref rented);
                rented.Append((byte)',');
                field4.WriteTo(ref rented);
                rented.Append((byte)',');
                field5.WriteTo(ref rented);
                rented.Append((byte)',');
                field6.WriteTo(ref rented);
                rented.Append((byte)',');
                field7.WriteTo(ref rented);
                rented.Append((byte)',');
                field8.WriteTo(ref rented);
                rented.Append((byte)',');
                field9.WriteTo(ref rented);
                rented.Append((byte)',');
                field10.WriteTo(ref rented);
                writeTail(ref rented, levelStr, file, member, line);
                Flush(ref rented);
            }
        }

        internal void write11(ReadOnlySpan<byte> prefix, ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, string levelStr, string file, string member, int line)
        {
            lock (locker)
            {
                ref Common.RentedBuffer rented = ref GetBuffer();
                if (prefix.Length == 0)
                {
                    rented.Append((byte)'{');
                } else
                {
                    rented.Append(prefix);
                    rented.Append((byte)',');
                }
                field1.WriteTo(ref rented);
                rented.Append((byte)',');
                field2.WriteTo(ref rented);
                rented.Append((byte)',');
                field3.WriteTo(ref rented);
                rented.Append((byte)',');
                field4.WriteTo(ref rented);
                rented.Append((byte)',');
                field5.WriteTo(ref rented);
                rented.Append((byte)',');
                field6.WriteTo(ref rented);
                rented.Append((byte)',');
                field7.WriteTo(ref rented);
                rented.Append((byte)',');
                field8.WriteTo(ref rented);
                rented.Append((byte)',');
                field9.WriteTo(ref rented);
                rented.Append((byte)',');
                field10.WriteTo(ref rented);
                rented.Append((byte)',');
                field11.WriteTo(ref rented);
                writeTail(ref rented, levelStr, file, member, line);
                Flush(ref rented);
            }
        }

        internal void write12(ReadOnlySpan<byte> prefix, ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, ref Field field12, string levelStr, string file, string member, int line)
        {
            lock (locker)
            {
                ref Common.RentedBuffer rented = ref GetBuffer();
                if (prefix.Length == 0)
                {
                    rented.Append((byte)'{');
                } else
                {
                    rented.Append(prefix);
                    rented.Append((byte)',');
                }
                field1.WriteTo(ref rented);
                rented.Append((byte)',');
                field2.WriteTo(ref rented);
                rented.Append((byte)',');
                field3.WriteTo(ref rented);
                rented.Append((byte)',');
                field4.WriteTo(ref rented);
                rented.Append((byte)',');
                field5.WriteTo(ref rented);
                rented.Append((byte)',');
                field6.WriteTo(ref rented);
                rented.Append((byte)',');
                field7.WriteTo(ref rented);
                rented.Append((byte)',');
                field8.WriteTo(ref rented);
                rented.Append((byte)',');
                field9.WriteTo(ref rented);
                rented.Append((byte)',');
                field10.WriteTo(ref rented);
                rented.Append((byte)',');
                field11.WriteTo(ref rented);
                rented.Append((byte)',');
                field12.WriteTo(ref rented);
                writeTail(ref rented, levelStr, file, member, line);
                Flush(ref rented);
            }
        }

        internal void write13(ReadOnlySpan<byte> prefix, ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, ref Field field12, ref Field field13, string levelStr, string file, string member, int line)
        {
            lock (locker)
            {
                ref Common.RentedBuffer rented = ref GetBuffer();
                if (prefix.Length == 0)
                {
                    rented.Append((byte)'{');
                } else
                {
                    rented.Append(prefix);
                    rented.Append((byte)',');
                }
                field1.WriteTo(ref rented);
                rented.Append((byte)',');
                field2.WriteTo(ref rented);
                rented.Append((byte)',');
                field3.WriteTo(ref rented);
                rented.Append((byte)',');
                field4.WriteTo(ref rented);
                rented.Append((byte)',');
                field5.WriteTo(ref rented);
                rented.Append((byte)',');
                field6.WriteTo(ref rented);
                rented.Append((byte)',');
                field7.WriteTo(ref rented);
                rented.Append((byte)',');
                field8.WriteTo(ref rented);
                rented.Append((byte)',');
                field9.WriteTo(ref rented);
                rented.Append((byte)',');
                field10.WriteTo(ref rented);
                rented.Append((byte)',');
                field11.WriteTo(ref rented);
                rented.Append((byte)',');
                field12.WriteTo(ref rented);
                rented.Append((byte)',');
                field13.WriteTo(ref rented);
                writeTail(ref rented, levelStr, file, member, line);
                Flush(ref rented);
            }
        }

        internal void write14(ReadOnlySpan<byte> prefix, ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, ref Field field12, ref Field field13, ref Field field14, string levelStr, string file, string member, int line)
        {
            lock (locker)
            {
                ref Common.RentedBuffer rented = ref GetBuffer();
                if (prefix.Length == 0)
                {
                    rented.Append((byte)'{');
                } else
                {
                    rented.Append(prefix);
                    rented.Append((byte)',');
                }
                field1.WriteTo(ref rented);
                rented.Append((byte)',');
                field2.WriteTo(ref rented);
                rented.Append((byte)',');
                field3.WriteTo(ref rented);
                rented.Append((byte)',');
                field4.WriteTo(ref rented);
                rented.Append((byte)',');
                field5.WriteTo(ref rented);
                rented.Append((byte)',');
                field6.WriteTo(ref rented);
                rented.Append((byte)',');
                field7.WriteTo(ref rented);
                rented.Append((byte)',');
                field8.WriteTo(ref rented);
                rented.Append((byte)',');
                field9.WriteTo(ref rented);
                rented.Append((byte)',');
                field10.WriteTo(ref rented);
                rented.Append((byte)',');
                field11.WriteTo(ref rented);
                rented.Append((byte)',');
                field12.WriteTo(ref rented);
                rented.Append((byte)',');
                field13.WriteTo(ref rented);
                rented.Append((byte)',');
                field14.WriteTo(ref rented);
                writeTail(ref rented, levelStr, file, member, line);
                Flush(ref rented);
            }
        }

        internal void write15(ReadOnlySpan<byte> prefix, ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, ref Field field12, ref Field field13, ref Field field14, ref Field field15, string levelStr, string file, string member, int line)
        {
            lock (locker)
            {
                ref Common.RentedBuffer rented = ref GetBuffer();
                if (prefix.Length == 0)
                {
                    rented.Append((byte)'{');
                } else
                {
                    rented.Append(prefix);
                    rented.Append((byte)',');
                }
                field1.WriteTo(ref rented);
                rented.Append((byte)',');
                field2.WriteTo(ref rented);
                rented.Append((byte)',');
                field3.WriteTo(ref rented);
                rented.Append((byte)',');
                field4.WriteTo(ref rented);
                rented.Append((byte)',');
                field5.WriteTo(ref rented);
                rented.Append((byte)',');
                field6.WriteTo(ref rented);
                rented.Append((byte)',');
                field7.WriteTo(ref rented);
                rented.Append((byte)',');
                field8.WriteTo(ref rented);
                rented.Append((byte)',');
                field9.WriteTo(ref rented);
                rented.Append((byte)',');
                field10.WriteTo(ref rented);
                rented.Append((byte)',');
                field11.WriteTo(ref rented);
                rented.Append((byte)',');
                field12.WriteTo(ref rented);
                rented.Append((byte)',');
                field13.WriteTo(ref rented);
                rented.Append((byte)',');
                field14.WriteTo(ref rented);
                rented.Append((byte)',');
                field15.WriteTo(ref rented);
                writeTail(ref rented, levelStr, file, member, line);
                Flush(ref rented);
            }
        }

        internal void write16(ReadOnlySpan<byte> prefix, ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, ref Field field12, ref Field field13, ref Field field14, ref Field field15, ref Field field16, string levelStr, string file, string member, int line)
        {
            lock (locker)
            {
                ref Common.RentedBuffer rented = ref GetBuffer();
                if (prefix.Length == 0)
                {
                    rented.Append((byte)'{');
                } else
                {
                    rented.Append(prefix);
                    rented.Append((byte)',');
                }
                field1.WriteTo(ref rented);
                rented.Append((byte)',');
                field2.WriteTo(ref rented);
                rented.Append((byte)',');
                field3.WriteTo(ref rented);
                rented.Append((byte)',');
                field4.WriteTo(ref rented);
                rented.Append((byte)',');
                field5.WriteTo(ref rented);
                rented.Append((byte)',');
                field6.WriteTo(ref rented);
                rented.Append((byte)',');
                field7.WriteTo(ref rented);
                rented.Append((byte)',');
                field8.WriteTo(ref rented);
                rented.Append((byte)',');
                field9.WriteTo(ref rented);
                rented.Append((byte)',');
                field10.WriteTo(ref rented);
                rented.Append((byte)',');
                field11.WriteTo(ref rented);
                rented.Append((byte)',');
                field12.WriteTo(ref rented);
                rented.Append((byte)',');
                field13.WriteTo(ref rented);
                rented.Append((byte)',');
                field14.WriteTo(ref rented);
                rented.Append((byte)',');
                field15.WriteTo(ref rented);
                rented.Append((byte)',');
                field16.WriteTo(ref rented);
                writeTail(ref rented, levelStr, file, member, line);
                Flush(ref rented);
            }
        }

        internal void write17(ReadOnlySpan<byte> prefix, ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, ref Field field12, ref Field field13, ref Field field14, ref Field field15, ref Field field16, ref Field field17, string levelStr, string file, string member, int line)
        {
            lock (locker)
            {
                ref Common.RentedBuffer rented = ref GetBuffer();
                if (prefix.Length == 0)
                {
                    rented.Append((byte)'{');
                } else
                {
                    rented.Append(prefix);
                    rented.Append((byte)',');
                }
                field1.WriteTo(ref rented);
                rented.Append((byte)',');
                field2.WriteTo(ref rented);
                rented.Append((byte)',');
                field3.WriteTo(ref rented);
                rented.Append((byte)',');
                field4.WriteTo(ref rented);
                rented.Append((byte)',');
                field5.WriteTo(ref rented);
                rented.Append((byte)',');
                field6.WriteTo(ref rented);
                rented.Append((byte)',');
                field7.WriteTo(ref rented);
                rented.Append((byte)',');
                field8.WriteTo(ref rented);
                rented.Append((byte)',');
                field9.WriteTo(ref rented);
                rented.Append((byte)',');
                field10.WriteTo(ref rented);
                rented.Append((byte)',');
                field11.WriteTo(ref rented);
                rented.Append((byte)',');
                field12.WriteTo(ref rented);
                rented.Append((byte)',');
                field13.WriteTo(ref rented);
                rented.Append((byte)',');
                field14.WriteTo(ref rented);
                rented.Append((byte)',');
                field15.WriteTo(ref rented);
                rented.Append((byte)',');
                field16.WriteTo(ref rented);
                rented.Append((byte)',');
                field17.WriteTo(ref rented);
                writeTail(ref rented, levelStr, file, member, line);
                Flush(ref rented);
            }
        }

        internal void write18(ReadOnlySpan<byte> prefix, ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, ref Field field12, ref Field field13, ref Field field14, ref Field field15, ref Field field16, ref Field field17, ref Field field18, string levelStr, string file, string member, int line)
        {
            lock (locker)
            {
                ref Common.RentedBuffer rented = ref GetBuffer();
                if (prefix.Length == 0)
                {
                    rented.Append((byte)'{');
                } else
                {
                    rented.Append(prefix);
                    rented.Append((byte)',');
                }
                field1.WriteTo(ref rented);
                rented.Append((byte)',');
                field2.WriteTo(ref rented);
                rented.Append((byte)',');
                field3.WriteTo(ref rented);
                rented.Append((byte)',');
                field4.WriteTo(ref rented);
                rented.Append((byte)',');
                field5.WriteTo(ref rented);
                rented.Append((byte)',');
                field6.WriteTo(ref rented);
                rented.Append((byte)',');
                field7.WriteTo(ref rented);
                rented.Append((byte)',');
                field8.WriteTo(ref rented);
                rented.Append((byte)',');
                field9.WriteTo(ref rented);
                rented.Append((byte)',');
                field10.WriteTo(ref rented);
                rented.Append((byte)',');
                field11.WriteTo(ref rented);
                rented.Append((byte)',');
                field12.WriteTo(ref rented);
                rented.Append((byte)',');
                field13.WriteTo(ref rented);
                rented.Append((byte)',');
                field14.WriteTo(ref rented);
                rented.Append((byte)',');
                field15.WriteTo(ref rented);
                rented.Append((byte)',');
                field16.WriteTo(ref rented);
                rented.Append((byte)',');
                field17.WriteTo(ref rented);
                rented.Append((byte)',');
                field18.WriteTo(ref rented);
                writeTail(ref rented, levelStr, file, member, line);
                Flush(ref rented);
            }
        }

        internal void write19(ReadOnlySpan<byte> prefix, ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, ref Field field12, ref Field field13, ref Field field14, ref Field field15, ref Field field16, ref Field field17, ref Field field18, ref Field field19, string levelStr, string file, string member, int line)
        {
            lock (locker)
            {
                ref Common.RentedBuffer rented = ref GetBuffer();
                if (prefix.Length == 0)
                {
                    rented.Append((byte)'{');
                } else
                {
                    rented.Append(prefix);
                    rented.Append((byte)',');
                }
                field1.WriteTo(ref rented);
                rented.Append((byte)',');
                field2.WriteTo(ref rented);
                rented.Append((byte)',');
                field3.WriteTo(ref rented);
                rented.Append((byte)',');
                field4.WriteTo(ref rented);
                rented.Append((byte)',');
                field5.WriteTo(ref rented);
                rented.Append((byte)',');
                field6.WriteTo(ref rented);
                rented.Append((byte)',');
                field7.WriteTo(ref rented);
                rented.Append((byte)',');
                field8.WriteTo(ref rented);
                rented.Append((byte)',');
                field9.WriteTo(ref rented);
                rented.Append((byte)',');
                field10.WriteTo(ref rented);
                rented.Append((byte)',');
                field11.WriteTo(ref rented);
                rented.Append((byte)',');
                field12.WriteTo(ref rented);
                rented.Append((byte)',');
                field13.WriteTo(ref rented);
                rented.Append((byte)',');
                field14.WriteTo(ref rented);
                rented.Append((byte)',');
                field15.WriteTo(ref rented);
                rented.Append((byte)',');
                field16.WriteTo(ref rented);
                rented.Append((byte)',');
                field17.WriteTo(ref rented);
                rented.Append((byte)',');
                field18.WriteTo(ref rented);
                rented.Append((byte)',');
                field19.WriteTo(ref rented);
                writeTail(ref rented, levelStr, file, member, line);
                Flush(ref rented);
            }
        }

        internal void write20(ReadOnlySpan<byte> prefix, ref Field field1, ref Field field2, ref Field field3, ref Field field4, ref Field field5, ref Field field6, ref Field field7, ref Field field8, ref Field field9, ref Field field10, ref Field field11, ref Field field12, ref Field field13, ref Field field14, ref Field field15, ref Field field16, ref Field field17, ref Field field18, ref Field field19, ref Field field20, string levelStr, string file, string member, int line)
        {
            lock (locker)
            {
                ref Common.RentedBuffer rented = ref GetBuffer();
                if (prefix.Length == 0)
                {
                    rented.Append((byte)'{');
                } else
                {
                    rented.Append(prefix);
                    rented.Append((byte)',');
                }
                field1.WriteTo(ref rented);
                rented.Append((byte)',');
                field2.WriteTo(ref rented);
                rented.Append((byte)',');
                field3.WriteTo(ref rented);
                rented.Append((byte)',');
                field4.WriteTo(ref rented);
                rented.Append((byte)',');
                field5.WriteTo(ref rented);
                rented.Append((byte)',');
                field6.WriteTo(ref rented);
                rented.Append((byte)',');
                field7.WriteTo(ref rented);
                rented.Append((byte)',');
                field8.WriteTo(ref rented);
                rented.Append((byte)',');
                field9.WriteTo(ref rented);
                rented.Append((byte)',');
                field10.WriteTo(ref rented);
                rented.Append((byte)',');
                field11.WriteTo(ref rented);
                rented.Append((byte)',');
                field12.WriteTo(ref rented);
                rented.Append((byte)',');
                field13.WriteTo(ref rented);
                rented.Append((byte)',');
                field14.WriteTo(ref rented);
                rented.Append((byte)',');
                field15.WriteTo(ref rented);
                rented.Append((byte)',');
                field16.WriteTo(ref rented);
                rented.Append((byte)',');
                field17.WriteTo(ref rented);
                rented.Append((byte)',');
                field18.WriteTo(ref rented);
                rented.Append((byte)',');
                field19.WriteTo(ref rented);
                rented.Append((byte)',');
                field20.WriteTo(ref rented);
                writeTail(ref rented, levelStr, file, member, line);
                Flush(ref rented);
            }
        }

        internal void writeTail(ref Common.RentedBuffer buf, string levelStr, string file, string member, int line) {
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
