using Lombiq.HelpfulLibraries.Libraries.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Environment.Extensions;
using System;
using System.Linq.Expressions;

namespace Microsoft.AspNetCore.Http
{
    public static class HttpContextExtensions
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

        /// <summary>
        /// Returns a relative URL string to a controller action inside an Orchard Core module. Similar to <see
        /// cref="Action"/>, but it uses a strongly typed lambda <see cref="Expression"/> to select the
        /// action and populate the query arguments.
        /// </summary>
        public static string Action<TController>(
            this HttpContext httpContext,
            Expression<Action<TController>> actionExpression,
            string tenantName = null,
            params (string Key, object Value)[] additionalArguments)
        {
            var provider = httpContext.RequestServices.GetService<ITypeFeatureProvider>();
            var model = RouteModel.CreateFromExpression(
                actionExpression,
                additionalArguments,
                provider);
            return model.ToString(tenantName);
        }
    }
}
