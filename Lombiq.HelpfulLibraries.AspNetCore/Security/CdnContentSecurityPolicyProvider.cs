using Microsoft.AspNetCore.Http;
using System;
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
    /// Gets the URLs whose <see cref="Uri.Host"/> will be added to the <see cref="StyleSrc"/> directive.
    /// </summary>
    public static ConcurrentBag<Uri> PermittedStyleSources { get; } = new(new[]
    {
        new Uri("https://fonts.googleapis.com/css"),
        new Uri("https://fonts.gstatic.com/"),
        new Uri("https://cdn.jsdelivr.net/npm"),
        new Uri("https://fastly.jsdelivr.net/npm"),
        new Uri("https://cdnjs.cloudflare.com/"),
        new Uri("https://maxcdn.bootstrapcdn.com/"),
    });

    /// <summary>
    /// Gets the URLs whose <see cref="Uri.Host"/> will be added to the <see cref="ScriptSrc"/> directive.
    /// </summary>
    public static ConcurrentBag<Uri> PermittedScriptSources { get; } = new(new[]
    {
        new Uri("https://cdn.jsdelivr.net/npm"),
        new Uri("https://cdnjs.cloudflare.com/"),
        new Uri("https://code.jquery.com/"),
        new Uri("https://fastly.jsdelivr.net/npm"),
        new Uri("https://maxcdn.bootstrapcdn.com/"),
    });

    /// <summary>
    /// Gets the URLs whose <see cref="Uri.Host"/> will be added to the <see cref="FontSrc"/> directive.
    /// </summary>
    public static ConcurrentBag<Uri> PermittedFontSources { get; } = new(new[]
    {
        new Uri("https://cdn.jsdelivr.net/npm"),
        new Uri("https://fonts.googleapis.com/"),
        new Uri("https://fonts.gstatic.com/"),
    });

    /// <summary>
    /// Gets the URLs whose <see cref="Uri.Host"/> will be added to the <see cref="FrameSrc"/> directive.
    /// </summary>
    public static ConcurrentBag<Uri> PermittedFrameSources { get; } = new();

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
