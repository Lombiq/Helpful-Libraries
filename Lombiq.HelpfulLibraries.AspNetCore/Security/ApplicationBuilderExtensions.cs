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
    /// Adds a middleware that supplies the <c>Content-Security-Policy</c> header. It may be further expanded by
    /// registering services that implement <see cref="IContentSecurityPolicyProvider"/>.
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
                [FormAction] = Self,
                // Needed for SVG images using "data:image/svg+xml,..." data URLs.
                [ImgSrc] = $"{Self} {Data}",
                // Modern sites shouldn't need <object>, <embed>, and <applet> elements.
                [ObjectSrc] = None,
                // Necessary to prevent clickjacking (https://developer.mozilla.org/en-US/docs/Glossary/Clickjacking).
                [FrameAncestors] = Self,
            };

            // Orchard Core setup will fail without 'unsafe-inline'. Additionally, it's almost guaranteed that some page
            // will contain non-precompiled Vue.js code from built-in OC features and that requires 'unsafe-eval'.
            if (allowInline)
            {
                securityPolicies[ScriptSrc] = $"{Self} {UnsafeInline} {UnsafeEval}";
                securityPolicies[StyleSrc] = $"{Self} {UnsafeInline}";
            }

            // The thought behind this provider model is that if you need something else than the default, you should
            // add a provider that only applies the additional directive on screens where it's actually needed. This way
            // we  maintain minimal permissions. If you need additional
            foreach (var provider in context.RequestServices.GetService<IEnumerable<IContentSecurityPolicyProvider>>())
            {
                await provider.UpdateAsync(securityPolicies, context);
            }

            var policy = string.Join("; ", securityPolicies.Select((key, value) => $"{key} {value}"));
            context.Response.Headers.Add("Content-Security-Policy", policy);

            await next();
        });

    /// <summary>
    /// Adds a middleware that sets the <c>X-Content-Type-Options</c> header to <c>nosniff</c>.
    /// </summary>
    /// <remarks><para>
    /// "The Anti-MIME-Sniffing header X-Content-Type-Options was not set to ’nosniff’. This allows older versions of
    /// Internet Explorer and Chrome to perform MIME-sniffing on the response body, potentially causing the response
    /// body to be interpreted  and displayed as a content type other than the declared content type. Current (early
    /// 2014) and legacy versions  of Firefox will use the declared content type (if one is set), rather than performing
    /// MIME-sniffing." As written in <a href="https://www.zaproxy.org/docs/alerts/10021/">the documentation</a>.
    /// </para></remarks>
    public static IApplicationBuilder UseNosniffContentTypeOptionsHeader(this IApplicationBuilder app) =>
        app.Use(async (context, next) =>
        {
            const string key = "X-Content-Type-Options";

            if (!context.Response.Headers.ContainsKey(key))
            {
                context.Response.Headers.Add(key, "nosniff");
            }

            await next();
        });
}
