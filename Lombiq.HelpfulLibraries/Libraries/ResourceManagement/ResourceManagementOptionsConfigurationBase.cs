using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;

namespace Lombiq.HelpfulLibraries.Libraries.ResourceManagement
{
    public class ResourceManagementOptionsConfigurationBase : IConfigureOptions<ResourceManagementOptions>
    {
        protected static readonly ResourceManifest manifest = new();

        public void Configure(ResourceManagementOptions options) => options.ResourceManifests.Add(manifest);
    }
}
