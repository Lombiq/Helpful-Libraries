using Microsoft.AspNetCore.Http;
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

}
