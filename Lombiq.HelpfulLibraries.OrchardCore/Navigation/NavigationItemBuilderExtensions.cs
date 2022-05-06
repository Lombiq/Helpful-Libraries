using Lombiq.HelpfulLibraries.OrchardCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OrchardCore.Environment.Extensions;
using OrchardCore.Navigation;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Navigation;

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

    /// <summary>
    /// Adds a menu item that behaves like a separator (horizontal line) in the MenuWidget.
    /// </summary>
    public static NavigationBuilder AddSeparator(this NavigationBuilder builder, IStringLocalizer localizer) =>
        builder.AddLabel(localizer["---"]);

    /// <summary>
    /// Adds a menu item that has no link and the "disabled" and "menuWidget__link_label" classes in the MenuWidget.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The "disabled" class is necessary so Bootstrap won't mark it as "active" on the home page. This also greys out
    /// the text by default. Themes derived from Lombiq.BaseTheme automatically remove this effect.
    /// </para>
    /// </remarks>
    public static NavigationBuilder AddLabel(this NavigationBuilder builder, LocalizedString label) =>
        builder.Add(label, subMenu => subMenu.Url("#").AddClass("disabled menuWidget__link_label"));
}
