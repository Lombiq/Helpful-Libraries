using AngleSharp.Common;
using Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement.TagHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace OrchardCore.ResourceManagement;

public static class ResourceManagerExtensions
{
    /// <summary>
    /// Registers a <c>stylesheet</c> resource by name at head.
    /// </summary>
    public static RequireSettings RegisterStyle(
        this IResourceManager resourceManager,
        string resourceName,
        string version = null) =>
        SetVersionIfAny(resourceManager.RegisterResource("stylesheet", resourceName), version);

    /// <summary>
    /// Registers a <c>script</c> resource by name at the given <paramref name="location"/> (at foot by default).
    /// </summary>
    public static RequireSettings RegisterScript(
        this IResourceManager resourceManager,
        string resourceName,
        string version = null,
        ResourceLocation location = ResourceLocation.Foot) =>
        SetVersionIfAny(resourceManager.RegisterResource("script", resourceName).AtLocation(location), version);

    /// <summary>
    /// Renders the HTML for the header section and provides a hook to alter the resulting string. Similar to the
    /// <c>&lt;resources type="Header"/&gt;</c> by <see cref="ResourcesTagHelper"/>.
    /// </summary>
    public static HtmlString RenderAndTransformHeader(
        this IResourceManager resourceManager,
        Func<string, string> transform)
    {
        using var stringWriter = new StringWriter();

        resourceManager.RenderMeta(stringWriter);
        resourceManager.RenderHeadLink(stringWriter);
        resourceManager.RenderStylesheet(stringWriter);
        resourceManager.RenderHeadScript(stringWriter);

        var headerHtml = transform(stringWriter.ToString());
        return new HtmlString(headerHtml);
    }

    /// <summary>
    /// Adds a <c>script-module"</c> resource to the manifest. All of these resources are mapped using <see
    /// cref="GetScriptModuleImportMap(IOrchardHelper)"/> so they can be imported by module type scripts using the
    /// <c>import ResourceName from 'resourceName'</c> statement.
    /// </summary>
    public static ResourceDefinition DefineScriptModule(this ResourceManifest manifest, string name) =>
        manifest.DefineResource(ResourceTypes.ScriptModule, name);

    /// <summary>
    /// Registers a <c>script-module"</c> resource to be used on the current page. These can be rendered using <see
    /// cref="GetRequiredScriptModuleTags"/> as <c>&lt;script src="..." type="module"&gt;</c> elements.
    /// </summary>
    public static RequireSettings RegisterScriptModule(this IResourceManager resourceManager, string name) =>
        resourceManager.RegisterResource(ResourceTypes.ScriptModule, name);

    /// <summary>
    /// Turns the required <c>script-module"</c> resources into <c>&lt;script src="..." type="module"&gt;</c> elements.
    /// </summary>
    /// <param name="basePath">
    /// The path that's used to resolve <c>~</c> in the resource URLs. Typically <see
    /// cref="ResourceManagementOptions.ContentBasePath"/> should be used..
    /// </param>
    /// <param name="filter">
    /// If not <see langword="null"/> it's used to select which required resources should be considered.
    /// </param>
    public static IEnumerable<TagBuilder> GetRequiredScriptModuleTags(
        this IResourceManager resourceManager,
        string basePath = null,
        Func<ResourceRequiredContext, bool> filter = null)
    {
        var contexts = resourceManager.GetRequiredResources(ResourceTypes.ScriptModule);
        if (filter != null) contexts = contexts.Where(filter);

        return contexts.Select(context =>
        {
            var builder = new TagBuilder("script")
            {
                TagRenderMode = TagRenderMode.Normal,
                Attributes =
                {
                    ["type"] = "module",
                    ["src"] = context.Resource.GetResourceUrl(
                        context.FileVersionProvider,
                        context.Settings.DebugMode,
                        context.Settings.CdnMode,
                        basePath),
                },
            };
            builder.MergeAttributes(context.Resource.Attributes, replaceExisting: true);

            return builder;
        });
    }

