using System;

namespace Lombiq.HelpfulLibraries.OrchardCore.Mvc;

[AttributeUsage(AttributeTargets.Method)]
public sealed class AdminRouteAttribute : Attribute
{
    public string Template { get; }

    public AdminRouteAttribute(string template) => Template = template;
}
