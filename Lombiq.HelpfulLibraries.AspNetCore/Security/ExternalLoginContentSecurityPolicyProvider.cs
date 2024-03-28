using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;

namespace Lombiq.HelpfulLibraries.AspNetCore.Security;

/// <summary>
/// A content security policy directive provider that provides additional permitted host names for <see
/// cref="FormAction"/> for external login providers that require this (like Microsoft and GitHub).
/// </summary>
public class ExternalLoginContentSecurityPolicyProvider : IContentSecurityPolicyProvider
{
    /// <summary>
    /// Gets the sources that will be added to the <see cref="FormAction"/> directive.
    /// </summary>
    public static ConcurrentBag<string> PermittedFormActions { get; } = new(new[]
    {
        "login.microsoftonline.com",
        "github.com",
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
