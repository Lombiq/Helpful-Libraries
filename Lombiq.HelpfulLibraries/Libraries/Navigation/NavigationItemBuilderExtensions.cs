using Lombiq.HelpfulLibraries.Libraries.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Environment.Extensions;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OrchardCore.Navigation;
public static class NavigationItemBuilderExtensions
{
    /// <summary>
    /// Adds a link to the menu item using <see cref="TypedRoute"/>.
    /// </summary>
    public static NavigationItemBuilder ActionTask<TContext>(
        this NavigationItemBuilder builder,
        HttpContext httpContext,
        Expression<Func<TContext, Task<IActionResult>>> actionExpression,
        params (string Key, object Value)[] additionalArguments)
        where TContext : ControllerBase =>
        builder.Action(httpContext, actionExpression.StripResult(), additionalArguments);

    /// <summary>
    /// Adds a link to the menu item using <see cref="TypedRoute"/>.
    /// </summary>
    public static NavigationItemBuilder Action<TContext>(
        this NavigationItemBuilder builder,
        HttpContext httpContext,
        Expression<Action<TContext>> actionExpression,
        params (string Key, object Value)[] additionalArguments)
        where TContext : ControllerBase
    {
        var provider = httpContext.RequestServices.GetService<ITypeFeatureProvider>();
        var route = TypedRoute.CreateFromExpression(
            actionExpression,
            additionalArguments,
            provider);

        return builder.Action(route);
    }
}
