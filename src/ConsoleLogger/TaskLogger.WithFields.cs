namespace ConsoleLogger;

// 当需要继承上一个 context 中的信息时，通过 WithField 来产生新的 TaskLogger 对象
// 类似于 golang 中的 ctx = context.WithValue(ctx, ...)

public partial class TaskLogger
{
    public TaskLogger WithFields(Field field1)
    {
        TaskLogger l = Logger.Get();
        l.prefix.Append(prefix.Bytes());
        if (l.prefix.Length == 0)
        {
            l.prefix.Append((byte)'{');
        }
        else
        {
            l.prefix.Append((byte)',');
        }
        field1.WriteTo(ref l.prefix);
        return l;
    }

    public TaskLogger WithFields(Field field1, Field field2)
    {
        TaskLogger l = Logger.Get();
        l.prefix.Append(prefix.Bytes());
        if (l.prefix.Length == 0)
        {
            l.prefix.Append((byte)'{');
        }
        else
        {
            l.prefix.Append((byte)',');
        }
        field1.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field2.WriteTo(ref l.prefix);
        return l;
    }

    public TaskLogger WithFields(Field field1, Field field2, Field field3)
    {
        TaskLogger l = Logger.Get();
        l.prefix.Append(prefix.Bytes());
        if (l.prefix.Length == 0)
        {
            l.prefix.Append((byte)'{');
        }
        else
        {
            l.prefix.Append((byte)',');
        }
        field1.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field2.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field3.WriteTo(ref l.prefix);
        return l;
    }

