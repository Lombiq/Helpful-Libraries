using System.Collections.Generic;

namespace Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;

/// <summary>
/// A service that can add static resources to the resource management pipeline.
/// </summary>
public interface IResourceFilterProvider
{
    /// <summary>
    /// Adds static resources to the pipeline that will be loaded based on various criteria.
    /// </summary>
    void AddResourceFilter(ResourceFilterBuilder builder);
}

public static class ResourceFilterProviderExtensions
{
    public static IEnumerable<string> GetRequiredThemes(this IResourceFilterProvider provider) =>
        ResourceFilterThemeRequirementAttribute.GetRequirementsByType(provider.GetType());
}
