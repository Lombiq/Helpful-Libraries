using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Lombiq.HelpfulLibraries.OrchardCore.Users;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="ICachingUserManager"/> and its implementation <see cref="CachingUserManager"/> to the service
    /// collection, making them available for use.
    /// </summary>
    public static void AddCachingUserServer(this IServiceCollection services) =>
        services.TryAddScoped<ICachingUserManager, CachingUserManager>();
}
