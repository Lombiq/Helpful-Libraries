using Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;
using Microsoft.AspNetCore.Html;
using OrchardCore.ResourceManagement.TagHelpers;
using System;
using System.IO;

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
    /// cref="GetScriptModuleMap"/> so they can be imported by module type scripts using the <c>import ... from</c>
    /// statement.
    /// </summary>
    public static ResourceDefinition DefineScriptModule(this ResourceManifest manifest, string name) =>
        manifest.DefineResource(ResourceTypes.ScriptModule, name);

    /// <summary>
    /// Registers a <c>script-module"</c> resource to be used on the current page. These can be rendered using <see
    /// cref="GetRequiredScriptModuleTags"/> as <c>&lt;script src="..." type="module"&gt;</c> elements.
    /// </summary>
    public static RequireSettings RegisterScriptModule(this IResourceManager resourceManager, string name) =>
        resourceManager.RegisterResource(ResourceTypes.ScriptModule, name);

    private static RequireSettings SetVersionIfAny(RequireSettings requireSettings, string version)
    {
        if (!string.IsNullOrEmpty(version)) requireSettings.UseVersion(version);
        return requireSettings;
    }
}
