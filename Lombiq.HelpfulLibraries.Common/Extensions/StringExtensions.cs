using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#nullable enable

namespace System;

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
    public static string ReplaceOrdinalIgnoreCase(this string text, string oldValue, string? newValue = "") =>
        text.Replace(oldValue, newValue ?? string.Empty, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// A shortcut for <c>string.Equals(string, StringComparison.Ordinal)</c>.
    /// </summary>
    [Obsolete("The string equals operator already uses ordinal string comparison.")]
    public static bool EqualsOrdinal(this string text, string? value) =>
        throw new NotSupportedException();

    /// <summary>
    /// A shortcut for <c>string.Contains(string, StringComparison.Ordinal)</c>.
    /// </summary>
    [Obsolete("The string.Contains(value) member method already uses ordinal string comparison.")]
    public static bool ContainsOrdinal(this string text, string value) =>
        throw new NotSupportedException();

    /// <summary>
    /// A shortcut for <c>string.StartsWith(string, StringComparison.Ordinal)</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It's worth noting that the <see cref="string.StartsWith(string)"/> member method uses <see
    /// cref="StringComparison.CurrentCulture"/> as the basis of its comparison.
    /// </para>
    /// </remarks>
    public static bool StartsWithOrdinal(this string text, string value) =>
        text.StartsWith(value, StringComparison.Ordinal);

    /// <summary>
    /// A shortcut for <c>string.EndsWith(string, StringComparison.Ordinal)</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It's worth noting that the <see cref="string.EndsWith(string)"/> member method uses <see
    /// cref="StringComparison.CurrentCulture"/> as the basis of its comparison.
    /// </para>
    /// </remarks>
    public static bool EndsWithOrdinal(this string text, string value) =>
        text.EndsWith(value, StringComparison.Ordinal);

    /// <summary>
    /// A shortcut for <c>string.Replace(string, string, StringComparison.Ordinal)</c>.
    /// </summary>
    [Obsolete("The string.Replace(oldValue, newValue) member method already uses ordinal string comparison.")]
    public static string ReplaceOrdinal(this string text, string oldValue, string? newValue = "") =>
        throw new NotSupportedException();

    /// <summary>
    /// A shortcut for <c>string.CompareOrdinal(string, string)</c> static method.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It's worth noting that the <see cref="string.Compare(string?,string?)"/> static method uses <see
    /// cref="StringComparison.CurrentCulture"/> as the basis of its comparison.
    /// </para>
    /// </remarks>
    public static int CompareOrdinal(this string strA, string strB) =>
        string.CompareOrdinal(strA, strB);

    /// <summary>
    /// A shortcut for <c>string.IndexOf(string, StringComparison.Ordinal)</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It's worth noting that the <see cref="string.IndexOf(string)"/> member method uses <see
    /// cref="StringComparison.CurrentCulture"/> as the basis of its comparison.
    /// </para>
    /// </remarks>
    public static int IndexOfOrdinal(this string text, string value, int startIndex = 0) =>
        text.IndexOf(value, startIndex, StringComparison.Ordinal);

    /// <summary>
    /// A shortcut for <c>string.LastIndexOf(string, StringComparison.Ordinal)</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It's worth noting that the <see cref="string.LastIndexOf(string)"/> member method uses <see
    /// cref="StringComparison.CurrentCulture"/> as the basis of its comparison.
    /// </para>
    /// </remarks>
    public static int LastIndexOfOrdinal(this string text, string value) =>
        text.LastIndexOf(value, StringComparison.Ordinal);

    /// <summary>
    /// Returns the first string that's not <see langword="null"/> or empty, starting with <paramref name="text"/> and
    /// then the items in <paramref name="alternatives"/> sequentially. Finally <see cref="string.Empty"/> if none
    /// matched the criteria.
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
    /// langword="true"/>, otherwise returns <paramref name="text"/>. A delegate is used to avoid unnecessary expensive
    /// async calls.
    /// </summary>
    public static Task<string?> OrIfAsync(
        this string? text,
        Func<string?, bool> condition,
        Func<Task<string?>> alternativeAsync) =>
        condition(text) ? alternativeAsync() : Task.FromResult(text);

    /// <summary>
    /// Performs <see cref="Regex.Match(string, string, RegexOptions, TimeSpan)"/> with timeout (default is 1s).
    /// </summary>
    public static Match RegexMatch(
        this string input,
        string pattern,
        RegexOptions options = RegexOptions.None,
        TimeSpan? within = null) =>
        Regex.Match(input, pattern, options, within ?? TimeSpan.FromSeconds(1));

    /// <summary>
    /// Performs <see cref="Regex.IsMatch(string, string, RegexOptions, TimeSpan)"/> with timeout (default is 1s).
    /// </summary>
    public static bool RegexIsMatch(
        this string input,
        string pattern,
        RegexOptions options = RegexOptions.None,
        TimeSpan? within = null) =>
        Regex.IsMatch(input, pattern, options, within ?? TimeSpan.FromSeconds(1));

    /// <summary>
    /// Performs <see cref="Regex.Replace(string, string, string, RegexOptions, TimeSpan)"/> with timeout (default is
    /// 1s).
    /// </summary>
    public static string RegexReplace(
        this string input,
        string pattern,
        string replacement,
        RegexOptions options = RegexOptions.None,
        TimeSpan? within = null) =>
        Regex.Replace(input, pattern, replacement, options, within ?? TimeSpan.FromSeconds(1));

    /// <summary>
    /// Performs <see cref="Regex.Replace(string, string, MatchEvaluator, RegexOptions, TimeSpan)"/> with timeout
    /// (default is 1s).
    /// </summary>
    public static string RegexReplace(
        this string input,
        string pattern,
        MatchEvaluator evaluator,
        RegexOptions options = RegexOptions.None,
        TimeSpan? within = null) =>
        Regex.Replace(input, pattern, evaluator, options, within ?? TimeSpan.FromSeconds(1));

    /// <summary>
    /// Similar to <see cref="string.IndexOf(string)"/>, but returns every match. The comparison is ordinal via simple
    /// character equality checks.
    /// </summary>
    public static IEnumerable<int> AllIndexesOf(this string text, string value)
    {
        if (string.IsNullOrEmpty(value)) yield break;

        var count = text.Length - value.Length;
        for (int textIndex = 0; textIndex < count; textIndex++)
        {
            var match = true;
            for (int valueIndex = 0; match && valueIndex < value.Length; valueIndex++)
            {
                if (text[textIndex + valueIndex] != value[valueIndex]) match = false;
            }

            if (match) yield return textIndex;
        }
    }

    /// <summary>
    /// Turns a camelCase or PascalCase token into snake_Case.
    /// </summary>
    /// <param name="input">The input in camelCase or PascalCase.</param>
    /// <returns>
    /// The input converted to snake_Case. It doesn't alter case so you can call either ToUpper or ToLower without any
    /// additional penalties.
    /// </returns>
    /// <remarks>
    /// <para><see href="https://stackoverflow.com/a/18781533"/>.</para>
    /// </remarks>
    public static string ToSnakeCase(this string input) =>
        string.Concat(input.Select((character, index) => index > 0 && char.IsUpper(character) ? "_" + character : character.ToString()));

    /// <summary>
    /// Splits the text into three pieces similarly to Python's <c>str.partition</c> method.
    /// </summary>
    /// <param name="text">The text to partition.</param>
    /// <param name="separator">The first instance of this text is the separator, if any.</param>
    /// <param name="ignoreCase">
    /// If <see langword="true"/> then <see cref="StringComparison.OrdinalIgnoreCase"/> is used, otherwise <see
    /// cref="StringComparison.Ordinal"/>.
    /// </param>
    /// <returns>
    /// If <paramref name="separator"/> is found, then (textBefore, firstMatch, textAfter) is returned. Otherwise (
    /// <paramref name="text"/>, <see langword="null"/>, <see langword="null"/>).
    /// </returns>
    public static (string? Left, string? Separator, string? Right) PartitionEnd(
        this string? text,
        string separator,
        bool ignoreCase = false) =>
        Partition(text, separator, ignoreCase, fromEnd: true);

    /// <summary>
    /// Splits the text into three pieces before, during and after the provided range.
    /// </summary>
    public static (string? Left, string? Separator, string? Right) Partition(this string? text, Range range) =>
        string.IsNullOrEmpty(text)
            ? (Left: text, Separator: null, Right: null)
            : (Left: text[..range.Start], Separator: text[range], Right: text[range.End..]);

    /// <summary>
    /// Splits the text into three pieces similarly to Python's <c>str.rpartition</c> method.
    /// </summary>
    /// <param name="text">The text to partition.</param>
    /// <param name="separator">The last instance of this text is the separator, if any.</param>
    /// <param name="ignoreCase">
    /// If <see langword="true"/> then <see cref="StringComparison.OrdinalIgnoreCase"/> is used, otherwise <see
    /// cref="StringComparison.Ordinal"/>.
    /// </param>
    /// <returns>
    /// If <paramref name="separator"/> is found, then (textBefore, lastMatch, textAfter) is returned. Otherwise ( <see
    /// langword="null"/>, <see langword="null"/>, <paramref name="text"/>).
    /// </returns>
    public static (string? Left, string? Separator, string? Right) Partition(
        this string? text,
        string separator,
        bool ignoreCase = false) =>
        Partition(text, separator, ignoreCase, fromEnd: false);

    private static (string? Left, string? Separator, string? Right) Partition(
        string? text,
        string separator,
        bool ignoreCase,
        bool fromEnd)
    {
        var stringComparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        var index = (fromEnd ? text?.LastIndexOf(separator, stringComparison) : text?.IndexOf(separator, stringComparison)) ?? -1;

        if (index < 0)
        {
            return fromEnd
                ? (Left: null, Separator: null, Right: text)
                : (Left: text, Separator: null, Right: null);
        }

        var end = index + separator.Length;
        return (text![..index], text[index..end], text[end..]);
    }
}
