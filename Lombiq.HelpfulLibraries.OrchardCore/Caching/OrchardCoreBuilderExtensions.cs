using Microsoft.Extensions.DependencyInjection;

namespace Lombiq.HelpfulLibraries.OrchardCore.Caching;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCachedWebRoot(this IServiceCollection services)
    {
        services.AddSingleton<CachedWebRootFileProvider>();
        services.AddMemoryCache();
        return services;
    }
}

//public static class OrchardCoreBuilderExtensions
//{
//    public static OrchardCoreBuilder AddCachedWebRoot(this OrchardCoreBuilder builder)
//    {
//        builder.ConfigureServices((tenantServices, serviceProvider) =>
//        {
//            tenantServices.AddSingleton<CachedWebRootFileProvider>();

//        });

//        return builder;
//    }
//}
