using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

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
}