    public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4)
    {
        TaskLogger l = Logger.Get();
        l.prefix.Append(prefix.Bytes());
        if (l.prefix.Length == 0)
        {
            l.prefix.Append((byte)'{');
        }
        else
        {
            l.prefix.Append((byte)',');
        }
        field1.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field2.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field3.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field4.WriteTo(ref l.prefix);
        return l;
    }

    public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5)
    {
        TaskLogger l = Logger.Get();
        l.prefix.Append(prefix.Bytes());
        if (l.prefix.Length == 0)
        {
            l.prefix.Append((byte)'{');
        }
        else
        {
            l.prefix.Append((byte)',');
        }
        field1.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field2.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field3.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field4.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field5.WriteTo(ref l.prefix);
        return l;
    }

    public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6)
    {
        TaskLogger l = Logger.Get();
        l.prefix.Append(prefix.Bytes());
        if (l.prefix.Length == 0)
        {
            l.prefix.Append((byte)'{');
        }
        else
        {
            l.prefix.Append((byte)',');
        }
        field1.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field2.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field3.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field4.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field5.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field6.WriteTo(ref l.prefix);
        return l;
    }

    public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7)
    {
        TaskLogger l = Logger.Get();
        l.prefix.Append(prefix.Bytes());
        if (l.prefix.Length == 0)
        {
            l.prefix.Append((byte)'{');
        }
        else
        {
            l.prefix.Append((byte)',');
        }
        field1.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field2.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field3.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field4.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field5.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field6.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field7.WriteTo(ref l.prefix);
        return l;
    }

    public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8)
    {
        TaskLogger l = Logger.Get();
        l.prefix.Append(prefix.Bytes());
        if (l.prefix.Length == 0)
        {
            l.prefix.Append((byte)'{');
        }
        else
        {
            l.prefix.Append((byte)',');
        }
        field1.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field2.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field3.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field4.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field5.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field6.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field7.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field8.WriteTo(ref l.prefix);
        return l;
    }

    public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9)
    {
        TaskLogger l = Logger.Get();
        l.prefix.Append(prefix.Bytes());
        if (l.prefix.Length == 0)
        {
            l.prefix.Append((byte)'{');
        }
        else
        {
            l.prefix.Append((byte)',');
        }
        field1.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field2.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field3.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field4.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field5.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field6.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field7.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field8.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field9.WriteTo(ref l.prefix);
        return l;
    }

    public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10)
    {
        TaskLogger l = Logger.Get();
        l.prefix.Append(prefix.Bytes());
        if (l.prefix.Length == 0)
        {
            l.prefix.Append((byte)'{');
        }
        else
        {
            l.prefix.Append((byte)',');
        }
        field1.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field2.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field3.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field4.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field5.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field6.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field7.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field8.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field9.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field10.WriteTo(ref l.prefix);
        return l;
    }

    public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11)
    {
        TaskLogger l = Logger.Get();
        l.prefix.Append(prefix.Bytes());
        if (l.prefix.Length == 0)
        {
            l.prefix.Append((byte)'{');
        }
        else
        {
            l.prefix.Append((byte)',');
        }
        field1.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field2.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field3.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field4.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field5.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field6.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field7.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field8.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field9.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field10.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field11.WriteTo(ref l.prefix);
        return l;
    }

    public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12)
    {
        TaskLogger l = Logger.Get();
        l.prefix.Append(prefix.Bytes());
        if (l.prefix.Length == 0)
        {
            l.prefix.Append((byte)'{');
        }
        else
        {
            l.prefix.Append((byte)',');
        }
        field1.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field2.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field3.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field4.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field5.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field6.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field7.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field8.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field9.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field10.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field11.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field12.WriteTo(ref l.prefix);
        return l;
    }

    public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13)
    {
        TaskLogger l = Logger.Get();
        l.prefix.Append(prefix.Bytes());
        if (l.prefix.Length == 0)
        {
            l.prefix.Append((byte)'{');
        }
        else
        {
            l.prefix.Append((byte)',');
        }
        field1.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field2.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field3.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field4.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field5.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field6.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field7.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field8.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field9.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field10.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field11.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field12.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field13.WriteTo(ref l.prefix);
        return l;
    }

    public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14)
    {
        TaskLogger l = Logger.Get();
        l.prefix.Append(prefix.Bytes());
        if (l.prefix.Length == 0)
        {
            l.prefix.Append((byte)'{');
        }
        else
        {
            l.prefix.Append((byte)',');
        }
        field1.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field2.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field3.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field4.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field5.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field6.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field7.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field8.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field9.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field10.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field11.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field12.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field13.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field14.WriteTo(ref l.prefix);
        return l;
    }

    public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15)
    {
        TaskLogger l = Logger.Get();
        l.prefix.Append(prefix.Bytes());
        if (l.prefix.Length == 0)
        {
            l.prefix.Append((byte)'{');
        }
        else
        {
            l.prefix.Append((byte)',');
        }
        field1.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field2.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field3.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field4.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field5.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field6.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field7.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field8.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field9.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field10.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field11.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field12.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field13.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field14.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field15.WriteTo(ref l.prefix);
        return l;
    }

    public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16)
    {
        TaskLogger l = Logger.Get();
        l.prefix.Append(prefix.Bytes());
        if (l.prefix.Length == 0)
        {
            l.prefix.Append((byte)'{');
        }
        else
        {
            l.prefix.Append((byte)',');
        }
        field1.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field2.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field3.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field4.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field5.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field6.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field7.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field8.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field9.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field10.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field11.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field12.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field13.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field14.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field15.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field16.WriteTo(ref l.prefix);
        return l;
    }

    public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17)
    {
        TaskLogger l = Logger.Get();
        l.prefix.Append(prefix.Bytes());
        if (l.prefix.Length == 0)
        {
            l.prefix.Append((byte)'{');
        }
        else
        {
            l.prefix.Append((byte)',');
        }
        field1.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field2.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field3.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field4.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field5.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field6.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field7.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field8.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field9.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field10.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field11.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field12.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field13.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field14.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field15.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field16.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field17.WriteTo(ref l.prefix);
        return l;
    }

    public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18)
    {
        TaskLogger l = Logger.Get();
        l.prefix.Append(prefix.Bytes());
        if (l.prefix.Length == 0)
        {
            l.prefix.Append((byte)'{');
        }
        else
        {
            l.prefix.Append((byte)',');
        }
        field1.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field2.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field3.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field4.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field5.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field6.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field7.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field8.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field9.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field10.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field11.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field12.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field13.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field14.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field15.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field16.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field17.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field18.WriteTo(ref l.prefix);
        return l;
    }

    public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18, Field field19)
    {
        TaskLogger l = Logger.Get();
        l.prefix.Append(prefix.Bytes());
        if (l.prefix.Length == 0)
        {
            l.prefix.Append((byte)'{');
        }
        else
        {
            l.prefix.Append((byte)',');
        }
        field1.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field2.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field3.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field4.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field5.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field6.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field7.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field8.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field9.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field10.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field11.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field12.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field13.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field14.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field15.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field16.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field17.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field18.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field19.WriteTo(ref l.prefix);
        return l;
    }

    public TaskLogger WithFields(Field field1, Field field2, Field field3, Field field4, Field field5, Field field6, Field field7, Field field8, Field field9, Field field10, Field field11, Field field12, Field field13, Field field14, Field field15, Field field16, Field field17, Field field18, Field field19, Field field20)
    {
        TaskLogger l = Logger.Get();
        l.prefix.Append(prefix.Bytes());
        if (l.prefix.Length == 0)
        {
            l.prefix.Append((byte)'{');
        }
        else
        {
            l.prefix.Append((byte)',');
        }
        field1.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field2.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field3.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field4.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field5.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field6.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field7.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field8.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field9.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field10.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field11.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field12.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field13.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field14.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field15.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field16.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field17.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field18.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field19.WriteTo(ref l.prefix);
        l.prefix.Append((byte)',');
        field20.WriteTo(ref l.prefix);
        return l;
    }
}
