#nullable enable

namespace System
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns an array by splitting the input along commas and stripping empty entries.
        /// </summary>
        public static string[] SplitByCommas(this string? text) =>
            text?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

        /// <summary>
        /// Returns the input split into lines (using <see cref="Environment.NewLine"/>).
        /// </summary>
        public static string[] SplitByNewLines(this string? text) =>
            text?.Split(Environment.NewLine) ?? Array.Empty<string>();

        /// <summary>
        /// A shortcut for <c>string.Contains(string, StringComparison.InvariantCultureIgnoreCase)</c>. It also safely
        /// returns <see langword="false"/> if either parameters are <see langword="null"/>.
        /// </summary>
        public static bool ContainsLoose(this string? text, string? toFind) =>
            text != null && toFind != null && text.Contains(toFind, StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        /// A shortcut for <c>string.Contains(string, StringComparison.OrdinalIgnoreCase)</c>.
        /// </summary>
        public static bool ContainsOrdinalIgnoreCase(this string? text, string? value) =>
            text != null && !string.IsNullOrEmpty(value) && text.Contains(value, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// A shortcut for <c>string.StartsWith(string, StringComparison.OrdinalIgnoreCase)</c>.
        /// </summary>
        public static bool StartsWithOrdinalIgnoreCase(this string? text, string? value) =>
            text != null && !string.IsNullOrEmpty(value) && text.StartsWith(value, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// A shortcut for <c>string.Equals(string, StringComparison.OrdinalIgnoreCase)</c>.
        /// </summary>
        public static bool EqualsOrdinalIgnoreCase(this string? text, string? value) =>
            text != null && !string.IsNullOrEmpty(value) && text.Equals(value, StringComparison.OrdinalIgnoreCase);
    }
}
