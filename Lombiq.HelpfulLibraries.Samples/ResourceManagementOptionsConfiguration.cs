using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;

namespace Lombiq.HelpfulLibraries.Samples;

public class ResourceManagementOptionsConfiguration : IConfigureOptions<ResourceManagementOptions>
{
    private static readonly ResourceManifest _manifest = new();

    static ResourceManagementOptionsConfiguration()
    {
        _manifest.DefineScriptModule("parent").SetUrl("~/Lombiq.HelpfulLibraries.Samples/parent.js");
        _manifest.DefineScriptModule("child").SetUrl("~/Lombiq.HelpfulLibraries.Samples/child.js");
    }

    public void Configure(ResourceManagementOptions options) => options.ResourceManifests.Add(_manifest);
}
