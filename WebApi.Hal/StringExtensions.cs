using System;

namespace WebApi.Hal
{
    [Obsolete("Will be removed in a future release", error: false)]
    public static class StringExtensions
    {
        [Obsolete("Use string.Replace(string, string?, StringComparison) instead", error: false)]
        public static string Replace(this string str, string oldValue, string newValue, StringComparison comparison)
        {
            return str.Replace(oldValue, newValue, comparison);
        } 
    }
}