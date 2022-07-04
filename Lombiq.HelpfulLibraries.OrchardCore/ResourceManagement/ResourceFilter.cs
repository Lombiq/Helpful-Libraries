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

    public void RegisterStylesheet(params string[] resources) =>
        Execute(resourceManager => resources.ForEach(resource => resourceManager.RegisterResource("stylesheet", resource)));

    public void RegisterFootScript(params string[] resources) =>
        Execute(resourceManager => resources.ForEach(resource => resourceManager.RegisterResource("script", resource).AtFoot()));

    public void RegisterHeadScript(params string[] resources) =>
        Execute(resourceManager => resources.ForEach(resource => resourceManager.RegisterResource("script", resource).AtHead()));
}
