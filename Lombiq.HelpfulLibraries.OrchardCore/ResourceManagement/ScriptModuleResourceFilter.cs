using AngleSharp.Common;
using Lombiq.HelpfulLibraries.OrchardCore.Contents;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.DisplayManagement.Implementation;
using OrchardCore.DisplayManagement.Layout;
using OrchardCore.ResourceManagement;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;

/// <summary>
/// A filter that looks for the required <c>script-module"</c> resources. If there were any, it injects the input map
/// (used for mapping module names to URLs) of all registered module resources and the script blocks of the currently
/// required resource.
/// </summary>
public record ScriptModuleResourceFilter(ILayoutAccessor LayoutAccessor) : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var shape = await context.HttpContext.RequestServices.CreateAdHocShapeForCurrentThemeAsync(
            nameof(ScriptModuleResourceFilter),
            displayContext => Task.FromResult(DisplayScriptModuleResources(displayContext.ServiceProvider)));

        await LayoutAccessor.AddShapeToZoneAsync("Content", shape, "After");
        await next();
    }

    // We can't safely inject resources from constructor here, as some get disposed by the time this display takes place.
    private IHtmlContent DisplayScriptModuleResources(IServiceProvider serviceProvider)
    {
        // Won't work correctly with injected resources, the scriptElements below will be empty. Possibly related to the
        // IResourceManager.InlineManifest being different.
        var resourceManager = serviceProvider.GetRequiredService<IResourceManager>();
        var options = serviceProvider.GetRequiredService<IOptions<ResourceManagementOptions>>().Value;
        var fileVersionProvider = serviceProvider.GetRequiredService<IFileVersionProvider>();

        var scriptElements = resourceManager
            .GetRequiredScriptModuleTags(options.ContentBasePath)
            .ToList();

        if (!scriptElements.Any()) return null;

        var importMap = options.GetScriptModuleImportMap(
            options.ResourceManifests.Concat(resourceManager.InlineManifest),
            fileVersionProvider);

        var content = new HtmlContentBuilder(capacity: scriptElements.Count + 1).AppendHtml(importMap);
        foreach (var script in scriptElements) content.AppendHtml(script);

        return content;
    }
}
