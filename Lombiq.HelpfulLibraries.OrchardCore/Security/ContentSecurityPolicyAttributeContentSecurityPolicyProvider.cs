using Lombiq.HelpfulLibraries.AspNetCore.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives.CommonValues;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Indicates that the action's view should have the <c>script-src: unsafe-eval</c> content security policy directive.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class ScriptUnsafeEvalAttribute : ContentSecurityPolicyAttribute
{
    public ScriptUnsafeEvalAttribute()
        : base(UnsafeEval, ScriptSrc)
    {
    }
}

/// <summary>
/// Indicates that the action's view should have the provided content security policy directive.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
[SuppressMessage(
    "Performance",
    "CA1813:Avoid unsealed attributes",
    Justification = $"Inherited by {nameof(ScriptUnsafeEvalAttribute)}.")]
public class ContentSecurityPolicyAttribute(string directiveValue, params string[] directiveNames) : Attribute
{
    /// <summary>
    /// Gets the fallback chain of the directive, excluding <see cref="DefaultSrc"/>. This is used to get the current
    /// value.
    /// </summary>
    public string[] DirectiveNames { get; } = directiveNames;

    /// <summary>
    /// Gets the value to be added to the directive. The content is split into words and added to the current values
    /// without repetition.
    /// </summary>
    public string DirectiveValue { get; } = directiveValue;
}

/// <summary>
/// Updates the content security policy based on <see cref="ContentSecurityPolicyAttribute"/> applied to the MVC action.
/// </summary>
public class ContentSecurityPolicyAttributeContentSecurityPolicyProvider : IContentSecurityPolicyProvider
{
    public ValueTask UpdateAsync(IDictionary<string, string> securityPolicies, HttpContext context)
    {
        if (context.RequestServices.GetService<IActionContextAccessor>() is
            { ActionContext.ActionDescriptor: ControllerActionDescriptor actionDescriptor })
        {
            foreach (var attribute in actionDescriptor.MethodInfo.GetCustomAttributes<ContentSecurityPolicyAttribute>())
            {
                securityPolicies[ScriptSrc] = IContentSecurityPolicyProvider
                    .GetDirective(securityPolicies, attribute.DirectiveNames)
                    .MergeWordSets(attribute.DirectiveValue);
            }
        }

        return ValueTask.CompletedTask;
    }
}
