using Lombiq.HelpfulLibraries.AspNetCore.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives.CommonValues;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Indicates that the action's view should have the <c>script-src: unsafe-eval</c> content security policy directive.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class ScriptUnsafeEvalAttribute : Attribute
{
}

public class ScriptUnsafeEvalAttributeContentSecurityPolicyProvider : IContentSecurityPolicyProvider
{
    public ValueTask UpdateAsync(IDictionary<string, string> securityPolicies, HttpContext context)
    {
        if (context.RequestServices.GetService<IActionContextAccessor>() is { ActionContext: { } actionContext } &&
            actionContext.ActionDescriptor is ControllerActionDescriptor actionDescriptor &&
            actionDescriptor.MethodInfo.GetCustomAttributes<ScriptUnsafeEvalAttribute>().Any())
        {
            securityPolicies[ScriptSrc] = IContentSecurityPolicyProvider
                .GetDirective(securityPolicies, ScriptSrc)
                .MergeWordSets(UnsafeEval);
        }

        return ValueTask.CompletedTask;
    }
}
