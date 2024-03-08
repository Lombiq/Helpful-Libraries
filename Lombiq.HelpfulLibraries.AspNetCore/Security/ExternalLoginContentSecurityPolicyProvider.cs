using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;

namespace Lombiq.HelpfulLibraries.AspNetCore.Security;

/// <summary>
/// A content security policy directive provider that provides additional permitted host names for <see
/// cref="FormAction"/>.
/// </summary>
public class ExternalLoginContentSecurityPolicyProvider : IContentSecurityPolicyProvider
{
    /// <summary>
    /// Gets the URLs whose <see cref="Uri.Host"/> will be added to the <see cref="FormAction"/> directive.
    /// </summary>
    public static ConcurrentBag<Uri> PermittedFormActions { get; } = new(new[]
    {
        new Uri("https://login.microsoftonline.com/"),
        new Uri("https://github.com/login/"),
    });

    public ValueTask UpdateAsync(IDictionary<string, string> securityPolicies, HttpContext context)
    {
        if (!PermittedFormActions.IsEmpty)
        {
            CspHelper.MergeValues(securityPolicies, FormAction, PermittedFormActions);
        }

        return ValueTask.CompletedTask;
    }
}
