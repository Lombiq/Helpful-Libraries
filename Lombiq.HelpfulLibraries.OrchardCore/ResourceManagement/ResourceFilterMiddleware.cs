using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.Admin;
using OrchardCore.DisplayManagement.Extensions;
using OrchardCore.DisplayManagement.Manifest;
using OrchardCore.Environment.Shell;
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

    public async Task InvokeAsync(HttpContext context, IMemoryCache memoryCache)
    {
        var services = context.RequestServices;
        var providers = services
            .GetRequiredService<IEnumerable<IResourceFilterProvider>>()
            .Select(provider => new ProviderInfo(
                provider,
                provider.GetRequiredThemes().Concat(provider.RequiredThemes).ToList()))
            .ToList();

        IList<string> themeIds = [];
        if (providers.Exists(providerInfo => providerInfo.ThemeRequirements.Count > 0))
        {
            // Without caching, this would issue dozens, if not hundreds of database calls for the Default tenant like
            // below on each request.
            // SELECT TOP (1) [Document].* FROM [Document] WHERE [Document].[Type] = @Type
            var themes = await memoryCache.GetOrCreateAsync(
                typeof(ResourceFilterMiddleware).FullName + ".Themes",
                async entry =>
                    // No options needed for the cache entry since ideally its kept for the lifetime of the shell, but can
                    // be evicted any time.
                    (await services.GetRequiredService<IShellFeaturesManager>().GetAvailableFeaturesAsync())
                        .SelectWhere(feature => feature.Extension as IThemeExtensionInfo)
                        .ToDictionary(info => info.Id));

            // This is necessary to determine if we are in admin mode, because AdminZoneFilter won't have executed yet
            // by this point of the pipeline.
            var adminPrefix = (services.GetService<IOptions<AdminOptions>>()?.Value ?? new AdminOptions()).AdminUrlPrefix ?? "Admin";
            var isAdmin = adminPrefix.EqualsOrdinalIgnoreCase(
                context.Request.Path.ToString().Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault());

            var themeName = isAdmin
                ? (await services.GetRequiredService<IAdminThemeService>().GetAdminThemeAsync())?.Id
                : (await services.GetRequiredService<ISiteThemeService>().GetSiteThemeAsync())?.Id;
            var themeNames = new List<string> { themeName };

            foreach (var resolver in services.GetServices<IResourceFilterThemeResolver>())
            {
                await resolver.UpdateThemeNamesAsync(themeNames, themes, providers);
            }

            themeIds = themeNames
                .SelectMany(themeName => GetThemeAndBaseIds(themes, themeName))
                .ToList();
        }

        var builder = new ResourceFilterBuilder();
        var anyProviders = providers
            .Where(providerInfo => providerInfo.ThemeRequirements.Count == 0 ||
                                   providerInfo.ThemeRequirements.Any(themeIds.Contains))
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
            : [id, .. GetThemeAndBaseIds(themes, baseThemeId)];
    }

    public record ProviderInfo(IResourceFilterProvider Provider, IList<string> ThemeRequirements);
}
