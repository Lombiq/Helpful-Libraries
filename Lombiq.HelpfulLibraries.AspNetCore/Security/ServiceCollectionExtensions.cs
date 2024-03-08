using Lombiq.HelpfulLibraries.AspNetCore.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers a Content Security Policy provider that implements <see cref="IContentSecurityPolicyProvider"/> and
    /// will be used by <see cref="ApplicationBuilderExtensions.UseContentSecurityPolicyHeader"/>.
    /// </summary>
    public static IServiceCollection AddContentSecurityPolicyProvider<TProvider>(this IServiceCollection services)
        where TProvider : class, IContentSecurityPolicyProvider =>
        services.AddScoped<IContentSecurityPolicyProvider, TProvider>();

    /// <summary>
    /// Configures the session cookie to be always secure. With this configuration the token won't work in an HTTP
    /// environment so make sure that HTTPS redirection is enabled.
    /// </summary>
    public static IServiceCollection ConfigureSessionCookieAlwaysSecure(this IServiceCollection services) =>
        services.Configure<SessionOptions>(options => options.Cookie.SecurePolicy = CookieSecurePolicy.Always);
}
