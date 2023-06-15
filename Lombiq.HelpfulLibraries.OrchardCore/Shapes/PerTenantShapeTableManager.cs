using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrchardCore.DisplayManagement.Descriptors;
using OrchardCore.DisplayManagement.Extensions;
using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Extensions.Features;
using OrchardCore.Environment.Shell;
using OrchardCore.Settings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Shapes;

/// <summary>
/// An altered version of <see cref="DefaultShapeTableManager"/> where the shape table is cached per tenant and per
/// theme instead of just per theme. Also the shape descriptor collection is cached per tenant instead of being a static
/// dictionary.
/// </summary>
public class PerTenantShapeTableManager : IShapeTableManager
{
    private readonly IHostEnvironment _hostingEnvironment;
    private readonly IEnumerable<IShapeTableProvider> _bindingStrategies;
    private readonly IShellFeaturesManager _shellFeaturesManager;
    private readonly IExtensionManager _extensionManager;
    private readonly ITypeFeatureProvider _typeFeatureProvider;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger _logger;
    private readonly ISiteService _siteService;

    [SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "All of these are necessary for shape table management.")]
    public PerTenantShapeTableManager(
        IHostEnvironment hostingEnvironment,
        IEnumerable<IShapeTableProvider> bindingStrategies,
        IShellFeaturesManager shellFeaturesManager,
        IExtensionManager extensionManager,
        ITypeFeatureProvider typeFeatureProvider,
        IMemoryCache memoryCache,
        ILogger<PerTenantShapeTableManager> logger,
        ISiteService siteService)
    {
        _hostingEnvironment = hostingEnvironment;
        _bindingStrategies = bindingStrategies;
        _shellFeaturesManager = shellFeaturesManager;
        _extensionManager = extensionManager;
        _typeFeatureProvider = typeFeatureProvider;
        _memoryCache = memoryCache;
        _logger = logger;
        _siteService = siteService;
    }

    public ShapeTable GetShapeTable(string themeId) =>
        GetShapeTableAsync(themeId)
            .GetAwaiter()
            .GetResult();

    public async Task<ShapeTable> GetShapeTableAsync(string themeId)
    {
        var siteSettings = await _siteService.GetSiteSettingsAsync();

        var shapeTableCacheKey = $"ShapeTable:{siteSettings.SiteName}:{themeId}";
        var shapeDescriptorsCacheKey = $"ShapeDescriptors:{siteSettings.SiteName}";

        if (_memoryCache.TryGetValue(shapeTableCacheKey, out ShapeTable shapeTable)) return shapeTable;

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Start building shape table");
        }

        var shapeDescriptors = _memoryCache.GetOrNew<Dictionary<string, FeatureShapeDescriptor>>(shapeDescriptorsCacheKey);
        var excludedFeatures = shapeDescriptors.Values.Select(value => value.Feature.Id).ToHashSet();

        foreach (var bindingStrategy in _bindingStrategies)
        {
            var strategyFeature = _typeFeatureProvider.GetFeatureForDependency(bindingStrategy.GetType());

            var builder = new ShapeTableBuilder(strategyFeature, excludedFeatures);
            bindingStrategy.Discover(builder);
            var builtAlterations = builder.BuildAlterations();

            BuildDescriptors(bindingStrategy, builtAlterations, shapeDescriptors);
        }

        _memoryCache.Set(shapeDescriptorsCacheKey, shapeDescriptors);

        var enabledAndOrderedFeatureIds = (await _shellFeaturesManager.GetEnabledFeaturesAsync())
            .Select(featureInfo => featureInfo.Id)
            .ToList();

        // Let the application acting as a super theme for shapes rendering.
        if (enabledAndOrderedFeatureIds.Remove(_hostingEnvironment.ApplicationName))
        {
            enabledAndOrderedFeatureIds.Add(_hostingEnvironment.ApplicationName);
        }

        var descriptors = shapeDescriptors
            .Where(sd => enabledAndOrderedFeatureIds.Contains(sd.Value.Feature.Id))
            .Where(sd => IsModuleOrRequestedTheme(sd.Value.Feature, themeId))
            .OrderBy(sd => enabledAndOrderedFeatureIds.IndexOf(sd.Value.Feature.Id))
            .GroupBy(sd => sd.Value.ShapeType, StringComparer.OrdinalIgnoreCase)
            .Select(group => new ShapeDescriptorIndex(
                shapeType: group.Key,
                alterationKeys: group.Select(kv => kv.Key),
                descriptors: new ConcurrentDictionary<string, FeatureShapeDescriptor>(shapeDescriptors)
            ))
            .ToList();

        shapeTable = new ShapeTable(
            descriptors: descriptors.ToDictionary(sd => sd.ShapeType, x => (ShapeDescriptor)x, StringComparer.OrdinalIgnoreCase),
            bindings: descriptors.SelectMany(sd => sd.Bindings).ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase)
        );

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Done building shape table");
        }

        _memoryCache.Set(shapeTableCacheKey, shapeTable, new MemoryCacheEntryOptions { Priority = CacheItemPriority.NeverRemove });

        return shapeTable;
    }

    private static void BuildDescriptors(
        IShapeTableProvider bindingStrategy,
        IEnumerable<ShapeAlteration> builtAlterations,
        IDictionary<string, FeatureShapeDescriptor> shapeDescriptors)
    {
        var alterationSets = builtAlterations.GroupBy(a => a.Feature.Id + a.ShapeType);

        foreach (var alterations in alterationSets)
        {
            var firstAlteration = alterations.First();
            var feature = firstAlteration.Feature;
            var shapeType = firstAlteration.ShapeType;

            var key = $"{bindingStrategy.GetType().Name}{feature.Id}{shapeType}".ToUpperInvariant();

            if (!shapeDescriptors.ContainsKey(key))
            {
                shapeDescriptors[key] = alterations.AggregateSeed(
                    new FeatureShapeDescriptor(feature, shapeType),
                    (descriptor, alteration) => alteration.Alter(descriptor));
            }
        }
    }

    private bool IsModuleOrRequestedTheme(IFeatureInfo feature, string themeId)
    {
        if (!feature.IsTheme())
        {
            return true;
        }

        if (string.IsNullOrEmpty(themeId))
        {
            return false;
        }

        return feature.Id == themeId || IsBaseTheme(feature.Id, themeId);
    }

    private bool IsBaseTheme(string themeFeatureId, string themeId) =>
        _extensionManager
            .GetFeatureDependencies(themeId)
            .Any(f => f.Id == themeFeatureId);

    public static void ReplaceDefaultShapeTableManager(IServiceCollection services)
    {
        services.RemoveAll(service => service.ImplementationType == typeof(DefaultShapeTableManager));
        services.AddTransient<IShapeTableManager, PerTenantShapeTableManager>();
    }
}
