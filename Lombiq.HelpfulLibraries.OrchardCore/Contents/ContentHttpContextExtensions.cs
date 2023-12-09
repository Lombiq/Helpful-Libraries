using Lombiq.HelpfulLibraries.OrchardCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Http;

public static class ContentHttpContextExtensions
{
    private const string ContentSessionDataInfix = "_SessionData_";

    /// <summary>
    /// Within the request, sets the specified <paramref name="content"/> object's data in dictionary format using the
    /// provided <paramref name="data"/> object.
    /// </summary>
    /// <param name="content">The content to be used as the key in the key value pair.</param>
    /// <param name="data">The data to set as the value in the key value pair.</param>
    public static void SetContentSessionData(this HttpContext httpContext, IContent content, object data) =>
        httpContext.Items[GetContentSessionDataKey(content)] = data;

    /// <summary>
    /// Retrieves <paramref name="content"/>'s session data if it already exists or creates a new entry if it does not.
    /// </summary>
    /// <param name="content">The content object whose data is to be retrieved or set.</param>
    public static T GetOrCreateContentSessionData<T>(this HttpContext httpContext, IContent content)
        where T : new()
    {
        if (httpContext.Items.GetMaybe(GetContentSessionDataKey(content)) is T data) return data;

        SetContentSessionData(httpContext, content, new T());
        return (T)httpContext.Items.GetMaybe(GetContentSessionDataKey(content));
    }

    /// <summary>
    /// Determines whether the request contains the specified <paramref name="content"/>'s data key.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the request contains the data key, <see langword="false"/> otherwise.
    /// </returns>
    public static bool ContainsContentSessionData(this HttpContext httpContext, IContent content) =>
        httpContext.Items.ContainsKey(GetContentSessionDataKey(content));

    /// <summary>
    /// Retrieves the data key of the given <paramref name="content"/> object.
    /// </summary>
    private static string GetContentSessionDataKey(IContent content) =>
        content.ContentItem.ContentType + ContentSessionDataInfix + content.ContentItem.ContentItemId;

    /// <summary>
    /// Returns a relative URL string to a controller action inside an Orchard Core module. Similar to <see
    /// cref="Action"/>, but it uses a strongly typed lambda <see cref="Expression"/> to select the action and populate
    /// the query arguments.
    /// </summary>
    public static string Action<TController>(
        this HttpContext httpContext,
        Expression<Action<TController>> actionExpression,
        params (string Key, object Value)[] additionalArguments)
        where TController : ControllerBase
    {
        var route = TypedRoute.CreateFromExpression(
            actionExpression,
            additionalArguments,
            httpContext.RequestServices);
        return route.ToString(httpContext);
    }

    /// <summary>
    /// Same as <see cref="Action{TController}"/>, but for actions returning a <see cref="Task{IActionResult}"/>.
    /// </summary>
    public static string ActionTask<TController>(
        this HttpContext httpContext,
        Expression<Func<TController, Task>> taskActionExpression,
        params (string Key, object Value)[] additionalArguments)
        where TController : ControllerBase =>
        httpContext.Action(taskActionExpression.StripResult(), additionalArguments);

    /// <summary>
    /// Returns the <see cref="ContentItem.ContentItemId"/> route value that's used e.g. in content display pages.
    /// </summary>
    public static string GetContentItemIdRouteValue(this HttpContext httpContext) =>
        httpContext?.Request.RouteValues.GetMaybe(nameof(ContentItem.ContentItemId))?.ToString();

    /// <summary>
    /// Returns a value indicating whether the current MVC route matches the provided <paramref name="area"/>, <paramref
    /// name="controller"/> and  <paramref name="action"/>.
    /// </summary>
    public static bool IsAction(this HttpContext httpContext, string area, string controller, string action) =>
        httpContext?.Request.RouteValues is { } routeValues &&
        routeValues.GetMaybe(nameof(area))?.ToString() == area &&
        routeValues.GetMaybe(nameof(controller))?.ToString() == controller &&
        routeValues.GetMaybe(nameof(action))?.ToString() == action;

    /// <summary>
    /// Returns a value indicating whether the current page is a content item display action.
    /// </summary>
    public static bool IsContentDisplay(this HttpContext httpContext) =>
        httpContext.IsAction("OrchardCore.Contents", "Item", "Display");
}
