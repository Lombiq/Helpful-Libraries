using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.DisplayManagement;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OrchardCore;

public static class ContentOrchardHelperExtensions
{
    /// <summary>
    /// Gets the given content item's edit URL.
    /// </summary>
    public static async Task<string> GetItemEditUrlAsync(
        this IOrchardHelper orchardHelper,
        ContentItem contentItem)
    {
        var urlHelperFactory = orchardHelper.HttpContext.RequestServices.GetService<IUrlHelperFactory>();
        var viewContextAccessor = orchardHelper.HttpContext.RequestServices.GetService<ViewContextAccessor>();
        var contentManager = orchardHelper.HttpContext.RequestServices.GetService<IContentManager>();

        var urlHelper = urlHelperFactory.GetUrlHelper(viewContextAccessor.ViewContext);
        var metadata = await contentManager.PopulateAspectAsync<ContentItemMetadata>(contentItem);
        return urlHelper.Action(metadata.EditorRouteValues["action"].ToString(), metadata.EditorRouteValues);
    }

    /// <summary>
    /// Gets the given content item's display URL.
    /// </summary>
    public static async Task<string> GetItemDisplayUrlAsync(
        this IOrchardHelper orchardHelper,
        ContentItem contentItem)
    {
        var urlHelperFactory = orchardHelper.HttpContext.RequestServices.GetService<IUrlHelperFactory>();
        var viewContextAccessor = orchardHelper.HttpContext.RequestServices.GetService<ViewContextAccessor>();
        var contentManager = orchardHelper.HttpContext.RequestServices.GetService<IContentManager>();

        var urlHelper = urlHelperFactory.GetUrlHelper(viewContextAccessor.ViewContext);
        var metadata = await contentManager.PopulateAspectAsync<ContentItemMetadata>(contentItem);
        return urlHelper.Action(metadata.DisplayRouteValues["action"].ToString(), metadata.DisplayRouteValues);
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
}
