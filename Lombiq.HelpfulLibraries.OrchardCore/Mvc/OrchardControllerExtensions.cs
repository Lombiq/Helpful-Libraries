using Lombiq.HelpfulLibraries.AspNetCore.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OrchardCore.ContentManagement;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc;

public static class OrchardControllerExtensions
{
    /// <summary>
    /// Uses <see cref="Routing.UrlHelperExtensions.DisplayContentItem(IUrlHelper,IContent)"/>
    /// extension method to redirect to this <see cref="ContentItem"/>'s display page.
    /// </summary>
    public static RedirectResult RedirectToContentDisplay(this Controller controller, IContent content) =>
        controller.RedirectToContentDisplay(content.ContentItem.ContentItemId);

    /// <inheritdoc cref="RedirectToContentDisplay(Microsoft.AspNetCore.Mvc.Controller,OrchardCore.ContentManagement.IContent)"/>
    public static RedirectResult RedirectToContentDisplay(this Controller controller, string contentItemId) =>
        controller.Redirect(controller.Url.DisplayContentItem(contentItemId));

    /// <summary>
    /// Uses <see cref="Routing.UrlHelperExtensions.EditContentItem"/> extension method to redirect to this <see
    /// cref="ContentItem"/>'s editor page.
    /// </summary>
    public static RedirectResult RedirectToContentEdit(this Controller controller, IContent content) =>
        controller.RedirectToContentEdit(content.ContentItem.ContentItemId);

    /// <inheritdoc cref="RedirectToContentEdit(Microsoft.AspNetCore.Mvc.Controller,string)"/>
    public static RedirectResult RedirectToContentEdit(this Controller controller, string contentItemId) =>
        controller.Redirect(controller.Url.EditContentItem(contentItemId));

    /// <summary>
    /// Similar to <c>controller.Json(data)</c>, but catches any exception in the <paramref name="dataFactory"/> and if
    /// one happens returns a JSON with the <c>error</c> property. If run from a local dev machine the <c>data</c>
    /// property is also filled with the exception string.
    /// </summary>
    public static async Task<JsonResult> SafeJsonAsync<T>(this Controller controller, Func<Task<T>> dataFactory)
    {
        var context = controller.HttpContext;

        try
        {
            return controller.Json(await dataFactory());
        }
        catch (FrontendException exception)
        {
            LogJsonError(controller, exception);
            return controller.Json(new
            {
                error = exception.Message,
                html = exception.HtmlMessages.Select(message => message.Html()),
                data = context.IsDevelopmentAndLocalhost(),
            });
        }
        catch (Exception exception)
        {
            LogJsonError(controller, exception);
            return controller.Json(context.IsDevelopmentAndLocalhost()
                ? new { error = exception.Message, data = exception.ToString() }
                : new
                {
                    error = context
                        .RequestServices
                        .GetService<IStringLocalizerFactory>()?
                        .Create(controller.GetType())["An error has occurred while trying to process your request."]
                        .Value ?? "An error has occurred while trying to process your request.",
                });
        }
    }

    private static void LogJsonError(Controller controller, Exception exception)
    {
        var context = controller.HttpContext;

        var logger = context
            .RequestServices
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger(controller.GetType());
        logger.LogError(
            exception,
            "An error has occurred while generating a JSON result. (Request Route Values: {RouteValues})",
            JsonSerializer.Serialize(context.Request.RouteValues));
    }
}
