using Lombiq.HelpfulLibraries.AspNetCore.Security;
using Lombiq.HelpfulLibraries.OrchardCore.DependencyInjection;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
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
    /// <para>This extension method configures the application as listed below.</para>
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
    /// <para>
    /// If you also need static file support, consider using <see cref="ConfigureSecurityDefaultsWithStaticFiles"/>
    /// instead. Alternatively, make sure to put the <c>app.UseStaticFiles()</c> call at the very end of your app
    /// configuration chain so it won't short-circuit prematurely and miss adding security headers to your static files.
    /// </para>
    /// </remarks>
    public static OrchardCoreBuilder ConfigureSecurityDefaults(
        this OrchardCoreBuilder builder,
        bool allowInlineScript = true,
        bool allowInlineStyle = false) =>
        builder.ConfigureSecurityDefaultsInner(allowInlineScript, allowInlineStyle, useStaticFiles: false);

    /// <summary>
    /// The same as <see cref="ConfigureSecurityDefaults"/>, but also registers the <see cref="StaticFileMiddleware"/>
    /// at the end of the chain, so <c>app.UseStaticFiles()</c> should not be called when this is used. This is helpful
    /// because <see cref="StaticFileMiddleware"/> short-circuits the call chain when delivering static files, so later
    /// middlewares are not executed (e.g. the <c>X-Content-Type-Options: nosniff</c> header wouldn't be added).
    /// </summary>
    public static OrchardCoreBuilder ConfigureSecurityDefaultsWithStaticFiles(
        this OrchardCoreBuilder builder,
        bool allowInlineScript = true,
        bool allowInlineStyle = false) =>
        builder.ConfigureSecurityDefaultsInner(allowInlineScript, allowInlineStyle, useStaticFiles: true);

    private static OrchardCoreBuilder ConfigureSecurityDefaultsInner(
        this OrchardCoreBuilder builder,
        bool allowInlineScript,
        bool allowInlineStyle,
        bool useStaticFiles)
    {
        builder.ApplicationServices.AddInlineStartup(
            services => services
                .AddContentSecurityPolicyProvider<CdnContentSecurityPolicyProvider>()
                .AddContentSecurityPolicyProvider<VueContentSecurityPolicyProvider>()
                .AddContentSecurityPolicyProvider<ContentSecurityPolicyAttributeContentSecurityPolicyProvider>()
                .AddContentSecurityPolicyProvider<SkipContentSecurityPolicyProvider>()
                .ConfigureSessionCookieAlwaysSecure(),
            (app, _, serviceProvider) =>
            {
                // Don't add any middlewares if the site is in setup mode.
                var shellSettings = serviceProvider
                    .GetRequiredService<IShellHost>()
                    .GetAllSettings()
                    .FirstOrDefault(settings => settings.Name == ShellSettings.DefaultShellName);
                if (shellSettings?.State == TenantState.Uninitialized) return;

                app
                    .UseContentSecurityPolicyHeader(allowInlineScript, allowInlineStyle)
                    .UseNosniffContentTypeOptionsHeader()
                    .UseStrictAndSecureCookies();

                if (useStaticFiles) app.UseStaticFiles();
            },
            order: 99); // Makes this service load fairly late. This should make the setup detection more accurate.

        return builder
            .ConfigureAntiForgeryAlwaysSecure()
            .AddTenantFeatures("OrchardCore.Diagnostics");
    }
}
