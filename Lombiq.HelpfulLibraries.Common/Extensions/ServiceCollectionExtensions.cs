using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Shortcuts to remove implementations from an <see cref="IServiceCollection"/> instance.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection RemoveImplementations<T>(this IServiceCollection services) =>
        RemoveImplementations(services, typeof(T).FullName);

    public static IServiceCollection RemoveImplementations(this IServiceCollection services, string serviceFullName)
    {
        var servicesToRemove = services
            .Where(service => service.ServiceType?.FullName == serviceFullName)
            .ToList();

        servicesToRemove.ForEach(service => services.Remove(service));

        return services;
    }

    public static IServiceCollection RemoveImplementationsExcept<TService, TImplementation>(this IServiceCollection services) =>
        RemoveImplementationsExcept<TService>(services, typeof(TImplementation).FullName);

    public static IServiceCollection RemoveImplementationsExcept<TService>(
        this IServiceCollection services,
        string keepImplementationTypeFullName)
    {
        if (!services.Any(service => service.ImplementationType?.FullName == keepImplementationTypeFullName))
        {
            throw new InvalidOperationException("There is no service registered that matches " +
                $"{keepImplementationTypeFullName}. This will make the service {typeof(TService).Name} unresolvable.");
        }

        var servicesToRemove = services
            .Where(service => service.ServiceType == typeof(TService) && service.ImplementationType.FullName != keepImplementationTypeFullName)
            .ToList();

        servicesToRemove.ForEach(service => services.Remove(service));

        return services;
    }
}
