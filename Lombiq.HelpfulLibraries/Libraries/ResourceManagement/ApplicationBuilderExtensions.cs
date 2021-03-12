using Microsoft.AspNetCore.Builder;

namespace Lombiq.HelpfulLibraries.Libraries.ResourceManagement
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseResourceFilters(this IApplicationBuilder app) => app.UseMiddleware<ResourceFilterMiddleware>();
    }
}
