using Lombiq.HelpfulLibraries.AspNetCore.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Environment.Shell;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;

namespace Lombiq.HelpfulLibraries.OrchardCore.Security;

internal sealed class ExternalLoginContentSecurityPolicyProvider : IContentSecurityPolicyProvider
{
    public async ValueTask UpdateAsync(IDictionary<string, string> securityPolicies, HttpContext context)
    {
        var shellFeaturesManager = context.RequestServices.GetRequiredService<IShellFeaturesManager>();
        var enabledFeatures = await shellFeaturesManager.GetEnabledFeaturesAsync();

        if (enabledFeatures.Any("OrchardCore.Microsoft.Authentication.AzureAD"))
        {
            CspHelper.MergeValues(securityPolicies, FormAction, "login.microsoftonline.com"); // #spell-check-ignore-line
        }

        if (enabledFeatures.Any("OrchardCore.GitHub.Authentication"))
        {
            CspHelper.MergeValues(securityPolicies, FormAction, "github.com");
        }
    }
}
