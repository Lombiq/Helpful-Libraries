using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.DisplayManagement.Extensions;
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
    [SuppressMessage("Design", "CA1055:URI-like return values should not be strings", Justification = "It only returns a relative URL.")]
    [Obsolete($"Use {nameof(GetItemEditUrlAsync)} instead.")]
    public static string GetItemEditUrl(this IOrchardHelper orchardHelper, ContentItem contentItem) =>
        orchardHelper.GetItemEditUrl(contentItem.ContentItemId);

    /// <summary>
    /// Gets the given content item's edit URL.
    /// </summary>
    [SuppressMessage("Design", "CA1055:URI-like return values should not be strings", Justification = "It only returns a relative URL.")]
    [Obsolete($"Use {nameof(GetItemEditUrlAsync)} instead.")]
    public static string GetItemEditUrl(this IOrchardHelper orchardHelper, string contentItemId)
    {
        var urlHelper = orchardHelper.GetUrlHelper();
        return urlHelper.EditContentItem(contentItemId);
    }

    /// <summary>
    /// Gets the given content item's edit URL.
    /// </summary>
    [SuppressMessage("Design", "CA1055:URI-like return values should not be strings", Justification = "It only returns a relative URL.")]
    public static Task<string> GetItemEditUrlAsync(this IOrchardHelper orchardHelper, ContentItem contentItem) =>
        orchardHelper.GetItemEditUrlAsync(contentItem.ContentItemId);

    /// <summary>
    /// Gets the given content item's edit URL.
    /// </summary>
    [SuppressMessage("Design", "CA1055:URI-like return values should not be strings", Justification = "It only returns a relative URL.")]
    public static async Task<string> GetItemEditUrlAsync(this IOrchardHelper orchardHelper, string contentItemId)
    {
        var urlHelper = await orchardHelper.GetUrlHelperAsync();
        return urlHelper.EditContentItem(contentItemId);
    }

    /// <summary>
    /// Gets the given content item's display URL.
    /// </summary>
    [SuppressMessage("Design", "CA1055:URI-like return values should not be strings", Justification = "It only returns a relative URL.")]
    [Obsolete($"Use {nameof(GetItemDisplayUrlAsync)} instead.")]
    public static string GetItemDisplayUrl(this IOrchardHelper orchardHelper, string contentItemId)
    {
        var urlHelper = orchardHelper.GetUrlHelper();
        return urlHelper.DisplayContentItem(contentItemId);
    }

    /// <summary>
    /// Gets the given content item's display URL.
    /// </summary>
    [SuppressMessage("Design", "CA1055:URI-like return values should not be strings", Justification = "It only returns a relative URL.")]
    public static async Task<string> GetItemDisplayUrlAsync(this IOrchardHelper orchardHelper, string contentItemId)
    {
        var urlHelper = await orchardHelper.GetUrlHelperAsync();
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

        return httpContext.Request.GetFormValueMaybe("PreviewContentItemId") is { } previewContentItemId &&
            !string.IsNullOrEmpty(previewContentItemId) &&
            httpContext.RequestServices.GetService<IContentManager>() is { } contentManager
                ? contentManager.GetAsync(previewContentItemId)
                : contentItemGetter();
    }

    /// <inheritdoc cref="ContentHttpContextExtensions.Action{TController}"/>
    public static string Action<TController>(
        this IOrchardHelper orchardHelper,
        Expression<Action<TController>> actionExpression,
        params (string Key, object? Value)[] additionalArguments)
        where TController : ControllerBase =>
        orchardHelper.HttpContext.Action(actionExpression, additionalArguments);

    /// <inheritdoc cref="ContentHttpContextExtensions.Action{TController}"/>
    public static string Action<TController>(
        this IOrchardHelper orchardHelper,
        Expression<Func<TController, Task>> taskActionExpression,
        params (string Key, object? Value)[] additionalArguments)
        where TController : ControllerBase =>
        orchardHelper.HttpContext.Action(taskActionExpression.StripResult(), additionalArguments);

    /// <summary>
    /// Constructs a new <see cref="IUrlHelper"/> instance using the current <see cref="IOrchardHelper.HttpContext"/>.
    /// </summary>
    [Obsolete($"Use {nameof(GetUrlHelperAsync)} instead.")]
    public static IUrlHelper GetUrlHelper(this IOrchardHelper orchardHelper) =>
        orchardHelper.GetUrlHelperAsync().Result;

    /// <summary>
    /// Constructs a new <see cref="IUrlHelper"/> instance using the current <see cref="IOrchardHelper.HttpContext"/>.
    /// </summary>
    public static async Task<IUrlHelper> GetUrlHelperAsync(this IOrchardHelper orchardHelper)
    {
        var serviceProvider = orchardHelper.HttpContext.RequestServices;
        var urlHelperFactory = serviceProvider.GetRequiredService<IUrlHelperFactory>();

        var actionContext = await orchardHelper.HttpContext.GetActionContextAsync() ??
            throw new InvalidOperationException("Couldn't access the action context.");

        return urlHelperFactory.GetUrlHelper(actionContext);
    }

    /// <summary>
    /// Returns <c>ocat-label</c> or <c>ocat-label ocat-label-required</c> depending on the <paramref name="settings"/>.
    /// This is a simplified version of a removed stock Orchard Core helper, only for content fields.
    /// </summary>
    public static string GetLabelClasses(this IOrchardHelper orchardHelper, FieldSettings settings) =>
        settings.Required ? "ocat-label ocat-label-required" : "ocat-label";
}
