using System.Text.RegularExpressions;

namespace cachCore
{
    public static class Extensions
    {
        public static bool Matches(this string src, string pattern)
        {
            return Regex.IsMatch(src.Trim(), pattern);
        }

        public static bool Matches(this string src, string pattern, RegexOptions options)
        {
            return Regex.IsMatch(src.Trim(), pattern, options);
        }
    }
}
