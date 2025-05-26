using Fluid;
using Fluid.Values;
using OrchardCore.Liquid;
using System;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Liquid;

public static class TemplateOptionsExtensions
{
    public static void RegisterLiquidPropertyAccessor<TBase>(
        this TemplateOptions options,
        string propertyName,
        Func<string, LiquidTemplateContext, Task<FluidValue>> getter) =>
        options.MemberAccessStrategy.Register<TBase, LiquidPropertyAccessor>(propertyName, (_, context) =>
        {
            var liquidTemplateContext = (LiquidTemplateContext)context;

            return new LiquidPropertyAccessor(liquidTemplateContext, getter);
        });

    public static void RegisterLiquidPropertyAccessor<TBase, TResult>(
        this TemplateOptions options,
        string propertyName,
        Func<string, LiquidTemplateContext, Task<TResult>> getter) =>
        options.RegisterLiquidPropertyAccessor<TBase>(
            propertyName,
            async (value, context) => FluidValue.Create(await getter(value, context), context.Options));
}
