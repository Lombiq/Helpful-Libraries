using Microsoft.AspNetCore.Builder;

namespace Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Activates <see cref="ResourceFilterMiddleware"/>.
    /// </summary>
    public static void UseResourceFilters(this IApplicationBuilder app) => app.UseMiddleware<ResourceFilterMiddleware>();
}
