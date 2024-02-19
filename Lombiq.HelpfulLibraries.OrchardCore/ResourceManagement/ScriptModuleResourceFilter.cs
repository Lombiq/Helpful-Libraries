using Lombiq.HelpfulLibraries.OrchardCore.Contents;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.DisplayManagement.Implementation;
using OrchardCore.DisplayManagement.Layout;
using OrchardCore.ResourceManagement;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;

// Don't replace the "script-module" there with <c>script-module</c> as that will cause the DOC105UseParamref analyzer
// to throw NullReferenceException. The same doesn't seem to happen in other files, for example the
// ResourceManagerExtensions.cs in this directory.

/// <summary>
/// A filter that looks for the required "script-module" resources. If there were any, it injects the input map
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

    // We can't safely inject resources from the constructor because some resources may get disposed by the time  this
    // display action takes place, leading to potential access of disposed objects. Instead, the DisplayContext's
    // service provider is used.
    private static IHtmlContent DisplayScriptModuleResources(IServiceProvider serviceProvider)
    {
        // Won't work correctly with injected resources, the scriptElements below will be empty. Possibly related to the
        // IResourceManager.InlineManifest being different.
        var resourceManager = serviceProvider.GetRequiredService<IResourceManager>();
        var options = serviceProvider.GetRequiredService<IOptions<ResourceManagementOptions>>().Value;

        var scriptElements = resourceManager.GetRequiredScriptModuleTags(options.ContentBasePath).ToList();
        if (!scriptElements.Any()) return null;

        var importMap = serviceProvider.GetScriptModuleImportMap();
        var content = new HtmlContentBuilder(capacity: scriptElements.Count + 1).AppendHtml(importMap);
        foreach (var script in scriptElements) content.AppendHtml(script);

        return content;
    }
}
