using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;

namespace Lombiq.HelpfulLibraries.AspNetCore.Security;

/// <summary>
/// A content security policy directive provider that provides additional permitted host names for <see
/// cref="StyleSrc"/> and <see cref="ScriptSrc"/>.
/// </summary>
public class CdnContentSecurityPolicyProvider : IContentSecurityPolicyProvider
{
    /// <summary>
    /// Gets the sources that will be added to the <see cref="StyleSrc"/> directive.
    /// </summary>
    public static ConcurrentBag<string> PermittedStyleSources { get; } = new(
    [
        "fonts.googleapis.com",
        "fonts.gstatic.com", // #spell-check-ignore-line
        "cdn.jsdelivr.net", // #spell-check-ignore-line
        "fastly.jsdelivr.net", // #spell-check-ignore-line
        "cdnjs.cloudflare.com", // #spell-check-ignore-line
        "maxcdn.bootstrapcdn.com", // #spell-check-ignore-line
    ]);

    /// <summary>
    /// Gets the sources that will be added to the <see cref="ScriptSrc"/> directive.
    /// </summary>
    public static ConcurrentBag<string> PermittedScriptSources { get; } = new(
    [
        "cdn.jsdelivr.net", // #spell-check-ignore-line
        "cdnjs.cloudflare.com", // #spell-check-ignore-line
        "code.jquery.com",
        "fastly.jsdelivr.net", // #spell-check-ignore-line
        "maxcdn.bootstrapcdn.com", // #spell-check-ignore-line
    ]);

    /// <summary>
    /// Gets the sources that will be added to the <see cref="FontSrc"/> directive.
    /// </summary>
    public static ConcurrentBag<string> PermittedFontSources { get; } = new(
    [
        "cdn.jsdelivr.net", // #spell-check-ignore-line
        "fonts.googleapis.com",
        "fonts.gstatic.com", // #spell-check-ignore-line
    ]);

    /// <summary>
    /// Gets the sources that will be added to the <see cref="FrameSrc"/> directive.
    /// </summary>
    public static ConcurrentBag<string> PermittedFrameSources { get; } = [];

    public ValueTask UpdateAsync(IDictionary<string, string> securityPolicies, HttpContext context)
    {
        var any = false;

        if (!PermittedStyleSources.IsEmpty)
        {
            any = true;
            CspHelper.MergeValues(securityPolicies, StyleSrc, PermittedStyleSources);
        }

        if (!PermittedScriptSources.IsEmpty)
        {
            any = true;
            CspHelper.MergeValues(securityPolicies, ScriptSrc, PermittedScriptSources);
        }

        if (!PermittedFontSources.IsEmpty)
        {
            any = true;
            CspHelper.MergeValues(securityPolicies, FontSrc, PermittedFontSources);
        }

        if (!PermittedFrameSources.IsEmpty)
        {
            any = true;
            CspHelper.MergeValues(securityPolicies, FrameSrc, PermittedFrameSources);
        }

        if (any)
        {
            var allPermittedSources = PermittedStyleSources.Concat(PermittedScriptSources).Concat(PermittedFontSources);
            CspHelper.MergeValues(securityPolicies, ConnectSrc, allPermittedSources);
        }

        return ValueTask.CompletedTask;
    }
}
