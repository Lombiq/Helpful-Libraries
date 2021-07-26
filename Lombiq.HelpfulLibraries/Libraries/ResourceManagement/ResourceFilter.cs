using Microsoft.AspNetCore.Http;
using OrchardCore.ResourceManagement;
using System;

namespace Lombiq.HelpfulLibraries.Libraries.ResourceManagement
{
    public class ResourceFilter
    {
        public Func<HttpContext, bool> Filter { get; set; }
        public Action<IResourceManager> Execution { get; set; }

        public void Execute(Action<IResourceManager> action) => Execution = action;

        public void RegisterStylesheet(string resource) =>
            Execute(resourceManager => resourceManager.RegisterResource("stylesheet", resource));

        public void RegisterFootScript(string resource) =>
            Execute(resourceManager => resourceManager.RegisterResource("script", resource).AtFoot());

        public void RegisterHeadScript(string resource) =>
            Execute(resourceManager => resourceManager.RegisterResource("script", resource).AtHead());
    }
}
