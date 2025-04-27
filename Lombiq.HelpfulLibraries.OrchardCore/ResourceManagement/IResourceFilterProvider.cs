using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;

/// <summary>
/// A service that can add static resources to the resource management pipeline.
/// </summary>
public interface IResourceFilterProvider
{
    /// <summary>
    /// Gets the collection of required themes dynamically. The contents are concatenated with any <see
    /// cref="ResourceFilterThemeRequirementAttribute"/>  attributes attached to the provider.
    /// </summary>
    IEnumerable<string> RequiredThemes => [];

    /// <summary>
    /// Adds static resources to the pipeline that will be loaded based on various criteria.
    /// </summary>
    void AddResourceFilter(ResourceFilterBuilder builder);
}

public static class ResourceFilterProviderExtensions
{
    /// <summary>
    /// Returns the themes required by the <see cref="ResourceFilterThemeRequirementAttribute"/> from the provider.
    /// </summary>
    [SuppressMessage(
        "Naming",
        "CS0618:Type or member is obsolete",
        Justification = "Obsolete is only used because the referenced member will be turned internal.")]
    public static IEnumerable<string> GetRequiredThemes(this IResourceFilterProvider provider) =>
        ResourceFilterThemeRequirementAttribute.GetRequirementsByType(provider.GetType());
}
