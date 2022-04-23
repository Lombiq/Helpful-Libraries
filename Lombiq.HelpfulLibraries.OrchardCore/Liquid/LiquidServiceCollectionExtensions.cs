using Fluid;
using Fluid.Values;
using Lombiq.HelpfulLibraries.OrchardCore.Liquid;
using OrchardCore.Liquid;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection;

public static class LiquidServiceCollectionExtensions
{
    public static void RegisterLiquidPropertyAccessor<TService>(this IServiceCollection services, string name)
        where TService : class, ILiquidPropertyRegistrar
    {
        services.AddScoped<ILiquidPropertyRegistrar, TService>();
        services.Configure<TemplateOptions>(options =>
        {
            options
                .MemberAccessStrategy
                .Register<LiquidContentAccessor, LiquidPropertyAccessor>(name, (_, context) =>
                {
                    var liquidTemplateContext = (LiquidTemplateContext)context;

                    return new LiquidPropertyAccessor(liquidTemplateContext, async (_, context) =>
                    {
                        var registrar = context
                            .Services
                            .GetRequiredService<IEnumerable<ILiquidPropertyRegistrar>>()
                            .Single();

                        return await registrar.GetObjectAsync(context) is { } result
                            ? new ObjectValue(result)
                            : NilValue.Instance;
                    });
                });
        });
    }
}
