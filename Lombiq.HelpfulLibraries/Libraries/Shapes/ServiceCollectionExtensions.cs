using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Lombiq.HelpfulLibraries.Libraries.Shapes;

public static class ServiceCollectionExtensions
{
    public static void AddShapeRenderer(this IServiceCollection services) =>
        services.TryAddScoped<IShapeRenderer, ShapeRenderer>();
}
