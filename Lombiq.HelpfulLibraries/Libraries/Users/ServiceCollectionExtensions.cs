using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Lombiq.HelpfulLibraries.Libraries.Users;

public static class ServiceCollectionExtensions
{
    public static void AddCachingUserServer(this IServiceCollection services) =>
        services.TryAddScoped<ICachingUserManager, CachingUserManager>();
}
