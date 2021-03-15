using System.Collections.Generic;

namespace System
{
    public static class StringExtensions
    {
        public static string[] SplitByCommas(this string text) => text.Split(',', StringSplitOptions.RemoveEmptyEntries);

        public static string[] SplitByNewLines(this string text) => text.Split(Environment.NewLine);

        public static T AsKeyIn<T>(this string key, IDictionary<string, T> dictionary)
            where T : class
            =>
            dictionary.TryGetValue(key, out var value) ? value : null;

        /// <summary>
        /// A shortcut for <c>string.Contains(string, StringComparison.InvariantCultureIgnoreCase)</c>. It also safely
        /// returns <see langword="false"/> if either parameters are <see langword="null"/>.
        /// </summary>
        public static bool ContainsLoose(this string text, string toFind) =>
            (text != null && toFind != null) && text.Contains(toFind, StringComparison.InvariantCultureIgnoreCase);
    }
}
