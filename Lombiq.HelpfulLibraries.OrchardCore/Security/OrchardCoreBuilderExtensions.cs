using Lombiq.HelpfulLibraries.AspNetCore.Security;
using Lombiq.HelpfulLibraries.OrchardCore.DependencyInjection;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

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
    /// Provides some default security configuration for Orchard Core. Use it in conjunction with <see
    /// cref="SecurityApplicationBuilderExtensions.UseSecurityDefaults"/>.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///     <item>
    ///         <description>
    ///             Add <see cref="AntiClickjackingContentSecurityPolicyProvider"/> to prevent clickjacking.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             Call <see cref="ConfigureAntiForgeryAlwaysSecure"/> to make the anti-forgery token secure.
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
    public static OrchardCoreBuilder ConfigureSecurityDefaults(this OrchardCoreBuilder builder)
    {
        builder.ApplicationServices.AddInlineStartup(
            services => services.AddAntiClickjackingContentSecurityPolicyProvider(),
            app => app
                .UseContentSecurityPolicyHeader(allowInline: true)
                .UseNosniffContentTypeOptionsHeader(),
            order: 99);
        return builder
            .ConfigureAntiForgeryAlwaysSecure()
            .AddTenantFeatures("OrchardCore.Diagnostics");
    }
}
