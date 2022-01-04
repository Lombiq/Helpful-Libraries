using System.Collections.Generic;
using System.Linq;

namespace System
{
    /// <summary>
    /// This class provides useful methods for string usage and manipulation, usually safe to use with empty and <see
    /// langword="null"/> strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Syntactic sugar to avoid <c>if</c> statements, especially in Views.
        /// </summary>
        /// <remarks><para>Usage example: <c>@("my-class".If(someCondition))</c>.</para></remarks>
        /// <param name="ifTrue">The string to return in case the given condition is <see langword="true"/>.</param>
        /// <param name="condition">The condition to check.</param>
        /// <returns>
        /// The string <paramref name="ifTrue"/> if <paramref name="condition"/> is <see langword="true"/>; else <see
        /// langword="null"/>.
        /// </returns>
        public static string If(this string ifTrue, bool condition) => condition ? ifTrue : null;

        /// <summary>
        /// Join strings fluently.
        /// </summary>
        /// <param name="values">The <see cref="string"/> values to join.</param>
        /// <param name="separator">The separator to use between the <paramref name="values"/>.</param>
        /// <returns>A new <see cref="string"/> that concatenates all values with the <paramref name="separator"/>
        /// provided.</returns>
        public static string Join(this IEnumerable<string> values, string separator = "") =>
            string.Join(separator, values ?? Enumerable.Empty<string>());

        /// <summary>
        /// Join strings fluently.
        /// </summary>
        /// <param name="values">The <see cref="string"/> values to join.</param>
        /// <param name="separator">The separator to use between the <paramref name="values"/>.</param>
        /// <returns>A new <see cref="string"/> that concatenates all values with the <paramref name="separator"/>
        /// provided.</returns>
        public static string JoinNonEmpty(this IEnumerable<string> values, string separator = "") =>
            values?.Where(s => !string.IsNullOrEmpty(s)).Join(separator);

        /// <summary>
        /// A convenience method for easier access to <see cref="string.Split(string[],StringSplitOptions)"/>.
        /// </summary>
        /// <param name="value">The string value to split.</param>
        /// <param name="separator">The separator string to split by.</param>
        /// <param name="stringSplitOptions">
        /// The <see cref="StringSplitOptions"/> to use; by default equal to <see cref="StringSplitOptions.None"/>.
        /// </param>
        /// <returns>
        /// An array with the substrings of the given string that are delimited by <paramref name="separator"/>.
        /// </returns>
        public static string[] Split(
            this string value,
            string separator,
            StringSplitOptions stringSplitOptions = StringSplitOptions.None) =>
            value?.Split(new[] { separator }, stringSplitOptions) ?? Array.Empty<string>();
    }
}
