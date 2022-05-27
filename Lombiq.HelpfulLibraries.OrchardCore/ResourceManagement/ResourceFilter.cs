using Microsoft.AspNetCore.Http;
using OrchardCore.ResourceManagement;
using System;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;

public class ResourceFilter
{
    public Func<HttpContext, bool> Filter { get; set; }
    public Func<HttpContext, Task<bool>> FilterAsync { get; set; }
    public Action<IResourceManager> Execution { get; set; }

    public ResourceFilter Execute(Action<IResourceManager> action)
    {
        Execution = action;
        return this;
    }

    public ResourceFilter RegisterStylesheet(string resource)
    {
        Execute(resourceManager => resourceManager.RegisterResource("stylesheet", resource));
        return this;
    }

    public ResourceFilter RegisterFootScript(string resource)
    {
        Execute(resourceManager => resourceManager.RegisterResource("script", resource).AtFoot());
        return this;
    }

    public ResourceFilter RegisterHeadScript(string resource)
    {
        Execute(resourceManager => resourceManager.RegisterResource("script", resource).AtHead());
        return this;
    }
}
