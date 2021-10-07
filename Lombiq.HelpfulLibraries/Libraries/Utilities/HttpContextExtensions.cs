using OrchardCore.ContentManagement;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Http
{
    public static class HttpContextExtensions
    {
        private const string ContentSessionDataInfix = "_SessionData_";

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

        public static void SetContentSessionData(this HttpContext httpContext, IContent content, object data) =>
            httpContext.Items[GetContentSessionDataKey(content)] = data;

        public static T GetOrCreateContentSessionData<T>(this HttpContext httpContext, IContent content)
            where T : new()
        {
            if (httpContext.Items.GetMaybe(GetContentSessionDataKey(content)) is T data) return data;

            SetContentSessionData(httpContext, content, new T());
            return (T)httpContext.Items.GetMaybe(GetContentSessionDataKey(content));
        }

        private static string GetContentSessionDataKey(IContent content) =>
            content.ContentItem.ContentType + ContentSessionDataInfix + content.ContentItem.ContentItemId;
    }
}
