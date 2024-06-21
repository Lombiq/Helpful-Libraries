using Microsoft.Extensions.Configuration;
using OrchardCore.Email;
using OrchardCore.Environment.Shell.Configuration;
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
    /// Configures SMTP settings (<see cref="SmtpSettings"/>) from the configuration provider.
    /// </summary>
    /// <param name="overrideAdminSettings">
    /// If set to <see langword="true"/> the settings coming from the configuration provider will override the ones set
    /// up from the admin UI.
    /// </param>
    public static OrchardCoreBuilder ConfigureSmtpSettings(
        this OrchardCoreBuilder builder,
        bool overrideAdminSettings = true)
    {
        builder.ConfigureServices((tenantServices, serviceProvider) =>
        {
            var shellConfiguration = serviceProvider.GetRequiredService<IShellConfiguration>().GetSection("SmtpSettings");
            tenantServices.PostConfigure<SmtpSettings>(settings =>
            {
                if (!overrideAdminSettings && !string.IsNullOrEmpty(settings.Host)) return;
                shellConfiguration.Bind(settings);
            });
        });

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
