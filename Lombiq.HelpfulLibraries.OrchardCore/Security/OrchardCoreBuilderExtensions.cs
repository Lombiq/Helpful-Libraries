using Lombiq.HelpfulLibraries.AspNetCore.Security;
using Lombiq.HelpfulLibraries.OrchardCore.DependencyInjection;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Models;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection;

public static class SecurityOrchardCoreBuilderExtensions
{
    /// <summary>
    /// Configures the anti-forgery token to be always secure. With this configuration the token won't work in an HTTP
    /// environment so make sure that HTTPS redirection is enabled.
    /// </summary>
    public static OrchardCoreBuilder ConfigureAntiForgeryAlwaysSecure(this OrchardCoreBuilder builder) =>
        builder.ConfigureServices((services, _) =>
            services.Configure<AntiforgeryOptions>(options => options.Cookie.SecurePolicy = CookieSecurePolicy.Always));

    /// <summary>
    /// Provides some default security configuration for Orchard Core.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///     <item>
    ///         <description>
    ///             Add <see cref="CdnContentSecurityPolicyProvider"/> to permit using script and style resources from
    ///             some common CDNs.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             Make the session token's cookie always secure.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             Make the anti-forgery token's cookie always secure.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             Enable the <c>OrchardCore.Diagnostics</c> feature to provide custom error screens in production and
    ///             don't leak error information.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///            Adds a middleware that supplies the <c>Content-Security-Policy</c> header.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             Adds a middleware that supplies the <c>X-Content-Type-Options: nosniff</c> header.
    ///         </description>
    ///     </item>
    /// </list>
    /// </remarks>
    public static OrchardCoreBuilder ConfigureSecurityDefaults(
        this OrchardCoreBuilder builder,
        bool allowInlineScript = true,
        bool allowInlineStyle = false)
    {
        builder.ApplicationServices.AddInlineStartup(
            services => services
                .AddContentSecurityPolicyProvider<CdnContentSecurityPolicyProvider>()
                .ConfigureSessionCookieAlwaysSecure(),
            (app, _, serviceProvider) =>
            {
                // Don't add any middlewares if the site is in setup mode.
                var shellSettings = serviceProvider
                    .GetRequiredService<IShellHost>()
                    .GetAllSettings()
                    .FirstOrDefault(settings => settings.Name == "Default");
                if (shellSettings?.State == TenantState.Uninitialized) return;

                app
                    .UseContentSecurityPolicyHeader(allowInlineScript, allowInlineStyle)
                    .UseNosniffContentTypeOptionsHeader();
            },
            order: 99);
        return builder
            .ConfigureAntiForgeryAlwaysSecure()
            .AddTenantFeatures("OrchardCore.Diagnostics");
    }
}
