using System.Globalization;

namespace System
{
    public static class NumberExtensions
    {
        public static string ToTechnicalString(this int number) => number.ToString(CultureInfo.InvariantCulture);
    }
}