    /// <summary>
    /// Turns the required <c>script-module"</c> resource with the <paramref name="resourceName"/> into a
    /// <c>&lt;script src="..." type="module"&gt;</c> element.
    /// </summary>
    /// <param name="basePath">
    /// The path that's used to resolve <c>~</c> in the resource URLs. Typically <see
    /// cref="ResourceManagementOptions.ContentBasePath"/> should be used..
    /// </param>
    /// <param name="resourceName">The expected value of <see cref="ResourceDefinition.Name"/>.</param>
    public static TagBuilder GetRequiredScriptModuleTag(
        this IResourceManager resourceManager,
        string basePath,
        string resourceName) =>
        resourceManager
            .GetRequiredScriptModuleTags(
                basePath,
                context => context.Resource.Name == resourceName)
            .FirstOrDefault();

    /// <summary>
    /// Returns a <c>&lt;script type="importmap"&gt;</c> element that maps all the registered module resources by
    /// resource name to their respective URLs so you can import these resources in your module type scripts using
    /// <c>import someModule from 'resourceName'</c> instead of using the full resource URL. This way import will work
    /// regardless of your CDN configuration.
    /// </summary>
    public static IHtmlContent GetScriptModuleImportMap(
        this ResourceManagementOptions resourceOptions,
        IEnumerable<ResourceManifest> resourceManifests,
        IFileVersionProvider fileVersionProvider)
    {
        var imports = (resourceManifests ?? resourceOptions.ResourceManifests)
            .SelectMany(manifest => manifest.GetResources(ResourceTypes.ScriptModule).Values)
            .SelectMany(list => list)
            .ToDictionary(
                resource => resource.Name,
                resource => resource.GetResourceUrl(
                    fileVersionProvider,
                    resourceOptions.DebugMode,
                    resourceOptions.UseCdn,
                    resourceOptions.ContentBasePath));

        var tagBuilder = new TagBuilder("script")
        {
            TagRenderMode = TagRenderMode.Normal,
            Attributes = { ["type"] = "importmap" },
        };

        tagBuilder.InnerHtml.AppendHtml(JsonSerializer.Serialize(new { imports }));
        return tagBuilder;
    }

    /// <inheritdoc cref="GetScriptModuleImportMap(ResourceManagementOptions, IEnumerable{ResourceManifest}, IFileVersionProvider)"/>
    internal static IHtmlContent GetScriptModuleImportMap(this IServiceProvider serviceProvider)
    {
        var options = serviceProvider.GetRequiredService<IOptions<ResourceManagementOptions>>().Value;
        var resourceManager = serviceProvider.GetRequiredService<IResourceManager>();
        var fileVersionProvider = serviceProvider.GetRequiredService<IFileVersionProvider>();

        return options.GetScriptModuleImportMap(
            options.ResourceManifests.Concat(resourceManager.InlineManifest),
            fileVersionProvider);
    }

    /// <inheritdoc cref="GetScriptModuleImportMap(ResourceManagementOptions, IEnumerable{ResourceManifest}, IFileVersionProvider)"/>
    public static IHtmlContent GetScriptModuleImportMap(this IOrchardHelper helper) =>
        helper.HttpContext.RequestServices.GetScriptModuleImportMap();

    private static string GetResourceUrl(
        this ResourceDefinition definition,
        IFileVersionProvider fileVersionProvider,
        bool isDebug,
        bool isCdn,
        PathString basePath)
    {
        static string Coalesce(params string[] strings) => strings.Find(str => !string.IsNullOrEmpty(str));

        var url = (isDebug, isCdn) switch
        {
            (true, true) => Coalesce(definition.UrlCdnDebug, definition.UrlDebug, definition.UrlCdn, definition.Url),
            (true, false) => Coalesce(definition.UrlDebug, definition.Url, definition.UrlCdnDebug, definition.UrlCdn),
            (false, true) => Coalesce(definition.UrlCdn, definition.Url, definition.UrlCdnDebug, definition.UrlDebug),
            (false, false) => Coalesce(definition.Url, definition.UrlDebug, definition.UrlCdn, definition.UrlCdnDebug),
        };

        if (string.IsNullOrEmpty(url)) return url;

        if (url.StartsWith("~/", StringComparison.Ordinal))
        {
            url = basePath.Value?.TrimEnd('/') + url[1..];
        }

        return fileVersionProvider.AddFileVersionToPath(basePath, url);
    }

    private static RequireSettings SetVersionIfAny(RequireSettings requireSettings, string version)
    {
        if (!string.IsNullOrEmpty(version)) requireSettings.UseVersion(version);
        return requireSettings;
    }
}
