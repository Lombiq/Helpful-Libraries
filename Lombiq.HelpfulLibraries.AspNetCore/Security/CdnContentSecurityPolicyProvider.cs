using Microsoft.AspNetCore.Http;
using System;
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
    // These may be amended during program setup.
#pragma warning disable CA2227 // CA2227: Change 'PermittedStyleSources' to be read-only by removing the property setter
    /// <summary>
    /// Gets or sets the URLs whose <see cref="Uri.Host"/> will be added to the <see cref="StyleSrc"/> directive.
    /// </summary>
    public static ICollection<Uri> PermittedStyleSources { get; set; } = new[]
    {
        new Uri("https://fonts.googleapis.com/css"),
        new Uri("https://fonts.gstatic.com/"),
        new Uri("https://cdn.jsdelivr.net/npm"),
    };

    /// <summary>
    /// Gets or sets the URLs whose <see cref="Uri.Host"/> will be added to the <see cref="ScriptSrc"/> directive.
    /// </summary>
    public static ICollection<Uri> PermittedScriptSources { get; set; } = new[]
    {
        new Uri("https://cdn.jsdelivr.net/npm"),
    };
#pragma warning restore CA2227 // CA2227: Change 'PermittedStyleSources' to be read-only by removing the property setter

    public ValueTask UpdateAsync(IDictionary<string, string> securityPolicies, HttpContext context)
    {
        if (PermittedStyleSources.Any())
        {
            securityPolicies[StyleSrc] += " " + string.Join(' ', PermittedStyleSources.Select(uri => uri.Host));
        }

        if (PermittedScriptSources.Any())
        {
            securityPolicies[ScriptSrc] += " " + string.Join(' ', PermittedScriptSources.Select(uri => uri.Host));
        }

        return ValueTask.CompletedTask;
    }
}
