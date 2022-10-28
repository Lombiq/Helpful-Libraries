using Microsoft.Extensions.Configuration;
using OrchardCore.Email;
using OrchardCore.Environment.Shell.Configuration;

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
}
