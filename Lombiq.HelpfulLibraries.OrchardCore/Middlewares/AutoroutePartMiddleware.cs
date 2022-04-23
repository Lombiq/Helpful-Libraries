using Microsoft.AspNetCore.Http;
using OrchardCore.ContentManagement.Routing;
using System;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Middlewares;

public class AutoroutePartMiddleware
{
    private const string BaseUrl = "/Contents/ContentItems/";

    private readonly RequestDelegate _next;

    public AutoroutePartMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, IAutorouteEntries autorouteEntries)
    {
        var url = context.Request.Path.ToString();
        if (!url.StartsWithOrdinalIgnoreCase(BaseUrl))
        {
            await _next(context);
            return;
        }

        var id = url.Replace(BaseUrl, string.Empty).Split('/')[0];
        var (success, entries) = await autorouteEntries.TryGetEntryByContentItemIdAsync(id);
        if (success) context.Response.Redirect(entries.Path);

        await _next(context);
    }
}
