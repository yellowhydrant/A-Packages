using System;
using System.Text.RegularExpressions;

namespace A.Extensions
{
    public static class AStringExtensions
    {
        public static bool IsValidGuid(this string guid) => Guid.TryParse(guid, out var _);

        public static string SplitCamelCase(this string input) => Regex.Replace(input, "(?<=[a-z])([A-Z])", " $1", RegexOptions.Compiled);
    }
}