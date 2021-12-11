using System.Globalization;

namespace System
{
    public static class NumberExtensions
    {
        /// <summary>
        /// Returns culture-invariant string created from the specified <see cref="int"/>.
        /// </summary>
        /// <param name="number">The number to stringify.</param>
        public static string ToTechnicalString(this int number) => number.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Returns the result if the <paramref name="number"/> can be parsed to <see cref="int"/>. Returns -1 if the
        /// parse failed.
        /// </summary>
        public static int ToTechnicalInt(this string number) =>
            int.TryParse(number, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : -1;
    }
}
