using System;
using System.Collections.Generic;
using System.Reflection;

namespace Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ResourceFilterThemeRequirementAttribute : Attribute
{
    public string Theme { get; }

    public ResourceFilterThemeRequirementAttribute(string theme) => Theme = theme;

    /// <summary>
    /// Retrieves the required themes by <paramref name="type"/>.
    /// </summary>
    public static IEnumerable<string> GetRequirementsByType(Type type) =>
        type
            .GetCustomAttributes<ResourceFilterThemeRequirementAttribute>(inherit: false)
            .SelectWhere(attribute => attribute.Theme, theme => !string.IsNullOrEmpty(theme));
}
