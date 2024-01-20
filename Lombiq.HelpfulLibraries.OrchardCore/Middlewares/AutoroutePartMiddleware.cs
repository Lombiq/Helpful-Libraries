using Microsoft.AspNetCore.Http;
using OrchardCore.ContentManagement.Routing;
using System;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Middlewares;

public class AutoroutePartMiddleware(RequestDelegate next)
{
    private const string BaseUrl = "/Contents/ContentItems/";

    public async Task InvokeAsync(HttpContext context, IAutorouteEntries autorouteEntries)
    {
        var url = context.Request.Path.ToString();
        if (!url.StartsWithOrdinalIgnoreCase(BaseUrl))
        {
            await next(context);
            return;
        }

        var id = url.Replace(BaseUrl, string.Empty).Split('/')[0];
        var (success, entries) = await autorouteEntries.TryGetEntryByContentItemIdAsync(id);
        if (success) context.Response.Redirect(entries.Path);

        await next(context);
    }
}
