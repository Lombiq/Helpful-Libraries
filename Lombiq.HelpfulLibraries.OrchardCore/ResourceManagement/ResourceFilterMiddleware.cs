using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Admin;
using OrchardCore.DisplayManagement.Extensions;
using OrchardCore.DisplayManagement.Manifest;
using OrchardCore.Environment.Shell;
using OrchardCore.ResourceManagement;
using OrchardCore.Themes.Services;
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
        var services = context.RequestServices;
        var providers = services
            .GetRequiredService<IEnumerable<IResourceFilterProvider>>()
            .Select(provider => new
            {
                Provider = provider,
                ThemeRequirements = provider.GetRequiredThemes().Concat(provider.RequiredThemes).ToList(),
            })
            .ToList();

        IList<string> themeIds = [];
        if (providers.Exists(providerInfo => providerInfo.ThemeRequirements.Count > 0))
        {
            var themes = (await services.GetRequiredService<IShellFeaturesManager>().GetAvailableFeaturesAsync())
                .SelectWhere(feature => feature.Extension as IThemeExtensionInfo)
                .ToDictionary(info => info.Id);
            themeIds = new[]
            {
                (await services.GetRequiredService<ISiteThemeService>().GetSiteThemeAsync())?.Id,
                (await services.GetRequiredService<IAdminThemeService>().GetAdminThemeAsync())?.Id,
            }
            .SelectMany(id => GetThemeAndBaseIds(themes, id))
            .ToList();
        }

        var builder = new ResourceFilterBuilder();
        var anyProviders = providers
            .Where(providerInfo => providerInfo.ThemeRequirements.Count == 0 ||
                                   providerInfo.ThemeRequirements.Exists(themeIds.Contains))
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

            foreach (var filter in activeFilters)
            {
                resourceManager ??= services.GetRequiredService<IResourceManager>();

                await filter.ApplyAsync(resourceManager);
            }
        }

        await _next(context);
    }

    private static IEnumerable<string> GetThemeAndBaseIds(Dictionary<string, IThemeExtensionInfo> themes, string id)
    {
        if (string.IsNullOrEmpty(id) || !themes.TryGetValue(id, out var info))
        {
            return [];
        }

        var baseThemeId = (info.Manifest.ModuleInfo as ThemeAttribute)?.BaseTheme;
        return string.IsNullOrEmpty(baseThemeId)
            ? [id]
            : [id, ..GetThemeAndBaseIds(themes, baseThemeId)];
    }
}
