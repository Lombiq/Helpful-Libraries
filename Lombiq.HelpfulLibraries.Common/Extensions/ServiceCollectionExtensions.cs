using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Shortcuts to remove implementations from an <see cref="IServiceCollection"/> instance.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Removes implementations of type <typeparamref name="T"/> from an <see cref="IServiceCollection"/> instance.
    /// </summary>
    public static IServiceCollection RemoveImplementations<T>(this IServiceCollection services) =>
        RemoveImplementations(services, typeof(T).FullName);

    /// <summary>
    /// Removes the implementations specified in <paramref name="serviceFullName"/> from an
    /// <see cref="IServiceCollection"/> instance.
    /// </summary>
    public static IServiceCollection RemoveImplementations(this IServiceCollection services, string serviceFullName)
    {
        var servicesToRemove = services
            .Where(service => service.ServiceType?.FullName == serviceFullName)
            .ToList();

        servicesToRemove.ForEach(service => services.Remove(service));

        return services;
    }

    /// <summary>
    /// Attempts to remove all service implementations from an <see cref="IServiceCollection"/> instance except for the
    /// type of the specified <typeparamref name="TImplementation"/>.
    /// </summary>
    public static IServiceCollection RemoveImplementationsExcept<TService, TImplementation>(this IServiceCollection services) =>
        RemoveImplementationsExcept<TService>(services, typeof(TImplementation).FullName);

    /// <summary>
    /// Attempts to remove all service implementations from an <see cref="IServiceCollection"/> instance except for the
    /// one specified in <paramref name="keepImplementationTypeFullName"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if there is no service registered that matches <paramref name="keepImplementationTypeFullName"/>.
    /// </exception>
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
