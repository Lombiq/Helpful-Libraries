using System.Globalization;

namespace System
{
    public static class NumberExtensions
    {
        /// <summary>
        /// Returns culture-invariant string created from the specified <see cref="sbyte"/>.
        /// </summary>
        /// <param name="number">The number to stringify.</param>
        public static string ToTechnicalString(this sbyte number) => number.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Returns culture-invariant string created from the specified <see cref="byte"/>.
        /// </summary>
        /// <param name="number">The number to stringify.</param>
        public static string ToTechnicalString(this byte number) => number.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Returns culture-invariant string created from the specified <see cref="short"/>.
        /// </summary>
        /// <param name="number">The number to stringify.</param>
        public static string ToTechnicalString(this short number) => number.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Returns culture-invariant string created from the specified <see cref="ushort"/>.
        /// </summary>
        /// <param name="number">The number to stringify.</param>
        public static string ToTechnicalString(this ushort number) => number.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Returns culture-invariant string created from the specified <see cref="int"/>.
        /// </summary>
        /// <param name="number">The number to stringify.</param>
        public static string ToTechnicalString(this int number) => number.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Returns culture-invariant string created from the specified <see cref="uint"/>.
        /// </summary>
        /// <param name="number">The number to stringify.</param>
        public static string ToTechnicalString(this uint number) => number.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Returns culture-invariant string created from the specified <see cref="long"/>.
        /// </summary>
        /// <param name="number">The number to stringify.</param>
        public static string ToTechnicalString(this long number) => number.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Returns culture-invariant string created from the specified <see cref="ulong"/>.
        /// </summary>
        /// <param name="number">The number to stringify.</param>
        public static string ToTechnicalString(this ulong number) => number.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Returns the result if the <paramref name="number"/> can be parsed to <see cref="int"/>. Returns -1 if the
        /// parse failed.
        /// </summary>
        public static int ToTechnicalInt(this string number) =>
            int.TryParse(number, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : -1;

        /// <summary>
        /// Returns the number of digits in the given <see cref="int"/>.
        /// </summary>
        /// <param name="number">The number to count the digits of.</param>
        public static int DigitCount(this int number) => (int)Math.Log10(number) + 1;
    }
}
