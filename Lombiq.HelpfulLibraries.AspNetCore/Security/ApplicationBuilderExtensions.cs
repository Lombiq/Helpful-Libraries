using Lombiq.HelpfulLibraries.AspNetCore.Security;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives.CommonValues;

namespace Microsoft.AspNetCore.Builder;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds a middleware that supplies <c>Content-Security-Policy</c> header. It may be further expanded by registering
    /// services that implement <see cref="IContentSecurityPolicyProvider"/>.
    /// </summary>
    /// <param name="allowInline">If <see langword="true"/> then inline scripts and styles are permitted.</param>
    public static IApplicationBuilder UseContentSecurityPolicyHeader(this IApplicationBuilder app, bool allowInline) =>
        app.Use(async (context, next) =>
        {
            var securityPolicies = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // Default values enforcing a same origin policy for all resources.
                [BaseUri] = Self,
                [DefaultSrc] = Self,
                [FrameSrc] = Self,
                [ScriptSrc] = Self,
                [StyleSrc] = Self,
                // Needed for SVG images using "data:image/svg+xml,..." data URLs.
                [ImgSrc] = $"{Self} {Data}",
                // Modern sites shouldn't need <object>, <embed>, and <applet> elements.
                [ObjectSrc] = None,
            };

            // Orchard Core setup will fail without 'unsafe-inline'. Additionally, it's almost guaranteed that some page
            // will contain non-precompiled Vue.js code from built-in OC features and that requires 'unsafe-eval'.
            if (allowInline)
            {
                securityPolicies[ScriptSrc] = $"{Self} {UnsafeInline} {UnsafeEval}";
                securityPolicies[StyleSrc] = $"{Self} {UnsafeInline}";
            }

            foreach (var provider in context.RequestServices.GetService<IEnumerable<IContentSecurityPolicyProvider>>())
            {
                await provider.UpdateAsync(securityPolicies, context);
            }

            var policy = string.Join("; ", securityPolicies.Select((key, value) => $"{key} {value}"));
            context.Response.Headers.Add("Content-Security-Policy", policy);

            await next();
        });
}
