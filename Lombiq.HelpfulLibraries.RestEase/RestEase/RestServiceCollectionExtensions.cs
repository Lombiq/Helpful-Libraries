using Microsoft.Extensions.DependencyInjection;
using RestEase;
using System;

namespace Lombiq.HelpfulLibraries.RestEase;

public static class RestServiceCollectionExtensions
{
    /// <summary>
    /// Adds a typed HTTP client to the service collection using RestEase.
    /// </summary>
    /// <typeparam name="TClient">API client type.</typeparam>
    /// <param name="services">Service collection.</param>
    /// <param name="name">Name of the HTTP client.</param>
    /// <param name="getBaseUrl">Function that returns the base URL of the API to be called with the client.</param>
    /// <returns>HTTP client builder.</returns>
    public static IHttpClientBuilder AddRestEaseHttpClient<TClient>(
        this IServiceCollection services,
        string name,
        Func<IServiceProvider, string> getBaseUrl)
        where TClient : class =>
        services
            .AddHttpClient(name, (provider, client) => client.BaseAddress = new Uri(getBaseUrl(provider)))
            .AddTypedClient(RestClient.For<TClient>);
}
