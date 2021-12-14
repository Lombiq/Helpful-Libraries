using System.Threading.Tasks;

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
        /// A shortcut for <c>string.Equals(string, StringComparison.OrdinalIgnoreCase)</c>.
        /// </summary>
        public static bool EqualsOrdinalIgnoreCase(this string text, string? value) =>
            text.Equals(value, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// A shortcut for <c>string.Contains(string, StringComparison.OrdinalIgnoreCase)</c>.
        /// </summary>
        public static bool ContainsOrdinalIgnoreCase(this string text, string value) =>
            text.Contains(value, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// A shortcut for <c>string.StartsWith(string, StringComparison.OrdinalIgnoreCase)</c>.
        /// </summary>
        public static bool StartsWithOrdinalIgnoreCase(this string text, string value) =>
            text.StartsWith(value, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// A shortcut for <c>string.EndsWith(string, StringComparison.OrdinalIgnoreCase)</c>.
        /// </summary>
        public static bool EndsWithOrdinalIgnoreCase(this string text, string value) =>
            text.EndsWith(value, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// A shortcut for <c>string.Replace(string, string, StringComparison.OrdinalIgnoreCase)</c>.
        /// </summary>
        public static string ReplaceOrdinalIgnoreCase(this string text, string oldValue, string? newValue) =>
            text.Replace(oldValue, newValue, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Returns the first string that's not <see langword="null"/> or empty, starting with <paramref name="text"/>
        /// and then the items in <paramref name="alternatives"/> sequentially. Finally <see cref="string.Empty"/> if
        /// none matched the criteria.
        /// </summary>
        public static string OrIfEmpty(this string? text, params string?[] alternatives)
        {
            if (!string.IsNullOrEmpty(text)) return text;

            foreach (var alternative in alternatives)
            {
                if (!string.IsNullOrEmpty(alternative)) return alternative;
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns <paramref name="alternative"/> if <paramref name="condition"/> is <see langword="true"/>, otherwise
        /// returns <paramref name="text"/>.
        /// </summary>
        public static string? OrIf(this string? text, Func<string?, bool> condition, string? alternative) =>
            condition(text) ? alternative : text;

        /// <summary>
        /// Returns the result of <paramref name="alternativeAsync"/> if <paramref name="condition"/> is <see
        /// langword="true"/>, otherwise returns <paramref name="text"/>. A delegate is used to avoid unnecessary
        /// expensive async calls.
        /// </summary>
        public static Task<string?> OrIfAsync(
            this string? text,
            Func<string?, bool> condition,
            Func<Task<string?>> alternativeAsync) =>
            condition(text) ? alternativeAsync() : Task.FromResult(text);
    }
}
