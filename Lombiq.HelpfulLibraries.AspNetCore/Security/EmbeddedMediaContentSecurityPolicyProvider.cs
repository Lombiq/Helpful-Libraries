using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;

namespace Lombiq.HelpfulLibraries.AspNetCore.Security;

/// <summary>
/// A content security policy directive provider that provides additional permitted host names used by usual media
/// embedding sources (like YouTube) for <see cref="FrameSrc"/>.
/// </summary>
public class EmbeddedMediaContentSecurityPolicyProvider : IContentSecurityPolicyProvider
{
    /// <summary>
    /// Gets the sources that will be added to the <see cref="FrameSrc"/> directive.
    /// </summary>
    public static ConcurrentBag<string> PermittedFrameSources { get; } = new(
    [
        "www.youtube.com",
        "www.youtube-nocookie.com",
    ]);

    public ValueTask UpdateAsync(IDictionary<string, string> securityPolicies, HttpContext context)
    {
        if (!PermittedFrameSources.IsEmpty)
        {
            CspHelper.MergeValues(securityPolicies, FrameSrc, PermittedFrameSources);
        }

        return ValueTask.CompletedTask;
    }
}
