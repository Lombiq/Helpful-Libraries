using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.AspNetCore.Middlewares;

public class DeferredTaskMiddleware
{
    private readonly RequestDelegate _next;

    public DeferredTaskMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(
        HttpContext context,
        IEnumerable<IDeferredTask> deferredTasks)
    {
        foreach (var deferredTask in deferredTasks)
        {
            deferredTask.IsScheduled = true;
            await deferredTask.PreProcessAsync(context);
        }

        await _next(context);

        foreach (var deferredTask in deferredTasks) await deferredTask.PostProcessAsync(context);
    }
}

public static class DeferredTaskApplicationBuilderExtensions
{
    public static void UseDeferredTasks(this IApplicationBuilder app) =>
        app.UseMiddleware<DeferredTaskMiddleware>();
}
