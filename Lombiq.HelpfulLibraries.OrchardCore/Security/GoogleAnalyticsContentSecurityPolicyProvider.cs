using Lombiq.HelpfulLibraries.AspNetCore.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Environment.Shell;
using System.Collections.Generic;
using System.Threading.Tasks;

using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;

namespace Lombiq.HelpfulLibraries.OrchardCore.Security;

public class GoogleAnalyticsContentSecurityPolicyProvider : IContentSecurityPolicyProvider
{
    private const string HttpContextItemKey = nameof(GoogleAnalyticsContentSecurityPolicyProvider);

    public async ValueTask UpdateAsync(IDictionary<string, string> securityPolicies, HttpContext context)
    {
        var googleAnalyticsIsEnabled = context.Items.ContainsKey(HttpContextItemKey);

        if (!googleAnalyticsIsEnabled)
        {
            var shellFeaturesManager = context.RequestServices.GetRequiredService<IShellFeaturesManager>();
            googleAnalyticsIsEnabled = await shellFeaturesManager.IsFeatureEnabledAsync("OrchardCore.Google.Analytics");
        }

        if (googleAnalyticsIsEnabled)
        {
            CspHelper.MergeValues(securityPolicies, ScriptSrc, "www.googletagmanager.com");
            CspHelper.MergeValues(securityPolicies, ConnectSrc, "*.google-analytics.com");
        }
    }

    /// <summary>
    /// Enable the CSP headers necessary for Google Analytics, even if the <c>OrchardCore.Google.Analytics</c> module is
    /// not enabled.
    /// </summary>
    /// <param name="context">The current <see cref="HttpContext"/>.</param>
    public static void EnableForCurrentRequest(HttpContext context) => context.Items[HttpContextItemKey] = "enabled";
}
