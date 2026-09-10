using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.Admin;
using OrchardCore.Modules;
using System;

namespace Lombiq.HelpfulLibraries.OrchardCore.Mvc;

public static class HttpContextExtensions
{
    /// <summary>
    /// Returns whether the current request is for an admin URL.
    /// </summary>
    public static bool IsAdminUrl(this HttpContext context)
    {
        var adminOptions = context.RequestServices.GetRequiredService<IOptions<AdminOptions>>();

        return context.Request.Path.Value?.StartsWithOrdinalIgnoreCase(value: "/" + adminOptions.Value.AdminUrlPrefix) == true;
    }

    public static IResult ChallengeOrForbidApi(this HttpContext httpContext) =>
        httpContext.ChallengeOrForbid("Api");

    /// <summary>
    /// Creates a new instance of <see cref="ActionContext"/> based on <paramref name="httpContext"/>, but with an empty
    /// <see cref="RouteData"/>. This has limited usability (e.g. it's fine when used to create <see cref="IUrlHelper"/>
    /// that will be only used to call <see cref="IUrlHelper.Content"/>), but for more complex use cases Orchard Core's
    /// built-in <c>GetActionContextAsync</c> extension method should be used.
    /// </summary>
    public static ActionContext CreateActionContextWithoutRouteData(this HttpContext httpContext)
    {
        var routeData = new RouteData();
        routeData.Routers.Add(new RouteCollection());

        return new ActionContext(httpContext, routeData, new ActionDescriptor());
    }
}
