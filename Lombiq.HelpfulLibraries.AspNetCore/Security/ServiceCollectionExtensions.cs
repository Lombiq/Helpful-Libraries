using Lombiq.HelpfulLibraries.AspNetCore.Security;
using Microsoft.AspNetCore.Builder;

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
    /// Registers <see cref="AntiClickjackingContentSecurityPolicyProvider"/>.
    /// </summary>
    public static IServiceCollection AddAntiClickjackingContentSecurityPolicyProvider(this IServiceCollection services) =>
        services.AddContentSecurityPolicyProvider<AntiClickjackingContentSecurityPolicyProvider>();
}
