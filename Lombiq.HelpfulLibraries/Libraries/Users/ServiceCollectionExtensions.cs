using Microsoft.Extensions.DependencyInjection;

namespace Lombiq.HelpfulLibraries.Libraries.Users
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCachingUserServer(this IServiceCollection services) =>
            services.AddScoped<ICachingUserManager, CachingUserManager>();
    }
}
