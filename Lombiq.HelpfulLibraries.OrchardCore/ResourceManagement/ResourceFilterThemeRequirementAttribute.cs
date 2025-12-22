using System;
using System.Collections.Generic;
using System.Reflection;

namespace Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ResourceFilterThemeRequirementAttribute : Attribute
{
    public string Theme { get; }

    public ResourceFilterThemeRequirementAttribute(string theme) => Theme = theme;
}
