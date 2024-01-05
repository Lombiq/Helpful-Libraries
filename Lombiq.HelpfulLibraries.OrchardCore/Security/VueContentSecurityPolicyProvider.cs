using Lombiq.HelpfulLibraries.AspNetCore.Security;
using Microsoft.AspNetCore.Http;
using OrchardCore.ResourceManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives.CommonValues;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Enable the <see cref="UnsafeEval"/> value for the <see cref="ScriptSrc"/> directive. This is necessary to evaluate
/// dynamic (not precompiled) templates. These are extensively used in stock Orchard Core. Also in many third party
/// modules where the DOM HTML template may contain Razor generated content.
/// </summary>
public class VueContentSecurityPolicyProvider : IContentSecurityPolicyProvider
{
    private readonly IResourceManager _resourceManager;

    public VueContentSecurityPolicyProvider(IResourceManager resourceManager) =>
        _resourceManager = resourceManager;

    public ValueTask UpdateAsync(IDictionary<string, string> securityPolicies, HttpContext context)
    {
        if (_resourceManager.GetRequiredResources("script").Any(script => script.Resource.Name == "vuejs"))
        {
            securityPolicies[ScriptSrc] += ' ' + UnsafeEval;
        }

        return ValueTask.CompletedTask;
    }
}
