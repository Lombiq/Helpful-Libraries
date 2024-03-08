using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;

namespace Lombiq.HelpfulLibraries.AspNetCore.Security;

/// <summary>
/// A service for updating the dictionary that will be turned into the <c>Content-Security-Policy</c> header value by
/// <see cref="ApplicationBuilderExtensions.UseContentSecurityPolicyHeader"/>.
/// </summary>
public interface IContentSecurityPolicyProvider
{
    /// <summary>
    /// Updates the <paramref name="securityPolicies"/> dictionary where the keys are the <c>Content-Security-Policy</c>
    /// directives names and the values are the matching directive values.
    /// </summary>
    public ValueTask UpdateAsync(IDictionary<string, string> securityPolicies, HttpContext context);

    /// <summary>
    /// Returns the first non-empty directive from the <paramref name="names"/> or <see cref="DefaultSrc"/> or an empty
    /// string.
    /// </summary>
    public static string GetDirective(IDictionary<string, string> securityPolicies, params string[] names)
    {
        foreach (var name in names)
        {
            if (securityPolicies.TryGetValue(name, out var value) && !string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return securityPolicies.GetMaybe(DefaultSrc) ?? string.Empty;
    }
}
