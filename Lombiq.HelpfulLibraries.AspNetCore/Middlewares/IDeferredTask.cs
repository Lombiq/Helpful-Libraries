using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.AspNetCore.Middlewares;

/// <summary>
/// A task that can be executed in the return edge of the middleware pipeline.
/// </summary>
public interface IDeferredTask
{
    /// <summary>
    /// Gets or sets a value indicating whether the task was scheduled.
    /// </summary>
    bool IsScheduled { get; set; }

    /// <summary>
    /// A task executed on the starting edge of the middleware pipeline (i.e. this is not "deferred").
    /// </summary>
    Task PreProcessAsync(HttpContext context) => Task.CompletedTask;

    /// <summary>
    /// A task executed on the return edge of the middleware pipeline. It is guaranteed that all filters have concluded
    /// at this point. Most middlewares too, as they typically only do work before passing to the next element of the
    /// pipeline.
    /// </summary>
    Task PostProcessAsync(HttpContext context);
}
