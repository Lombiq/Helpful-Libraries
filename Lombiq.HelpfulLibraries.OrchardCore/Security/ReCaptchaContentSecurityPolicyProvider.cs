using Lombiq.HelpfulLibraries.AspNetCore.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Environment.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Security;

internal class ReCaptchaContentSecurityPolicyProvider : IContentSecurityPolicyProvider
{
    public async ValueTask UpdateAsync(IDictionary<string, string> securityPolicies, HttpContext context)
    {
        var shellFeaturesManager = context.RequestServices.GetRequiredService<IShellFeaturesManager>();
        var reCaptchaIsEnabled = (await shellFeaturesManager.GetEnabledFeaturesAsync())
           .Any(feature => feature.Id == "OrchardCore.ReCaptcha");

        if (reCaptchaIsEnabled)
        {
            CspHelper.MergeValues(
                securityPolicies,
                ContentSecurityPolicyDirectives.ScriptSrc,
                new Uri("https://www.google.com/"),
                new Uri("https://www.gstatic.com/"));

            CspHelper.MergeValues(
                securityPolicies,
                ContentSecurityPolicyDirectives.FrameSrc,
                new Uri("https://www.google.com/"));
        }
    }
}
