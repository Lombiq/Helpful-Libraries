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
    }
}
