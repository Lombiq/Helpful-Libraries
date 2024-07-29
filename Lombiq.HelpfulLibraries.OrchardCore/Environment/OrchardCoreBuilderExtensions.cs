using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
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

    /// <summary>
    /// Recommended default configuration for features of a standard Orchard Core application. If any of the
    /// configuration values exist, they won't be overridden, so e.g. appsettings.json configuration will take
    /// precedence.
    /// </summary>
    /// <param name="webApplicationBuilder">The <see cref="WebApplicationBuilder"/> instance of the app.</param>
    /// <param name="enableHealthChecksInProduction">
    /// Indicates whether to enable <c>OrchardCore.HealthChecks</c> in the Production environment.
    /// </param>
    public static OrchardCoreBuilder ConfigureHostingDefaults(
        this OrchardCoreBuilder builder,
        WebApplicationBuilder webApplicationBuilder,
        bool enableHealthChecksInProduction = true)
    {
        var ocSection = webApplicationBuilder.Configuration.GetSection("OrchardCore");

        ocSection.GetSection("OrchardCore_Tenants").AddValueIfKeyNotExists("TenantRemovalAllowed", "true");

        if (webApplicationBuilder.Environment.IsDevelopment())
        {
            // Orchard Core 1.8 and prior, this can be removed after an Orchard Core upgrade to 2.0.
            // OrchardCore_Email_Smtp below is 2.0+.
            var oc18SmtpSection = ocSection.GetSection("SmtpSettings");

            if (oc18SmtpSection["Host"] == null)
            {
                oc18SmtpSection["Host"] = "127.0.0.1";
                oc18SmtpSection["RequireCredentials"] = "false";
                oc18SmtpSection["Port"] = "25";
            }

            oc18SmtpSection.AddValueIfKeyNotExists("DefaultSender", "sender@example.com");

            var smtpSection = ocSection.GetSection("OrchardCore_Email_Smtp");

            if (smtpSection["Host"] == null)
            {
                smtpSection["Host"] = "127.0.0.1";
                smtpSection["RequireCredentials"] = "false";
                smtpSection["Port"] = "25";
            }

            smtpSection.AddValueIfKeyNotExists("DefaultSender", "sender@example.com");
        }

        if (enableHealthChecksInProduction && webApplicationBuilder.Environment.IsProduction())
        {
            builder.AddTenantFeatures("OrchardCore.HealthChecks");
        }

        builder
            .AddDatabaseShellsConfigurationIfAvailable(webApplicationBuilder.Configuration)
            .ConfigureSmtpSettings(overrideAdminSettings: false)
            .ConfigureSecurityDefaultsWithStaticFiles(allowInlineStyle: true);

        return builder;
    }

    /// <summary>
    /// Recommended default configuration for features of an Orchard Core application hosted in Azure. If any of the
    /// configuration values exist, they won't be overridden, so e.g. appsettings.json configuration will take
    /// precedence.
    /// </summary>
    /// <param name="webApplicationBuilder">The <see cref="WebApplicationBuilder"/> instance of the app.</param>
    /// <param name="enableAzureMediaStorage">
    /// Indicates whether to enable <c>OrchardCore.Media.Azure.Storage</c> and its dependencies when hosted in Azure.
    /// </param>
    /// <param name="enableHealthChecksInProduction">
    /// Indicates whether to enable <c>OrchardCore.HealthChecks</c> in the Production environment.
    /// </param>
    public static OrchardCoreBuilder ConfigureAzureHostingDefaults(
        this OrchardCoreBuilder builder,
        WebApplicationBuilder webApplicationBuilder,
        bool enableAzureMediaStorage = true,
        bool enableHealthChecksInProduction = true)
    {
        builder.ConfigureHostingDefaults(webApplicationBuilder);

        var ocSection = webApplicationBuilder.Configuration.GetSection("OrchardCore");

        if (webApplicationBuilder.Configuration.IsAzureHosting())
        {
            builder
                .AddTenantFeatures(
                    "OrchardCore.DataProtection.Azure",
                    "Lombiq.Hosting.BuildVersionDisplay")
                .DisableResourceDebugMode();

            if (enableAzureMediaStorage)
            {
                // Azure Media Storage and its dependencies. Keep this updated with Orchard upgrades.
                builder.AddTenantFeatures(
                    "OrchardCore.Contents",
                    "OrchardCore.ContentTypes",
                    "OrchardCore.Liquid",
                    "OrchardCore.Media",
                    "OrchardCore.Media.Azure.Storage",
                    "OrchardCore.Media.Cache",
                    "OrchardCore.Settings");
            }
        }

        return builder;
    }
}
