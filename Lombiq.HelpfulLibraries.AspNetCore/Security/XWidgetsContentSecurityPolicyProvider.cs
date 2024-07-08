using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;

namespace Lombiq.HelpfulLibraries.AspNetCore.Security;

/// <summary>
/// An optional content security policy provider that provides configuration to allow the usage of X (Twitter) social
/// widgets.
/// </summary>
public class XWidgetsContentSecurityPolicyProvider : IContentSecurityPolicyProvider
{
    private const string PlatformDotTwitter = "platform.twitter.com";

    public ValueTask UpdateAsync(IDictionary<string, string> securityPolicies, HttpContext context)
    {
        CspHelper.MergeValues(securityPolicies, FrameSrc, PlatformDotTwitter);
        CspHelper.MergeValues(securityPolicies, ImgSrc, PlatformDotTwitter, "syndication.twitter.com");
        CspHelper.MergeValues(securityPolicies, StyleSrc, PlatformDotTwitter);
        CspHelper.MergeValues(securityPolicies, ScriptSrc, PlatformDotTwitter);

        return ValueTask.CompletedTask;
    }
}
