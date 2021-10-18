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
        /// Returns culture-invariant <see cref="int"/> created from the specified <see cref="string"/>.
        /// </summary>
        /// <param name="number">The number as <see cref="string"/> to parse to <see cref="int"/>.</param>
        public static int ToTechnicalInt(this string number) => int.Parse(number, CultureInfo.InvariantCulture);
    }
}
