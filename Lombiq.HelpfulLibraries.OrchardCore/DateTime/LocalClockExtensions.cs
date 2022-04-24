using Lombiq.HelpfulLibraries.Libraries.DateTime;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace OrchardCore.Modules;

public static class LocalClockExtensions
{
    /// <summary>
    /// Converts the given UTC date to the given time-zone by temporarily setting it in the HTTP context.
    /// </summary>
    /// <param name="dateTimeUtc">UTC date.</param>
    /// <param name="timeZoneId">IANA time-zone ID.</param>
    /// <param name="httpContext">HTTP context to be used to temporarily set in the HTTP context.</param>
    /// <returns>Local date.</returns>
    public static async Task<DateTime> ConvertToLocalAsync(
        this ILocalClock localClock,
        DateTime dateTimeUtc,
        string timeZoneId,
        HttpContext httpContext) =>
        (await ExecuteInDifferentTimeZoneAsync(
            httpContext,
            timeZoneId,
            () => localClock.ConvertToLocalAsync(ForceUtc(dateTimeUtc)))).DateTime;

    /// <summary>
    /// Converts the given local date to UTC using the given time-zone by temporarily setting it in the HTTP context.
    /// </summary>
    /// <param name="dateTimeLocal">Local date.</param>
    /// <param name="timeZoneId">IANA time-zone ID.</param>
    /// <param name="httpContext">HTTP context to be used to temporarily set in the HTTP context.</param>
    /// <returns>UTC date.</returns>
    public static Task<DateTime> ConvertToUtcAsync(
        this ILocalClock localClock,
        DateTime dateTimeLocal,
        string timeZoneId,
        HttpContext httpContext) =>
        ExecuteInDifferentTimeZoneAsync(
            httpContext,
            timeZoneId,
            () => localClock.ConvertToUtcAsync(dateTimeLocal));

    /// <summary>
    /// Converts a UTC DateTime to local time and formats it to long time format The <paramref name="dateTimeUtc"/> must
    /// be UTC. If the <see cref="DateTime.Kind"/> is something other than <see cref="DateTimeKind.Utc"/> then it will
    /// be coerced without any conversion. If you need conversion use <see cref="ConvertToUtcAsync"/> first.
    /// </summary>
    public static async Task<string> LocalizeAndFormatAsync(
        this ILocalClock localClock,
        DateTime? dateTimeUtc)
    {
        if (dateTimeUtc == null) return null;

        return ((DateTime?)(await localClock.ConvertToLocalAsync(dateTime: dateTimeUtc.Value)).DateTime).ToString();
    }

    private static async Task<T> ExecuteInDifferentTimeZoneAsync<T>(HttpContext httpContext, string timeZoneId, Func<Task<T>> asyncAction)
    {
        var previousTimeZoneId = httpContext.GetTimeZoneId();
        httpContext.SetTimeZoneId(timeZoneId);

        var result = await asyncAction();

        if (previousTimeZoneId == null) httpContext.Items.Remove(HttpContextKeys.TimeZoneIdKey);
        else httpContext.SetTimeZoneId(previousTimeZoneId);

        return result;
    }

    private static DateTimeOffset ForceUtc(DateTime dateTimeUtc) =>
        new(dateTimeUtc.Kind != DateTimeKind.Utc
            ? new DateTime(dateTimeUtc.Ticks, DateTimeKind.Utc)
            : dateTimeUtc);
}
