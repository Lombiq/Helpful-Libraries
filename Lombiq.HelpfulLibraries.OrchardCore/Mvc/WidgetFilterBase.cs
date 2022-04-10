using Lombiq.HelpfulLibraries.OrchardCore.Contents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using OrchardCore.Admin;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Layout;
using OrchardCore.Security.Permissions;
using System;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Mvc;

/// <summary>
/// A base class for filters that can display a widget in the Zone indicated by <see cref="ZoneName"/>.
/// </summary>
/// <typeparam name="TViewModel">The type of the view-model which is passed to the widget.</typeparam>
public abstract class WidgetFilterBase<TViewModel> : IAsyncResultFilter
{
    private readonly Permission _requiredPermission;
    private readonly IAuthorizationService _authorizationService;
    private readonly ILayoutAccessor _layoutAccessor;
    private readonly IShapeFactory _shapeFactory;

    /// <summary>
    /// Gets the name of the zone where the widget may be displayed.
    /// </summary>
    protected abstract string ZoneName { get; }

    /// <summary>
    /// Gets the name of the view/shape that may be displayed.
    /// </summary>
    protected abstract string ViewName { get; }

    /// <summary>
    /// Gets a value indicating whether the widget only shows up in routes with <see cref="AdminAttribute"/>.
    /// </summary>
    protected virtual bool AdminOnly => false;

    /// <summary>
    /// Gets a value indicating whether the widget only shows up in routes with no <see cref="AdminAttribute"/>.
    /// </summary>
    protected virtual bool FrontEndOnly => false;

    /// <summary>
    /// Initializes a new instance of the <see cref="WidgetFilterBase{T}"/> class. The <paramref
    /// name="requiredPermission"/> must be specified by the derived class, the rest should be provided through
    /// dependency injection.
    /// </summary>
    protected WidgetFilterBase(
        Permission requiredPermission,
        IAuthorizationService authorizationService,
        ILayoutAccessor layoutAccessor,
        IShapeFactory shapeFactory)
    {
        _requiredPermission = requiredPermission;
        _authorizationService = authorizationService;
        _layoutAccessor = layoutAccessor;
        _shapeFactory = shapeFactory;
    }

    /// <summary>
    /// Returns the object used as the view-model and passed to the widget shape.
    /// </summary>
    protected abstract Task<TViewModel> GetViewModelAsync();

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context?.HttpContext == null || context.IsNotFullViewRendering())
        {
            await next();
            return;
        }

        if (AdminOnly && FrontEndOnly)
        {
            throw new InvalidOperationException(FormattableString.Invariant(
                $"You must not set both {nameof(AdminOnly)} and {nameof(FrontEndOnly)} to true!"));
        }

        var isAdmin = AdminAttribute.IsApplied(context.HttpContext);
        var user = context.HttpContext?.User;
        if ((AdminOnly && !isAdmin) ||
            (FrontEndOnly && isAdmin) ||
            (_requiredPermission != null && !await _authorizationService.AuthorizeAsync(user, _requiredPermission)))
        {
            await next();
            return;
        }

        var viewModel = await GetViewModelAsync();
        await _layoutAccessor.AddShapeToZoneAsync(ZoneName, await _shapeFactory.CreateAsync(ViewName, viewModel));

        await next();
    }
}
