using Microsoft.AspNetCore.Http;
using OrchardCore.ResourceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;

public class ResourceFilter
{
    public Func<HttpContext, bool> Filter { get; set; }
    public Func<HttpContext, Task<bool>> FilterAsync { get; set; }

    public IList<Action<IResourceManager>> Executions { get; init; } = [];
    public IList<Func<IResourceManager, Task>> ExecutionsAsync { get; init; } = [];

    public ResourceFilter Execute(Action<IResourceManager> action)
    {
        Executions.Add(action);
        return this;
    }

    public ResourceFilter ExecuteTask(Func<IResourceManager, Task> actionAsync)
    {
        ExecutionsAsync.Add(actionAsync);
        return this;
    }

    /// <summary>
    /// Registers the provided <c>stylesheet</c> <paramref name="resources"/>.
    /// </summary>
    public ResourceFilter RegisterStylesheet(params string[] resources) =>
        Execute(resourceManager => resources.ForEach(resource => resourceManager.RegisterResource("stylesheet", resource)));

    /// <summary>
    /// Registers the provided <c>script</c> <paramref name="resources"/> at the foot of the page.
    /// </summary>
    public ResourceFilter RegisterFootScript(params string[] resources) =>
        Execute(resourceManager => resources.ForEach(resource => resourceManager.RegisterResource("script", resource).AtFoot()));

    /// <summary>
    /// Registers the provided <c>script</c> <paramref name="resources"/> at the head of the page.
    /// </summary>
    public ResourceFilter RegisterHeadScript(params string[] resources) =>
        Execute(resourceManager => resources.ForEach(resource => resourceManager.RegisterResource("script", resource).AtHead()));

    /// <summary>
    /// Registers the provided <c>link</c> <paramref name="resources"/> at the head of the page.
    /// </summary>
    public ResourceFilter RegisterLink(params LinkEntry[] resources) =>
        Execute(resourceManager => resources.ForEach(resourceManager.RegisterLink));

    /// <summary>
    /// Registers an icon <c>link</c> resource with the provided address and attributes.
    /// </summary>
    public ResourceFilter RegisterFavoriteIcon(string href, string type = "image/x-icon", string rel = "shortcut icon") =>
        Execute(resourceManager => resourceManager.RegisterLink(new LinkEntry
        {
            Href = href,
            Rel = rel,
            Type = type,
        }));

    /// <summary>
    /// Applies the actions listed in <see cref="ExecutionsAsync"/> and <see cref="Executions"/>.
    /// </summary>
    public async Task ApplyAsync(IResourceManager resourceManager)
    {
        foreach (var executionAsync in ExecutionsAsync)
        {
            await executionAsync(resourceManager);
        }

        foreach (var execution in Executions)
        {
            execution(resourceManager);
        }
    }
}
