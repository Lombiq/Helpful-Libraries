using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Lombiq.HelpfulLibraries.OrchardCore.Shapes;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="IShapeRenderer"/> and its implementation <see cref="ShapeRenderer"/> to the service collection,
    /// making them available for use.
    /// </summary>
    public static void AddShapeRenderer(this IServiceCollection services) =>
        services.TryAddScoped<IShapeRenderer, ShapeRenderer>();
}
