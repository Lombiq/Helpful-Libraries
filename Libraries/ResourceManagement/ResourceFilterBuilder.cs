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
                Filter = filter
            };

            ResourceFilters.Add(resourceFilter);

            return resourceFilter;
        }
    }


    public static class ResourceFilterBuilderExtensions
    {
        public static ResourceFilter WhenPath(this ResourceFilterBuilder builder, string path) =>
            builder.When(context => context.Request.Path.Value.ToUpperInvariant() == path.ToUpperInvariant());

        public static ResourceFilter WhenHomePage(this ResourceFilterBuilder builder) =>
            builder.WhenPath("/");

        public static ResourceFilter WhenPathStartsWith(this ResourceFilterBuilder builder, string path) =>
            builder.When(context => context.Request.Path.Value.ToUpperInvariant().StartsWith(path.ToUpperInvariant()));
    }
}
