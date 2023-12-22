using Lombiq.HelpfulLibraries.AspNetCore.Security;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;

namespace Microsoft.AspNetCore.Builder;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds a middleware that supplies <c>Content-Security-Policy</c> header. It may be further expanded by registering
    /// services that implement <see cref="IContentSecurityPolicyProvider"/>.
    /// </summary>
    /// <param name="allowInline">If <see langword="true"/> inline scripts are permitted by including the </param>
    public static IApplicationBuilder UseContentSecurityPolicyHeader(this IApplicationBuilder app, bool allowInline = true) =>
        app.Use(async (context, next) =>
        {
            var securityPolicies = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // Default value enforcing a same origin policy for all resources.
                [DefaultSrc] = CommonValues.Self,
                // Needed for SVG images using "data:image/svg+xml,..." data URLs.
                [ImgSrc] = $"{CommonValues.Self} {CommonValues.Data}",
            };

            if (allowInline) securityPolicies[ScriptSrc] = CommonValues.UnsafeInline;

            foreach (var provider in context.RequestServices.GetService<IEnumerable<IContentSecurityPolicyProvider>>())
            {
                await provider.UpdateAsync(securityPolicies);
            }

            var policy = string.Join("; ", securityPolicies.Select((key, value) => $"{key} {value}"));
            context.Response.Headers.Add("Content-Security-Policy", policy);

            await next();
        });
}
