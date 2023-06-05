using System;

namespace Lombiq.HelpfulLibraries.Common.Extensions;

public static class TimeSpanExtensions
{
    /// <summary>
    /// Returns a culture-invariant <see cref="string"/> created from the given <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="timeSpan">The <see cref="TimeSpan"/> to stringify.</param>
    public static string ToTechnicalString(this TimeSpan timeSpan) => timeSpan.ToString("c");
}
