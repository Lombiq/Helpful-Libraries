using Lombiq.HelpfulLibraries.OrchardCore.Shapes;
using Microsoft.Extensions.FileProviders;
using Moq;
using OrchardCore.DisplayManagement.Descriptors;
using OrchardCore.DisplayManagement.Descriptors.ShapeTemplateStrategy;
using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Extensions.Features;
using OrchardCore.Environment.Shell;
using Shouldly;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Lombiq.HelpfulLibraries.Tests.UnitTests.Shapes;

public class ShapeTemplateBindingStrategyHotfixTests
{
    private const string ExtensionId = "Lombiq.HelpfulLibraries.Tests.ShapeTemplateBindingStrategyHotfix";
    private const string ShapeType = "testpart_edit";

    [Fact]
    public async Task TemplatesShouldBeBoundToFeaturesExcludedByOtherProviders()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var viewsPath = Path.Combine(rootPath, ExtensionId, "Views");
        Directory.CreateDirectory(viewsPath);
        await File.WriteAllTextAsync(
            Path.Combine(viewsPath, "TestPart.Edit.cshtml"),
            string.Empty,
            TestContext.Current.CancellationToken);

        try
        {
            using var fileProvider = new PhysicalFileProvider(rootPath);

            var extension = new Mock<IExtensionInfo>();
            var widgets = CreateFeature(extension.Object, ExtensionId + ".Widgets");
            var flows = CreateFeature(extension.Object, ExtensionId + ".Flows");
            extension.SetupGet(info => info.Id).Returns(ExtensionId);
            extension.SetupGet(info => info.SubPath).Returns(ExtensionId);
            extension.SetupGet(info => info.Features).Returns([widgets, flows]);

            (await DiscoverAsync(fileProvider, [widgets], [])).ShouldBe([(widgets.Id, ShapeType)]);

            // Flows is already excluded because another shape table provider has a descriptor for it, but the
            // templates weren't bound to it yet.
            (await DiscoverAsync(fileProvider, [flows], [widgets.Id, flows.Id])).ShouldBe([(flows.Id, ShapeType)]);

            (await DiscoverAsync(fileProvider, [flows], [flows.Id])).ShouldBeEmpty();
        }
        finally
        {
            Directory.Delete(rootPath, recursive: true);
        }
    }

    private static IFeatureInfo CreateFeature(IExtensionInfo extension, string id)
    {
        var feature = new Mock<IFeatureInfo>();
        feature.SetupGet(info => info.Id).Returns(id);
        feature.SetupGet(info => info.Extension).Returns(extension);

        return feature.Object;
    }

    private static async Task<List<(string FeatureId, string ShapeType)>> DiscoverAsync(
        IFileProvider fileProvider,
        IList<IFeatureInfo> enabledFeatures,
        IReadOnlyCollection<string> excludedFeatureIds)
    {
        var shellFeaturesManager = new Mock<IShellFeaturesManager>();
        shellFeaturesManager.Setup(manager => manager.GetEnabledFeaturesAsync()).ReturnsAsync(enabledFeatures);

        var viewEngine = new Mock<IShapeTemplateViewEngine>();
        viewEngine.SetupGet(engine => engine.TemplateFileExtensions).Returns([".cshtml"]);

        var fileProviderAccessor = new Mock<IShapeTemplateFileProviderAccessor>();
        fileProviderAccessor.SetupGet(accessor => accessor.FileProvider).Returns(fileProvider);

        var strategy = new ShapeTemplateBindingStrategyHotfix(
            [new BasicShapeTemplateHarvester()],
            shellFeaturesManager.Object,
            [viewEngine.Object],
            fileProviderAccessor.Object);

        var builder = new ShapeTableBuilder(enabledFeatures[0], excludedFeatureIds);
        await strategy.DiscoverAsync(builder);

        return builder
            .BuildAlterations()
            .Select(alteration => (alteration.Feature.Id, alteration.ShapeType))
            .ToList();
    }
}
