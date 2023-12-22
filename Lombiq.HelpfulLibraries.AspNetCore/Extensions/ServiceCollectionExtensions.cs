using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Lombiq.HelpfulLibraries.AspNetCore.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures <see cref="MvcOptions"/> to add the <typeparamref name="TFilter"/> to the list of filters.
    /// </summary>
    public static void AddAsyncResultFilter<TFilter>(this IServiceCollection services)
        where TFilter : IAsyncResultFilter =>
        services.Configure<MvcOptions>(options => options.Filters.Add(typeof(TFilter)));
}
