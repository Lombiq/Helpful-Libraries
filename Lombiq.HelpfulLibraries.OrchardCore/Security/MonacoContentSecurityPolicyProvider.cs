using Lombiq.HelpfulLibraries.AspNetCore.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;

namespace Lombiq.HelpfulLibraries.OrchardCore.Security;

/// <summary>
/// Adds content security policy headers required by the Monaco editor to admin pages.
/// </summary>
public class MonacoContentSecurityPolicyProvider : IContentSecurityPolicyProvider
{
    public ValueTask UpdateAsync(IDictionary<string, string> securityPolicies, HttpContext context)
    {
        if (AdminAttribute.IsApplied(context) && IsEnabled(context))
        {
            AddMonacoPolicies(securityPolicies);
        }

        return ValueTask.CompletedTask;
    }

    private static bool IsEnabled(HttpContext context) =>
        !context
            .RequestServices
            .GetRequiredService<IOptions<ContentSecurityPolicyHeaderOptions>>()
            .Value
            .DisableMonacoContentSecurityPolicyProvider;

    public static void AddMonacoPolicies(IDictionary<string, string> securityPolicies)
    {
        CspHelper.MergeValues(securityPolicies, ScriptSrc, CommonValues.Blob, CommonValues.UnsafeEval);
        CspHelper.MergeValues(securityPolicies, WorkerSrc, CommonValues.Blob);
    }
}
