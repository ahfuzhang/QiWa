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
        }

        ~TaskLogger()
        {
            prefix.Dispose();
        }
    }
}
