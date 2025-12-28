using Lombiq.HelpfulLibraries.AspNetCore.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives.CommonValues;

namespace Microsoft.AspNetCore.Builder;

public static class ApplicationBuilderExtensions
{
    private static readonly char[] _cspDirectivesSeparator = [';'];

    /// <summary>
    /// Adds a middleware that supplies the <c>Content-Security-Policy</c> header. It may be further expanded by
    /// registering services that implement <see cref="IContentSecurityPolicyProvider"/>.
    /// </summary>
    /// <param name="options">Options for the middleware.</param>
    [SuppressMessage(
        "Critical Code Smell",
        "S3776:Cognitive Complexity of methods should not be too high",
        Justification = "It's not that complex, calculation is skewed by the logic being inside an anonymous function.")]
    public static IApplicationBuilder UseContentSecurityPolicyHeader(
        this IApplicationBuilder app,
        ContentSecurityPolicyHeaderConfiguration options) =>
        app.Use(async (context, next) =>
        {
            const string key = "Content-Security-Policy";
            var headerExists = context.Response.Headers.ContainsKey(key);

            if (headerExists && options.OverwriteMode == ContentSecurityPolicyHeaderOverwriteMode.StopIfHeaderExists)
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

            if (options.AllowInlineScript) securityPolicies[ScriptSrc] = $"{Self} {UnsafeInline}";
            if (options.AllowInlineStyle) securityPolicies[StyleSrc] = $"{Self} {UnsafeInline}";

            context.Response.OnStarting(async () =>
            {
                var providers = context.RequestServices.GetServices<IContentSecurityPolicyProvider>().AsList();

                // Additional extension point for scenarios where it's desirable to skip the Content-Security-Policy
                // entirely.
                foreach (var provider in providers)
                {
                    if (await provider.ShouldSuppressHeaderAsync(context)) return;
                }

                // The thought behind this provider model is that if you need something else than the default, you
                // should add a provider that only applies the additional directive on screens where it's actually
                // needed. This way we maintain minimal permissions. Also if you need additional permissions for a
                // specific action you can use the [ContentSecurityPolicyAttribute(value, name, parentName)] attribute.
                foreach (var provider in providers)
                {
                    await provider.UpdateAsync(securityPolicies, context);
                }

                if (securityPolicies.Count == 0) return;

                if (headerExists && options.OverwriteMode == ContentSecurityPolicyHeaderOverwriteMode.MergeWithExistingHeader)
                {
                    var existingPolicy = context.Response.Headers[key];
                    var existingDirectives = existingPolicy.SelectMany(policy =>
                        policy?.Split(_cspDirectivesSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? []);

                    foreach (var directive in existingDirectives)
                    {
                        var parts = directive.Split(' ', 2);

                        if (parts.Length != 2) continue;

                        var name = parts[0];
                        var value = parts[1];

                        securityPolicies[name] = securityPolicies.TryGetValue(name, out var overridingValue)
                            ? value.MergeWordSets(overridingValue)
                            : value;
                    }
                }

                var policy = string.Join("; ", securityPolicies.Select(pair => $"{pair.Key} {pair.Value}"));
                context.Response.Headers[key] = policy;
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
    /// MIME-sniffing." As written in <see href="https://www.zaproxy.org/docs/alerts/10021/">the documentation</see>.
    /// </para></remarks>
    public static IApplicationBuilder UseNosniffContentTypeOptionsHeader(this IApplicationBuilder app) =>
        app.Use(async (context, next) =>
        {
            const string key = "X-Content-Type-Options";

            if (!context.Response.Headers.ContainsKey(key))
            {
                context.Response.Headers.Append(key, "nosniff");
            }

            await next();
        });

    /// <summary>
    /// Adds a middleware that checks all <c>Set-Cookie</c> headers and replaces any with a version containing
    /// <c>Secure</c> and <c>SameSite=Strict</c> modifiers if they were missing.
    /// </summary>
    /// <remarks><para>
    /// With this all cookies will only work in a secure context, so you should have some way to automatically redirect
    /// any HTTP request to HTTPS.
    /// </para></remarks>
    public static IApplicationBuilder UseStrictAndSecureCookies(this IApplicationBuilder app)
    {
        static void UpdateIfMissing(ref string cookie, ref bool changed, string test, string append)
        {
            if (!cookie.ContainsOrdinalIgnoreCase(test))
            {
                cookie += append;
                changed = true;
            }
        }

        return app.Use((context, next) =>
        {
            const string setCookieHeader = "Set-Cookie";
            context.Response.OnStarting(() =>
            {
                var setCookie = context.Response.Headers[setCookieHeader];
                if (setCookie.Count == 0) return Task.CompletedTask;

                var newCookies = new List<string>(capacity: setCookie.Count);
                var changed = false;

                foreach (var cookie in setCookie.WhereNotNullOrWhiteSpace())
                {
                    var newCookie = cookie;

                    // The "orch_notify" cookie is used by Orchard Core' INotifier. It's usually set before a redirect,
                    // so using SameSite=Strict would break it.
                    UpdateIfMissing(
                        ref newCookie,
                        ref changed,
                        "SameSite",
                        newCookie.StartsWithOrdinalIgnoreCase("orch_notify") ? "; SameSite=Lax" : "; SameSite=Strict");

                    UpdateIfMissing(ref newCookie, ref changed, "Secure", "; Secure");

                    newCookies.Add(newCookie);
                }

                if (changed)
                {
                    context.Response.Headers[setCookieHeader] = new StringValues([.. newCookies]);
                }

                return Task.CompletedTask;
            });

            return next();
        });
    }
}

public enum ContentSecurityPolicyHeaderOverwriteMode
{
    /// <summary>
    /// If the <c>Content-Security-Policy</c> header already exists then nothing will be done.
    /// </summary>
    StopIfHeaderExists,

    /// <summary>
    /// If the <c>Content-Security-Policy</c> header already exists then it will be overwritten.
    /// </summary>
    OverwriteExistingHeader,

    /// <summary>
    /// If the <c>Content-Security-Policy</c> header already exists then the new directives will be merged with the
    /// existing ones, the result being a union of all the directives with all their values.
    /// </summary>
    MergeWithExistingHeader,
}

public class ContentSecurityPolicyHeaderConfiguration
{
    /// <summary>
    /// Gets or sets the mode of how to handle existing <c>Content-Security-Policy</c> header values.
    /// </summary>
    public ContentSecurityPolicyHeaderOverwriteMode OverwriteMode { get; set; } = ContentSecurityPolicyHeaderOverwriteMode.StopIfHeaderExists;

    /// <summary>
    /// Gets or sets a value indicating whether inline scripts are permitted. When using Orchard Core a lot of front end
    /// shapes use inline script blocks without a nonce (see https://github.com/OrchardCMS/OrchardCore/issues/13389)
    /// making this a required setting.
    /// </summary>
    public bool AllowInlineScript { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether inline styles are permitted. Note that even if your site has no embedded
    /// style blocks and no style attributes, some JavaScript libraries may still create some from code.
    /// </summary>
    public bool AllowInlineStyle { get; set; }
}
