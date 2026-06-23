using Lombiq.HelpfulLibraries.AspNetCore.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Contents.Controllers;
using OrchardCore.Mvc.Core.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;

namespace Lombiq.HelpfulLibraries.OrchardCore.Security;

/// <summary>
/// Adds content security policy headers required by the Monaco editor to certain admin pages that use it.
/// </summary>
public class MonacoContentSecurityPolicyProvider : IContentSecurityPolicyProvider
{
    public async ValueTask UpdateAsync(IDictionary<string, string> securityPolicies, HttpContext context)
    {
        if (IsContentEditor(context) || IsDeployment(context))
        {
            AddMonacoPolicies(securityPolicies);
        }
    }

    private static bool IsContentEditor(HttpContext context)
    {
        var adminControllerName = typeof(AdminController).ControllerName();
        return context.IsMvcRoute(nameof(AdminController.Create), adminControllerName, "OrchardCore.Contents") ||
            context.IsMvcRoute(nameof(AdminController.Edit), adminControllerName, "OrchardCore.Contents");
    }

    private static bool IsDeployment(HttpContext context) =>
        context.IsMvcRoute(area: "OrchardCore.Deployment");

    public static void AddMonacoPolicies(IDictionary<string, string> securityPolicies)
    {
        CspHelper.MergeValues(securityPolicies, ScriptSrc, CommonValues.Blob, CommonValues.UnsafeEval);
        CspHelper.MergeValues(securityPolicies, WorkerSrc, CommonValues.Blob);
    }
}
