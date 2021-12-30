using System;
using System.Globalization;
using System.Linq;

namespace Lombiq.HelpfulLibraries.Libraries.Utilities
{
    public static class StringHelper
    {
        public static string Concatenate(params FormattableString[] formattableStrings) =>
            Join(string.Empty, formattableStrings);

        public static string Join(string separator, params FormattableString[] formattableStrings) =>
            string.Join(
                separator,
                formattableStrings.Select(formattable => formattable.ToString(CultureInfo.InvariantCulture)));

        public static string ConcatenateConvertiblesInvariant(params IConvertible[] formattables) =>
            string.Join(
                string.Empty,
                formattables.Select(formattable => formattable.ToString(CultureInfo.InvariantCulture)));
    }
}
