namespace FlatMate.Module.Common.Extensions
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxChars, bool addPeriods = true)
        {
            if (value.Length <= maxChars)
            {
                return value;
            }

            if (addPeriods)
            {
                return value.Substring(0, maxChars) + "...";
            }

            return value.Substring(0, maxChars);
        }
    }
}