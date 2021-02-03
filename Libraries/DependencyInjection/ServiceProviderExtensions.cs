using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Scope;
using RestEase;
using System.Threading.Tasks;

namespace System
{
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Executes <paramref name="asyncAction"/> in the specified shell's scope.
        /// </summary>
        public static async Task WithShellScopeAsync(
            this IServiceProvider serviceProvider,
            Func<ShellScope, Task> asyncAction,
            string scopeName = "Default")
        {
            var shellHost = serviceProvider.GetRequiredService<IShellHost>();
            var shellScope = await shellHost.GetScopeAsync(scopeName);
            await shellScope.UsingAsync(asyncAction);
        }

        public static async Task<T> GetWithShellScopeAsync<T>(
            this IServiceProvider serviceProvider,
            Func<ShellScope, Task<T>> asyncFunc,
            string scopeName = "Default")
        {
            T result = default;

            await serviceProvider.WithShellScopeAsync(async scope => result = await asyncFunc(scope), scopeName);

            return result;
        }

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
            Func<string> getBaseUrl)
            where TClient : class =>
            services.AddHttpClient(name, client =>
                {
                    client.BaseAddress = new Uri(getBaseUrl());
                })
                .AddTypedClient(RestClient.For<TClient>);
    }
}
