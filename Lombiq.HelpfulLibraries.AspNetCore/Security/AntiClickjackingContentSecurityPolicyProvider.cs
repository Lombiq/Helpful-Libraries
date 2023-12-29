using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives.CommonValues;

namespace Lombiq.HelpfulLibraries.AspNetCore.Security;

/// <summary>
/// A content security policy directive provider that sets the <see cref="FrameAncestors"/> directive to <see
/// cref="Self"/>, to prevent <a href="https://developer.mozilla.org/en-US/docs/Glossary/Clickjacking">clickjacking</a>.
/// </summary>
public class AntiClickjackingContentSecurityPolicyProvider : IContentSecurityPolicyProvider
{
    public ValueTask UpdateAsync(IDictionary<string, string> securityPolicies, HttpContext context)
    {
        securityPolicies[FrameAncestors] = Self;

        return ValueTask.CompletedTask;
    }
}
