using Microsoft.Extensions.DependencyInjection;

namespace System;

public static class ServiceProviderExtensions
{
    /// <summary>
    /// Returns a <see cref="Lazy{T}"/> accessor for the service so you can access services with a shorter lifecyle in
    /// your service implementation without storing a service provider which is an anti-pattern.
    /// </summary>
    /// <typeparam name="T">The type of the required service.</typeparam>
    public static Lazy<T> GetLazyService<T>(this IServiceProvider serviceProvider) =>
        new(serviceProvider.GetRequiredService<T>);
}
