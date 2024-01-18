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
    });

    /// <summary>
    /// Gets the URLs whose <see cref="Uri.Host"/> will be added to the <see cref="ScriptSrc"/> directive.
    /// </summary>
    public static ConcurrentBag<Uri> PermittedScriptSources { get; } = new(new[]
    {
        new Uri("https://cdn.jsdelivr.net/npm"),
    });

    /// <summary>
    /// Gets the URLs whose <see cref="Uri.Host"/> will be added to the <see cref="FontSrc"/> directive.
    /// </summary>
    public static ConcurrentBag<Uri> PermittedFontSources { get; } = new(new[]
    {
        new Uri("https://fonts.googleapis.com/"),
        new Uri("https://fonts.gstatic.com/"),
    });

    public ValueTask UpdateAsync(IDictionary<string, string> securityPolicies, HttpContext context)
    {
        var any = false;

        if (PermittedStyleSources.Any())
        {
            any = true;
            MergeValues(securityPolicies, StyleSrc, PermittedStyleSources);
        }

        if (PermittedScriptSources.Any())
        {
            any = true;
            MergeValues(securityPolicies, ScriptSrc, PermittedScriptSources);
        }

        if (PermittedFontSources.Any())
        {
            any = true;
            MergeValues(securityPolicies, FontSrc, PermittedFontSources);
        }

        if (any)
        {
            var allPermittedSources = PermittedStyleSources.Concat(PermittedScriptSources).Concat(PermittedFontSources);
            MergeValues(securityPolicies, ConnectSrc, allPermittedSources);
        }

        return ValueTask.CompletedTask;
    }

    private static void MergeValues(IDictionary<string, string> policies, string key, IEnumerable<Uri> sources)
    {
        var directiveValue = policies.GetMaybe(key) ?? policies.GetMaybe(DefaultSrc) ?? string.Empty;

        policies[key] = string.Join(' ', directiveValue
            .Split(' ')
            .Union(sources.Select(uri => uri.Host))
            .Distinct());
    }
}
