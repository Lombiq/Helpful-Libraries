using Fluid;
using Fluid.Values;
using Lombiq.HelpfulLibraries.OrchardCore.Liquid;
using OrchardCore.DisplayManagement.Liquid;
using OrchardCore.Liquid;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection;

public static class LiquidServiceCollectionExtensions
{
    /// <summary>
    /// Allows registering a new Liquid property with the provided <paramref name="name"/>.
    /// </summary>
    public static IServiceCollection RegisterLiquidPropertyAccessor<TService>(this IServiceCollection services, string name)
        where TService : class, ILiquidPropertyRegistrar
    {
        services.AddScoped<ILiquidPropertyRegistrar, TService>();
        return services.Configure<TemplateOptions>(options =>
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
                }));
    }

    /// <summary>
    /// Configures the <see cref="LiquidViewOptions"/> with an additional parser tag.
    /// </summary>
    public static IServiceCollection AddLiquidParserTag<T>(this IServiceCollection services, string tagName)
            where T : class, ILiquidParserTag
    {
        services.AddScoped<ILiquidParserTag, T>();
        services.AddKeyedScoped<ILiquidParserTag, T>(tagName);

        return services.Configure<LiquidViewOptions>(options =>
            options.LiquidViewParserConfiguration.Add(parser => parser.RegisterParserTag(
                tagName,
                parser.ArgumentsListParser,
                (arguments, writer, encoder, context) =>
                {
                    var provider = ((LiquidTemplateContext)context).Services;
                    var service = provider.GetKeyedService<ILiquidParserTag>(tagName);
                    return service.WriteToAsync(arguments, writer, encoder, context);
                })));
    }
}
