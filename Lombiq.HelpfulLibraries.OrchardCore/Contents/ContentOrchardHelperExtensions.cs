using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OrchardCore;

public static class ContentOrchardHelperExtensions
{
    /// <summary>
    /// Gets the given content item's edit URL.
    /// </summary>
    [Obsolete($"Use {nameof(GetItemEditUrl)} instead as this method does not need to be async.")]
    public static Task<string> GetItemEditUrlAsync(this IOrchardHelper orchardHelper, ContentItem contentItem) =>
        Task.FromResult(orchardHelper.GetItemEditUrl(contentItem));

    /// <summary>
    /// Gets the given content item's edit URL.
    /// </summary>
    [SuppressMessage("Design", "CA1055:URI-like return values should not be strings", Justification = "It only returns relative URL.")]
    public static string GetItemEditUrl(this IOrchardHelper orchardHelper, ContentItem contentItem) =>
        orchardHelper.GetItemEditUrl(contentItem?.ContentItemId);

    /// <summary>
    /// Gets the given content item's edit URL.
    /// </summary>
    [SuppressMessage("Design", "CA1055:URI-like return values should not be strings", Justification = "It only returns relative URL.")]
    public static string GetItemEditUrl(this IOrchardHelper orchardHelper, string contentItemId)
    {
        var urlHelper = orchardHelper.GetUrlHelper();
        return urlHelper.EditContentItem(contentItemId);
    }

    /// <summary>
    /// Gets the given content item's display URL.
    /// </summary>
    [SuppressMessage("Design", "CA1055:URI-like return values should not be strings", Justification = "It only returns relative URL.")]
    public static string GetItemDisplayUrl(this IOrchardHelper orchardHelper, string contentItemId)
    {
        var urlHelper = orchardHelper.GetUrlHelper();
        return urlHelper.DisplayContentItem(contentItemId);
    }

    /// <summary>
    /// Runs a getter delegate to get a content item or loads the item currently viewed via Content Preview.
    /// </summary>
    /// <remarks>
    /// <para>This is useful when supporting preview in a decoupled scenario.</para>
    /// </remarks>
    public static Task<ContentItem> GetContentItemOrPreviewAsync(
        this IOrchardHelper orchardHelper,
        Func<Task<ContentItem>> contentItemGetter)
    {
        var httpContext = orchardHelper.HttpContext;

        if (httpContext.Request.Method == "POST")
        {
            var previewContentItemId = httpContext.Request.Form["PreviewContentItemId"];
            if (!string.IsNullOrEmpty(previewContentItemId))
            {
                return httpContext.RequestServices.GetService<IContentManager>().GetAsync(previewContentItemId);
            }
        }

        return contentItemGetter();
    }

    /// <inheritdoc cref="ContentHttpContextExtensions.Action{TController}"/>
    public static string Action<TController>(
        this IOrchardHelper orchardHelper,
        Expression<Action<TController>> actionExpression,
        params (string Key, object Value)[] additionalArguments)
        where TController : ControllerBase =>
        orchardHelper.HttpContext.Action(actionExpression, additionalArguments);

    /// <inheritdoc cref="ContentHttpContextExtensions.Action{TController}"/>
    public static string Action<TController>(
        this IOrchardHelper orchardHelper,
        Expression<Func<TController, Task>> taskActionExpression,
        params (string Key, object Value)[] additionalArguments)
        where TController : ControllerBase =>
        orchardHelper.HttpContext.Action(taskActionExpression.StripResult(), additionalArguments);

    /// <summary>
    /// Constructs a new <see cref="IUrlHelper"/> instance using the current <see cref="IOrchardHelper.HttpContext"/>.
    /// </summary>
    public static IUrlHelper GetUrlHelper(this IOrchardHelper orchardHelper)
    {
        var serviceProvider = orchardHelper.HttpContext.RequestServices;
        var urlHelperFactory = serviceProvider.GetService<IUrlHelperFactory>();
        var actionContext = serviceProvider.GetService<IActionContextAccessor>()?.ActionContext ??
            throw new InvalidOperationException("Couldn't access the action context.");

        return urlHelperFactory.GetUrlHelper(actionContext);
    }
}
