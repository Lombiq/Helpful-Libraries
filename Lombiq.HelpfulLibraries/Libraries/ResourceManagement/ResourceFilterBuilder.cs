using Microsoft.AspNetCore.Http;
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
            When(context => context.Request.Path.Value.ToUpperInvariant() == path.ToUpperInvariant());

        public ResourceFilter WhenHomePage() => WhenPath("/");

        public ResourceFilter WhenPathStartsWith(string path) =>
            When(context => context.Request.Path.Value.ToUpperInvariant().StartsWith(path.ToUpperInvariant(), StringComparison.OrdinalIgnoreCase));
    }
}
