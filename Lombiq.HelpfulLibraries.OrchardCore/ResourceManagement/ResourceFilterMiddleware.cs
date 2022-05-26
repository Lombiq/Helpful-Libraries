using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Admin;
using OrchardCore.ResourceManagement;
using OrchardCore.Themes.Services;
using System;
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
        var providers = context
            .RequestServices
            .GetRequiredService<IEnumerable<IResourceFilterProvider>>()
            .Select(provider => new { Provider = provider, ThemeRequirements = provider.GetRequiredThemes().ToList() })
            .ToList();

        IList<string> themes =
            providers.Any(providerInfo => providerInfo.ThemeRequirements.Any())
                ? new[]
                    {
                        await context.RequestServices.GetRequiredService<ISiteThemeService>().GetSiteThemeAsync(),
                        await context.RequestServices.GetRequiredService<IAdminThemeService>().GetAdminThemeAsync(),
                    }
                    .Where(info => info != null)
                    .Select(info => info.Id)
                    .ToList()
                : Array.Empty<string>();

        var builder = new ResourceFilterBuilder();
        var anyProviders = providers
            .Where(providerInfo => providerInfo.ThemeRequirements.Any(themes.Contains))
            .ForEach(providerInfo => providerInfo.Provider.AddResourceFilter(builder));

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
