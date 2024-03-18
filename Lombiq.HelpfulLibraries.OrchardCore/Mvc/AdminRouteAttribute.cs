using System;

namespace Lombiq.HelpfulLibraries.OrchardCore.Mvc;

[Obsolete("Use the [Admin(route)] attribute instead.")]
[AttributeUsage(AttributeTargets.Method)]
public sealed class AdminRouteAttribute : Attribute
{
    public string Template { get; }

    public AdminRouteAttribute(string template) => Template = template;
}
