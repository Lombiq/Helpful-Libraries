using Microsoft.AspNetCore.Http;
using OrchardCore.ResourceManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;

public class ResourceFilter
{
    public Func<HttpContext, bool> Filter { get; set; }
    public Func<HttpContext, Task<bool>> FilterAsync { get; set; }
    public Action<IResourceManager> Execution { get; set; }

    public void Execute(Action<IResourceManager> action) => Execution = action;

    /// <summary>
    /// Registers the provided <c>stylesheet</c> <paramref name="resources"/>.
    /// </summary>
    public void RegisterStylesheet(params string[] resources) =>
        Execute(resourceManager => resources.ForEach(resource => resourceManager.RegisterResource("stylesheet", resource)));

    /// <summary>
    /// Registers the provided <c>script</c> <paramref name="resources"/> at the foot of the page.
    /// </summary>
    public void RegisterFootScript(params string[] resources) =>
        Execute(resourceManager => resources.ForEach(resource => resourceManager.RegisterResource("script", resource).AtFoot()));

    /// <summary>
    /// Registers the provided <c>script</c> <paramref name="resources"/> at the head of the page.
    /// </summary>
    public void RegisterHeadScript(params string[] resources) =>
        Execute(resourceManager => resources.ForEach(resource => resourceManager.RegisterResource("script", resource).AtHead()));
}
