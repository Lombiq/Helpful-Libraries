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

    private static RequireSettings SetVersionIfAny(RequireSettings requireSettings, string version)
    {
        if (!string.IsNullOrEmpty(version)) requireSettings.UseVersion(version);
        return requireSettings;
    }
}
