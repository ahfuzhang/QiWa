namespace ConsoleLogger
{
    public partial class TaskLogger
    {
        const int defaultPrefixLen = 512;
        internal Common.RentedBuffer prefix;
        internal TaskLogger()
        {
            System.Diagnostics.Debug.Assert(Logger.Instance!=null);
            prefix.Rent(Logger.Instance.TagPrefix.Length+defaultPrefixLen);
            prefix.Append(Logger.Instance.TagPrefix);
            //Console.WriteLine(System.Text.Encoding.UTF8.GetString(prefix.Bytes()));
        }

        ~TaskLogger()
        {
            prefix.Dispose();
        }
    }
}
