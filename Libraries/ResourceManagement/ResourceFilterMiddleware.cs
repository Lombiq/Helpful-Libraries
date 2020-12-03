using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ResourceManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.Libraries.ResourceManagement
{
    public class ResourceFilterMiddleware
    {
        private readonly RequestDelegate _next;

        public ResourceFilterMiddleware(RequestDelegate next) => _next = next;

        public Task InvokeAsync(HttpContext context)
        {
            var resourceFilterProviders = context.RequestServices.GetService<IEnumerable<IResourceFilterProvider>>();

            if (resourceFilterProviders?.Any() == true)
            {
                var builder = new ResourceFilterBuilder();

                foreach (var provider in resourceFilterProviders)
                {
                    provider.AddResourceFilter(builder);
                }

                var activeFilters = builder.ResourceFilters.Where(filter => filter.Filter(context));

                if (activeFilters.Any())
                {
                    var resourceManager = context.RequestServices.GetService<IResourceManager>();

                    foreach (var filter in activeFilters)
                    {
                        filter.Execution(resourceManager);
                    }
                }
            }

            return _next(context);
        }
    }
}
