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
    }
}
