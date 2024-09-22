using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class HostingDefaultsOrchardCoreBuilderExtensions
{
    /// <summary>
    /// Lombiq-recommended opinionated default configuration for features of a standard Orchard Core application. If
    /// any of the configuration values exist, they won't be overridden, so e.g. appsettings.json configuration will
    /// take precedence.
    /// </summary>
    /// <param name="webApplicationBuilder">The <see cref="WebApplicationBuilder"/> instance of the app.</param>
    /// <param name="hostingConfiguration">Configuration for the hosting defaults.</param>
    public static OrchardCoreBuilder ConfigureHostingDefaults(
        this OrchardCoreBuilder builder,
        WebApplicationBuilder webApplicationBuilder,
        HostingConfiguration hostingConfiguration = null)
    {
        hostingConfiguration ??= new HostingConfiguration();

        // Not using static type references for the names here because those practically never change, but we'd need to
        // add project/package references to all the affected projects.

        var ocSection = webApplicationBuilder.Configuration.GetSection("OrchardCore");

        ocSection.GetSection("OrchardCore_Localization_CultureOptions").AddValueIfKeyNotExists("IgnoreSystemSettings", "true");

        var shellsDatabaseSection = ocSection.GetSection("OrchardCore_Shells_Database");

        shellsDatabaseSection.AddValueIfKeyNotExists("DatabaseProvider", "SqlConnection");
        shellsDatabaseSection.AddValueIfKeyNotExists("TablePrefix", "Shells");

        ocSection.GetSection("OrchardCore_Tenants").AddValueIfKeyNotExists("TenantRemovalAllowed", "true");

        var logLevelSection = webApplicationBuilder.Configuration.GetSection("Logging:LogLevel");
        var elasticSearchSection = ocSection.GetSection("OrchardCore_Elasticsearch");

        if (webApplicationBuilder.Environment.IsDevelopment())
        {
            logLevelSection
                .AddValueIfKeyNotExists("Default", "Debug")
                .AddValueIfKeyNotExists("System", "Information")
                .AddValueIfKeyNotExists("Microsoft", "Information");

            // Orchard Core 1.8 and prior section. Keeping it here for leftover configs, because it keeps working under
            // 2.0 too
            var oc18SmtpSection = ocSection.GetSection("SmtpSettings");
            var smtpSection = ocSection.GetSection("OrchardCore_Email_Smtp");

            if (oc18SmtpSection["Host"] == null && smtpSection["Host"] == null)
            {
                smtpSection["IsEnabled"] = "true";
                smtpSection["Host"] = "127.0.0.1";
                smtpSection["RequireCredentials"] = "false";
                smtpSection["Port"] = "25";
            }

            smtpSection.AddValueIfKeyNotExists("DefaultSender", "sender@example.com");

            if (elasticSearchSection["Url"] == null)
            {
                elasticSearchSection["ConnectionType"] = "SingleNodeConnectionPool";
                elasticSearchSection["Url"] = "http://localhost";
                elasticSearchSection["Ports:0"] = "9200";
                elasticSearchSection["Username"] = "admin";
                elasticSearchSection["Password"] = "admin";
            }
        }
        else
        {
            logLevelSection
                .AddValueIfKeyNotExists("Default", "Warning")
                .AddValueIfKeyNotExists("Microsoft.AspNetCore", "Warning");

            ocSection.AddValueIfKeyNotExists("DatabaseProvider", "SqlConnection");

            // Elastic Cloud configuration if none is provided. The Url and Password are still needed.
            if (elasticSearchSection["ConnectionType"] == null &&
                elasticSearchSection["Ports"] == null &&
                elasticSearchSection["Username"] == null)
            {
                elasticSearchSection["ConnectionType"] = "CloudConnectionPool";
                elasticSearchSection["Ports:0"] = "9243";
                elasticSearchSection["Username"] = "elastic";
            }
        }

        if (hostingConfiguration.AlwaysEnableHealthChecksInProduction && webApplicationBuilder.Environment.IsProduction())
        {
            builder.AddTenantFeatures("OrchardCore.HealthChecks");
        }

        builder
            .AddDatabaseShellsConfigurationIfAvailable(webApplicationBuilder.Configuration)
            .ConfigureSecurityDefaultsWithStaticFiles(allowInlineStyle: true);

        return builder;
    }

    /// <summary>
    /// Lombiq-recommended opinionated default configuration for features of an Orchard Core application hosted in
    /// Azure. If any of the configuration values exist, they won't be overridden, so e.g. appsettings.json
    /// configuration will take precedence.
    /// </summary>
    /// <param name="webApplicationBuilder">The <see cref="WebApplicationBuilder"/> instance of the app.</param>
    /// <param name="hostingConfiguration">Configuration for the hosting defaults.</param>
    public static OrchardCoreBuilder ConfigureAzureHostingDefaults(
        this OrchardCoreBuilder builder,
        WebApplicationBuilder webApplicationBuilder,
        AzureHostingConfiguration hostingConfiguration = null)
    {
        hostingConfiguration ??= new AzureHostingConfiguration();

        builder.ConfigureHostingDefaults(webApplicationBuilder, hostingConfiguration);

        var ocSection = webApplicationBuilder.Configuration.GetSection("OrchardCore");

        if (!webApplicationBuilder.Environment.IsDevelopment())
        {
            ocSection.AddValueIfKeyNotExists(AzureConfigurationExtensions.IsAzureHostingKey, "true");
        }

        if (webApplicationBuilder.Configuration.IsAzureHosting())
        {
            builder
                .AddTenantFeatures(
                    "OrchardCore.DataProtection.Azure",
                    "Lombiq.Hosting.BuildVersionDisplay")
                .DisableResourceDebugMode();

            if (hostingConfiguration.AlwaysEnableAzureMediaStorage)
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

        var mediaSection = ocSection.GetSection("OrchardCore_Media_Azure");

        mediaSection.AddValueIfKeyNotExists("ContainerName", "media");
        mediaSection.AddValueIfKeyNotExists("BasePath", "{{ ShellSettings.Name }}");

        if (webApplicationBuilder.Environment.IsDevelopment())
        {
            var dataProtectionSection = ocSection.GetSection("OrchardCore_DataProtection_Azure");

            dataProtectionSection.AddValueIfKeyNotExists("CreateContainer", "true");
            dataProtectionSection.AddValueIfKeyNotExists("ConnectionString", "UseDevelopmentStorage=true");

            mediaSection.AddValueIfKeyNotExists("CreateContainer", "true");
            mediaSection.AddValueIfKeyNotExists("ConnectionString", "UseDevelopmentStorage=true");
        }

        return builder;
    }
}

public class HostingConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether to always enable <c>OrchardCore.HealthChecks</c> and its dependencies in
    /// the Production environment, for all tenants, without the ability to turn them off.
    /// </summary>
    public bool AlwaysEnableHealthChecksInProduction { get; set; } = true;
}

public class AzureHostingConfiguration : HostingConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether to always enable <c>OrchardCore.Media.Azure.Storage</c> and its
    /// dependencies when hosted in Azure, for all tenants, without the ability to turn them off.
    /// </summary>
    public bool AlwaysEnableAzureMediaStorage { get; set; } = true;
}
