namespace Mcce.SmartOffice.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool HasValue(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public static int ToInt(this string value)
        {
            return int.TryParse(value, out int result) ? result : 0;
        }
    }
}
