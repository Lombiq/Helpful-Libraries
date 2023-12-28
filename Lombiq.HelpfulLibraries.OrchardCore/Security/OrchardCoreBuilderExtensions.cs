using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

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
    public static OrchardCoreBuilder ConfigureSecurityDefaults(this OrchardCoreBuilder builder) => builder
        .ConfigureServices(services => services.AddAntiClickjackingContentSecurityPolicyProvider())
        .ConfigureAntiForgeryAlwaysSecure()
        .AddTenantFeatures("OrchardCore.Diagnostics");
}
