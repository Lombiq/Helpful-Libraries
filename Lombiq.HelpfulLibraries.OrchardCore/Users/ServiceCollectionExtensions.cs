using Lombiq.HelpfulLibraries.Common.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Lombiq.HelpfulLibraries.OrchardCore.Users;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="ICachingUserManager"/> and its implementation <see cref="CachingUserManager"/> to the service
    /// collection, making them available for use. Since <see cref="CachingUserManager"/> uses Lazy initialization it
    /// also calls <see cref="Common.DependencyInjection.ServiceCollectionExtensions.AddLazyInjectionSupport"/>.
    /// </summary>
    public static void AddCachingUserServer(this IServiceCollection services)
    {
        services.AddLazyInjectionSupport();
        services.TryAddScoped<ICachingUserManager, CachingUserManager>();
    }
}
