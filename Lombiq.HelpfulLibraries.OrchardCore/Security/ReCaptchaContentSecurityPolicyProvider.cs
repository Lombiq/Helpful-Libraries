using Lombiq.HelpfulLibraries.AspNetCore.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Environment.Shell;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;

namespace Lombiq.HelpfulLibraries.OrchardCore.Security;

internal sealed class ReCaptchaContentSecurityPolicyProvider : IContentSecurityPolicyProvider
{
    public async ValueTask UpdateAsync(IDictionary<string, string> securityPolicies, HttpContext context)
    {
        var shellFeaturesManager = context.RequestServices.GetRequiredService<IShellFeaturesManager>();
        var reCaptchaIsEnabled = (await shellFeaturesManager.GetEnabledFeaturesAsync())
           .Any(feature => feature.Id == "OrchardCore.ReCaptcha");

        if (reCaptchaIsEnabled)
        {
            CspHelper.MergeValues(securityPolicies, ScriptSrc, "www.google.com", "www.gstatic.com");
            CspHelper.MergeValues(securityPolicies, FrameSrc, "www.google.com");
        }
    }
}
