using Microsoft.AspNetCore.Http;
using OrchardCore.ResourceManagement;
using System;
using System.Collections.Generic;

namespace Lombiq.HelpfulLibraries.Libraries.ResourceManagement
{
    public class ResourceFilterBuilder
    {
        public IList<ResourceFilter> ResourceFilters { get; private set; } = new List<ResourceFilter>();

        public ResourceFilter When(Func<HttpContext, bool> filter)
        {
            var resourceFilter = new ResourceFilter
            {
                Filter = filter,
            };

            ResourceFilters.Add(resourceFilter);

            return resourceFilter;
        }

        public ResourceFilter WhenPath(string path) =>
            When(context => context.Request.Path.Value.EqualsOrdinalIgnoreCase(path));

        public ResourceFilter WhenHomePage() => WhenPath("/");

        public ResourceFilter WhenPathStartsWith(string path) =>
            When(context => context.Request.Path.Value.StartsWithOrdinalIgnoreCase(path));

        public ResourceFilter Always(Action<IResourceManager> execution = null)
        {
            var filter = When(_ => true);
            filter.Execution = execution;
            return filter;
        }
    }
}
