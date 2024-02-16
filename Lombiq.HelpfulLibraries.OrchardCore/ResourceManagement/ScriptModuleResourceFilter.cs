using AngleSharp.Common;
using Lombiq.HelpfulLibraries.OrchardCore.Contents;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using OrchardCore.DisplayManagement.Implementation;
using OrchardCore.DisplayManagement.Layout;
using OrchardCore.ResourceManagement;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;

public record ScriptModuleResourceFilter(
    IFileVersionProvider FileVersionProvider,
    ILayoutAccessor LayoutAccessor,
    IResourceManager ResourceManager,
    IOptions<ResourceManagementOptions> ResourceManagerOptions) : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var shape = await context.HttpContext.RequestServices.CreateAdHocShapeForCurrentThemeAsync(
            nameof(ScriptModuleResourceFilter),
            _ => Task.FromResult(DisplayScriptModuleResources()));

        await LayoutAccessor.AddShapeToZoneAsync("Content", shape, "After");
        await next();
    }

    private IHtmlContent DisplayScriptModuleResources()
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
