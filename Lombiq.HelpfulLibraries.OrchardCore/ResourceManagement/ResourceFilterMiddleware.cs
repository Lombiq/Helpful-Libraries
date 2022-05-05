using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ResourceManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;

public class ResourceFilterMiddleware
{
    private readonly RequestDelegate _next;

    public ResourceFilterMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var builder = new ResourceFilterBuilder();
        var anyProviders = context
            .RequestServices
            .GetService<IEnumerable<IResourceFilterProvider>>()
            .ForEach(provider => provider.AddResourceFilter(builder));

        if (anyProviders)
        {
            IResourceManager resourceManager = null;

            var activeFilters = await builder
                .ResourceFilters
                .Where(filter => filter.Filter != null || filter.FilterAsync != null)
                .WhereAsync(
                    filter => filter.Filter != null
                        ? Task.FromResult(filter.Filter(context))
                        : filter.FilterAsync(context));

            activeFilters.ForEach(
                filter => filter.Execution(resourceManager),
                _ => resourceManager = context.RequestServices.GetService<IResourceManager>());
        }

        await _next(context);
    }
}
