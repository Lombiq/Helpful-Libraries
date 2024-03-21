using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;

namespace Lombiq.HelpfulLibraries.AspNetCore.Security;

/// <summary>
/// A content security policy directive provider that permits wide local access for <see cref="ConnectSrc"/> so it can
/// be used in Visual Studio's debug mode feature Browser Link.
/// </summary>
public class BrowserLinkContentSecurityPolicyProvider : IContentSecurityPolicyProvider
{
    private readonly IHttpContextAccessor _hca;

    public BrowserLinkContentSecurityPolicyProvider(IHttpContextAccessor hca) =>
        _hca = hca;

    public ValueTask UpdateAsync(IDictionary<string, string> securityPolicies, HttpContext context)
    {
        if (_hca.HttpContext?.IsDevelopmentAndLocalhost() == true)
        {
            // Browser Link is accessed through multiple random ports on localhost.
            securityPolicies[ConnectSrc] = IContentSecurityPolicyProvider
                .GetDirective(securityPolicies, ConnectSrc)
                .MergeWordSets("localhost:*");
        }

        return ValueTask.CompletedTask;
    }
}
