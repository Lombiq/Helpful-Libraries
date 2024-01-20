using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.AspNetCore.Middlewares;

public class DeferredTaskMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        IEnumerable<IDeferredTask> deferredTasks)
    {
        var deferredTasksList = deferredTasks.AsList();

        foreach (var deferredTask in deferredTasksList)
        {
            deferredTask.IsScheduled = true;
            await deferredTask.PreProcessAsync(context);
        }

        await next(context);

        foreach (var deferredTask in deferredTasksList) await deferredTask.PostProcessAsync(context);
    }
}

public static class DeferredTaskApplicationBuilderExtensions
{
    /// <summary>
    /// Enables <see cref="DeferredTaskMiddleware"/>.
    /// </summary>
    public static void UseDeferredTasks(this IApplicationBuilder app) =>
        app.UseMiddleware<DeferredTaskMiddleware>();
}
