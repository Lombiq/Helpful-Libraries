using System;

namespace OrchardCore.Modules;

public static class ClockExtensions
{
    /// <summary>
    /// Returns the Unix timestamp of the current time in UTC.
    /// </summary>
    public static long GetUnixTimeMilliseconds(this IClock clock) =>
        new DateTimeOffset(clock.UtcNow).ToUnixTimeMilliseconds();
}
