using Lombiq.HelpfulLibraries.Common.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Lombiq.HelpfulLibraries.OrchardCore.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="IOrchardServices{T}"/> and its implementation <see cref="OrchardServices{T}"/> to the
    /// service collection, making them available for use. Also enables lazy dependency injection.
    /// </summary>
    public static void AddOrchardServices(this IServiceCollection services)
    {
        services.AddLazyInjectionSupport();
        services.TryAddTransient(typeof(IOrchardServices<>), typeof(OrchardServices<>));
    }

    /// <summary>
    /// Configures <see cref="MvcOptions"/> to add the <typeparamref name="TFilter"/> to the list of filters.
    /// </summary>
    public static void AddAsyncResultFilter<TFilter>(this IServiceCollection services)
        where TFilter : IAsyncResultFilter =>
        services.Configure<MvcOptions>(options => options.Filters.Add(typeof(TFilter)));
}
