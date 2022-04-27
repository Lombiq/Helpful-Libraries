using System;

namespace Microsoft.AspNetCore.Http;

public static class CookieHttpContextExtensions
{
    /// <summary>
    /// Sets the cookie with the given name with a maximal expiration time.
    /// </summary>
    public static void SetCookieForever(this HttpContext httpContext, string name, string value) =>
        httpContext.Response.Cookies.Append(name, value, new CookieOptions
        {
            Expires = DateTimeOffset.MaxValue,
            Secure = true,
            HttpOnly = true,
        });

    /// <summary>
    /// Sets the cookie with the given name with a maximal expiration time.
    /// </summary>
    public static void SetCookieForever(this IHttpContextAccessor httpContextAccessor, string name, string value) =>
        httpContextAccessor.HttpContext.SetCookieForever(name, value);
}
