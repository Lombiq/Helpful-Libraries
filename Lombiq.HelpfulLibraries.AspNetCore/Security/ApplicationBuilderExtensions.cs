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
    /// <param name="allowInlineScript">
    /// If <see langword="true"/> then inline scripts are permitted. When using Orchard Core a lot of front end shapes
    /// use inline script blocks without a nonce (see https://github.com/OrchardCMS/OrchardCore/issues/13389) making
    /// this a required setting.
    /// </param>
    /// <param name="allowInlineStyle">
    /// If <see langword="true"/> then inline styles are permitted. Note that even if your site has no embedded style
    /// blocks and no style attributes, some Javascript libraries may still create some from code.
    /// </param>
    public static IApplicationBuilder UseContentSecurityPolicyHeader(
        this IApplicationBuilder app,
        bool allowInlineScript,
        bool allowInlineStyle) =>
        app.Use(async (context, next) =>
        {
            const string key = "Content-Security-Policy";

            if (context.Response.Headers.ContainsKey(key))
            {
                await next();
                return;
            }

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

            if (allowInlineScript) securityPolicies[ScriptSrc] = $"{Self} {UnsafeInline}";
            if (allowInlineStyle) securityPolicies[StyleSrc] = $"{Self} {UnsafeInline}";

            context.Response.OnStarting(async () =>
            {
                // No need to do content security policy on non-HTML responses.
                if (context.Response.ContentType?.ContainsOrdinalIgnoreCase("text/html") == true) return;

                // The thought behind this provider model is that if you need something else than the default, you should
                // add a provider that only applies the additional directive on screens where it's actually needed. This way
                // we  maintain minimal permissions. If you need additional
                foreach (var provider in context.RequestServices.GetServices<IContentSecurityPolicyProvider>())
                {
                    await provider.UpdateAsync(securityPolicies, context);
                }

                var policy = string.Join("; ", securityPolicies.Select((key, value) => $"{key} {value}"));
                context.Response.Headers.Add(key, policy);
            });

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
