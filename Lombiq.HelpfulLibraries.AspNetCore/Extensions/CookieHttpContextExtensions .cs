using System;

namespace Microsoft.AspNetCore.Http;

public static class CookieHttpContextExtensions
{
    /// <summary>
    /// Sets the cookie with the given name with a maximal expiration time.
    /// </summary>
    public static void SetCookieForever(this HttpContext httpContext, string name, string value, SameSiteMode sameSite = SameSiteMode.Lax) =>
        httpContext.Response.Cookies.Append(name, value, new CookieOptions
        {
            Expires = DateTimeOffset.MaxValue,
            Secure = true,
            HttpOnly = true,
            SameSite = sameSite,
        });

    /// <summary>
    /// Sets the cookie with the given name with a maximal expiration time.
    /// </summary>
    public static void SetCookieForever(this IHttpContextAccessor httpContextAccessor, string name, string value) =>
        httpContextAccessor.HttpContext.SetCookieForever(name, value);
}
