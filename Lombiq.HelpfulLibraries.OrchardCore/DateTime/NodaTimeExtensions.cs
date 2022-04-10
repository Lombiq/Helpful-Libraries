using System;
using System.Globalization;

namespace NodaTime;

public static class NodaTimeExtensions
{
    /// <summary>
    /// Does the same as <see cref="DateTime.ToShortDateString"/> except for <see cref="LocalDate"/>.
    /// </summary>
    public static string ToShortDateString(this LocalDate localDate) =>
        localDate.ToString("d", CultureInfo.InvariantCulture);
}
