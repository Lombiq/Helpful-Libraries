using Lombiq.HelpfulLibraries.AspNetCore.DateTime;

namespace Microsoft.AspNetCore.Http;

public static class DateTimeHttpContextExtensions
{
    /// <summary>
    /// Sets the time-zone in the HTTP context.
    /// </summary>
    /// <param name="timeZoneId">IANA time-zone ID.</param>
    public static void SetTimeZoneId(this HttpContext httpContext, string timeZoneId) =>
        httpContext.Items[HttpContextKeys.TimeZoneIdKey] = timeZoneId;

    /// <summary>
    /// Gets the time-zone set in the HTTP context.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It returns null if the HTTP context doesn't contain any time-zone data which doesn't mean that no time-zone
    /// information is provided by other providers.
    /// </para>
    /// </remarks>
    /// <returns>IANA time-zone ID.</returns>
    public static string GetTimeZoneId(this HttpContext httpContext) =>
        httpContext.Items.ContainsKey(HttpContextKeys.TimeZoneIdKey) ?
            (string)httpContext.Items[HttpContextKeys.TimeZoneIdKey] : null;
}
