using AngleSharp.Common;
using Lombiq.HelpfulLibraries.OrchardCore.Contents;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using OrchardCore.DisplayManagement.Descriptors;
using OrchardCore.DisplayManagement.Layout;
using OrchardCore.DisplayManagement.Shapes;
using OrchardCore.DisplayManagement.Theming;
using OrchardCore.ResourceManagement;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;

public record ScriptModuleResourceFilter(
    IFileVersionProvider FileVersionProvider,
    ILayoutAccessor LayoutAccessor,
    IResourceManager ResourceManager,
    IOptions<ResourceManagementOptions> ResourceManagerOptions,
    IShapeTableManager ShapeTableManager,
    IThemeManager ThemeManager) : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var theme = await ThemeManager.GetThemeAsync();
        var shapeTable = ShapeTableManager.GetShapeTable(theme.Id);
        var shape = new Shape
        {
            Metadata =
            {
                Type = nameof(ScriptModuleResourceFilter),
            },
        };

        var shapeDescriptor = new ShapeDescriptor
        {
            ShapeType = nameof(ScriptModuleResourceFilter),
            Bindings =
            {
                [nameof(ScriptModuleResourceFilter)] = new ShapeBinding
                {
                    BindingName = nameof(ScriptModuleResourceFilter),
                    BindingAsync = _ => Task.FromResult(Display()),
                },
            },
        };

        shapeTable.Descriptors[shapeDescriptor.ShapeType] = shapeDescriptor;
        foreach (var binding in shapeDescriptor.Bindings)
        {
            shapeTable.Bindings[binding.Key] = binding.Value;
        }

        await LayoutAccessor.AddShapeToZoneAsync("Content", shape, "After");
        await next();
    }

    private IHtmlContent Display()
    {
        var options = ResourceManagerOptions.Value;
        var scriptElements = ResourceManager
            .GetRequiredScriptModuleTags(options.ContentBasePath)
            .ToList();

        if (!scriptElements.Any()) return null;

        var importMap = options.GetScriptModuleImportMap(
            options.ResourceManifests.Concat(ResourceManager.InlineManifest),
            FileVersionProvider);

        var content = new HtmlContentBuilder(capacity: scriptElements.Count + 1).AppendHtml(importMap);
        foreach (var script in scriptElements) content.AppendHtml(script);

        return content;
    }
}
