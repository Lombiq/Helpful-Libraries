using Microsoft.AspNetCore.Http;
using OrchardCore.ResourceManagement;
using System;

namespace Lombiq.HelpfulLibraries.Libraries.ResourceManagement
{
    public class ResourceFilter
    {
        public Func<HttpContext, bool> Filter { get; set; }
        public Action<IResourceManager> Execution { get; set; }


        public void Execute(Action<IResourceManager> execute) =>
            Execution = execute;
    }


    public static class ResourceFilterExtensions
    {
        public static void RegisterStylesheet(this ResourceFilter filter, string resource) =>
            filter.Execute(resourceManager => resourceManager.RegisterResource("stylesheet", resource));

        public static void RegisterFootScript(this ResourceFilter filter, string resource) =>
            filter.Execute(resourceManager => resourceManager.RegisterResource("script", resource).AtFoot());

        public static void RegisterHeadScript(this ResourceFilter filter, string resource) =>
            filter.Execute(resourceManager => resourceManager.RegisterResource("script", resource).AtHead());
    }
}
