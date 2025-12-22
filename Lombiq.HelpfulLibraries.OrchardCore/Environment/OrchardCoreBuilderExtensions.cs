using Microsoft.Extensions.Configuration;
using OrchardCore.ResourceManagement;

namespace Microsoft.Extensions.DependencyInjection;

public static class OrchardCoreBuilderExtensions
{
    /// <summary>
    /// Adds database shell configuration usage but only if the necessary connection string configuration is available.
    /// </summary>
    public static OrchardCoreBuilder AddDatabaseShellsConfigurationIfAvailable(
        this OrchardCoreBuilder builder,
        IConfiguration configuration)
    {
        var shellsConnectionString = configuration
            .GetValue<string>("OrchardCore:OrchardCore_Shells_Database:ConnectionString");

        if (!string.IsNullOrEmpty(shellsConnectionString)) builder.AddDatabaseShellsConfiguration();

        return builder;
    }

    /// <summary>
    /// Disables the resource debug mode, regardless of the environment.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default, in a Staging or other non-production environment <see cref="ResourceManagementOptions.DebugMode"/>
    /// would be <see langword="true"/>, but you may want resource management to be as close to Production as possible.
    /// </para>
    /// </remarks>
    public static OrchardCoreBuilder DisableResourceDebugMode(this OrchardCoreBuilder builder) =>
        builder.ConfigureServices((tenantServices, _) =>
            tenantServices.PostConfigure<ResourceManagementOptions>(settings => settings.DebugMode = false));
}
