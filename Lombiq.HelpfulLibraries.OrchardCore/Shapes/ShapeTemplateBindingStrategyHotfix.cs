using Lombiq.HelpfulLibraries.OrchardCore.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Descriptors;
using OrchardCore.DisplayManagement.Descriptors.ShapeTemplateStrategy;
using OrchardCore.Environment.Extensions.Features;
using OrchardCore.Environment.Shell;
using OrchardCore.Mvc.FileProviders;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Shapes;

/// <summary>
/// An altered version of <see cref="ShapeTemplateBindingStrategy"/> that only skips the features it has already bound
/// the templates to. The original skips the whole extension if any of its features already has a shape descriptor, even
/// if it's from another <see cref="IShapeTableProvider"/>, so on some tenants the templates are never bound.
/// </summary>
public class ShapeTemplateBindingStrategyHotfix : ShapeTableProvider, IShapeTableHarvester
{
    private static readonly ConcurrentDictionary<string, bool> BoundFeatureIds = new();

    private readonly IEnumerable<IShapeTemplateHarvester> _harvesters;
    private readonly IShellFeaturesManager _shellFeaturesManager;
    private readonly IShapeTemplateFileProviderAccessor _fileProviderAccessor;
    private readonly Dictionary<string, IShapeTemplateViewEngine> _viewEnginesByExtension =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly string[] _viewEnginesFileExtensions;

    public ShapeTemplateBindingStrategyHotfix(
        IEnumerable<IShapeTemplateHarvester> harvesters,
        IShellFeaturesManager shellFeaturesManager,
        IEnumerable<IShapeTemplateViewEngine> shapeTemplateViewEngines,
        IShapeTemplateFileProviderAccessor fileProviderAccessor)
    {
        _harvesters = harvesters;
        _shellFeaturesManager = shellFeaturesManager;
        _fileProviderAccessor = fileProviderAccessor;

        foreach (var viewEngine in shapeTemplateViewEngines)
        {
            foreach (var extension in viewEngine.TemplateFileExtensions)
            {
                _viewEnginesByExtension.TryAdd(extension, viewEngine);
            }
        }

        _viewEnginesFileExtensions = [.. _viewEnginesByExtension.Keys];
    }

    public override async ValueTask DiscoverAsync(ShapeTableBuilder builder)
    {
        var enabledFeatures = (await _shellFeaturesManager.GetEnabledFeaturesAsync()).ToList();

        var featureIdsToBind = enabledFeatures
            .Select(feature => feature.Id)
            .Where(id => !BoundFeatureIds.ContainsKey(id) || !builder.ExcludedFeatureIds.Contains(id))
            .ToHashSet();

        var extensions = enabledFeatures
            .Where(feature => featureIdsToBind.Contains(feature.Id))
            .Select(feature => feature.Extension)
            .DistinctBy(extension => extension.Id);

        foreach (var extension in extensions)
        {
            var features = extension.Features.Where(feature => featureIdsToBind.Contains(feature.Id)).ToList();

            foreach (var harvester in _harvesters)
            {
                foreach (var subPath in harvester.SubPaths())
                {
                    BindTemplates(builder, harvester, extension.SubPath, subPath, features);
                }
            }
        }

        foreach (var id in featureIdsToBind)
        {
            BoundFeatureIds.TryAdd(id, value: true);
        }
    }

    private void BindTemplates(
        ShapeTableBuilder builder,
        IShapeTemplateHarvester harvester,
        string extensionSubPath,
        string subPath,
        IList<IFeatureInfo> features)
    {
        var filePaths = _fileProviderAccessor.FileProvider.GetViewFilePaths(
            PathExtensions.Combine(extensionSubPath, subPath),
            _viewEnginesFileExtensions,
            inViewsFolder: true,
            inDepth: false);

        foreach (var filePath in filePaths)
        {
            var harvestShapeInfo = new HarvestShapeInfo
            {
                SubPath = subPath,
                FileName = Path.GetFileNameWithoutExtension(filePath),
                RelativePath = filePath,
                Extension = Path.GetExtension(filePath),
            };

            var viewEngineType = _viewEnginesByExtension[harvestShapeInfo.Extension].GetType();

            foreach (var hit in harvester.HarvestShape(harvestShapeInfo))
            {
                foreach (var feature in features)
                {
                    builder
                        .Describe(hit.ShapeType)
                        .From(feature)
                        .BoundAs(filePath, displayContext => displayContext
                            .ServiceProvider
                            .GetServices<IShapeTemplateViewEngine>()
                            .Single(viewEngine => viewEngine.GetType() == viewEngineType)
                            .RenderAsync(filePath, displayContext));
                }
            }
        }
    }

    // Mark this as obsolete once https://github.com/OrchardCMS/OrchardCore/issues/20102 is fixed.
    public static void ReplaceStrategy(IServiceCollection services) =>
        services
            .RemoveByImplementation<ShapeTemplateBindingStrategy>()
            .AddShapeTableProvider<ShapeTemplateBindingStrategyHotfix>();

    public static void ReplaceStrategy(OrchardCoreBuilder builder) =>
        builder.ApplicationServices.AddInlineStartup(ReplaceStrategy, order: 99);
}
