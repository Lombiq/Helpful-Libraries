using System.Collections.Generic;
using System.Threading.Tasks;

using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives.CommonValues;

namespace Lombiq.HelpfulLibraries.AspNetCore.Security;

public class AntiClickjackingContentSecurityPolicyProvider : IContentSecurityPolicyProvider
{
    public ValueTask UpdateAsync(IDictionary<string, string> securityPolicies)
    {
        securityPolicies[FrameAncestors] = Self;

        return ValueTask.CompletedTask;
    }
}
