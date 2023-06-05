using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Lombiq.HelpfulLibraries.Common.Utilities;

/// <summary>
/// Utility functions to convert other types into string.
/// </summary>
public static class StringHelper
{
    /// <summary>
    /// Converts <see cref="FormattableString"/> instances into <see cref="CultureInfo.InvariantCulture"/><see
    /// langword="string"/> and then concatenates them.
    /// </summary>
    /// <param name="formattableStrings">You can pass interpolated strings here directly.</param>
    public static string Concatenate(params FormattableString[] formattableStrings) =>
        Join(string.Empty, formattableStrings);

    /// <summary>
    /// Converts <see cref="FormattableString"/> instances into <see cref="CultureInfo.InvariantCulture"/><see
    /// langword="string"/> and then joins them together with <paramref name="separator"/> between them.
    /// </summary>
    /// <param name="separator">The text to insert between the instances.</param>
    /// <param name="formattableStrings">You can pass interpolated strings here directly.</param>
    public static string Join(string separator, params FormattableString[] formattableStrings) =>
        string.Join(
            separator,
            formattableStrings.Select(formattable => formattable.ToString(CultureInfo.InvariantCulture)));

    /// <summary>
    /// Converts <see cref="IConvertible"/> instances (this includes most primitive types) into <see
    /// cref="CultureInfo.InvariantCulture"/><see langword="string"/> and then concatenates them.
    /// </summary>
    public static string ConcatenateConvertiblesInvariant(params IConvertible[] convertibles) =>
        string.Join(
            string.Empty,
            convertibles.Select(formattable => formattable.ToString(CultureInfo.InvariantCulture)));

    /// <summary>
    /// Creates a <see langword="string"/> from an interpolated string with the invariant culture. This affects culture-
    /// sensitive formatting of interpolated values.
    /// </summary>
    public static string CreateInvariant(this DefaultInterpolatedStringHandler value) =>
        string.Create(CultureInfo.InvariantCulture, ref value);
}
