using Lombiq.HelpfulLibraries.OrchardCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Http;

public static class ContentHttpContextExtensions
{
    private const string ContentSessionDataInfix = "_SessionData_";

    public static void SetContentSessionData(this HttpContext httpContext, IContent content, object data) =>
        httpContext.Items[GetContentSessionDataKey(content)] = data;

    public static T GetOrCreateContentSessionData<T>(this HttpContext httpContext, IContent content)
        where T : new()
    {
        if (httpContext.Items.GetMaybe(GetContentSessionDataKey(content)) is T data) return data;

        SetContentSessionData(httpContext, content, new T());
        return (T)httpContext.Items.GetMaybe(GetContentSessionDataKey(content));
    }

    public static bool ContainsContentSessionData(this HttpContext httpContext, IContent content) =>
        httpContext.Items.ContainsKey(GetContentSessionDataKey(content));

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
        var provider = httpContext.RequestServices.GetService<ITypeFeatureProvider>();
        var route = TypedRoute.CreateFromExpression(
            actionExpression,
            additionalArguments,
            provider);
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
}
