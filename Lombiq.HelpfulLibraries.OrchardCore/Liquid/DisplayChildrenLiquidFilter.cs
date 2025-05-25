using Fluid;
using Fluid.Values;
using Microsoft.AspNetCore.Mvc.Localization;
using OrchardCore.DisplayManagement;
using OrchardCore.Liquid;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Liquid;

public class DisplayChildrenLiquidFilter : ILiquidFilter
{
    public ValueTask<FluidValue> ProcessAsync(FluidValue input, FilterArguments arguments, LiquidTemplateContext context)
    {
        if (input is not ObjectValue { Value: IShape shape }) return NilValue.Instance;

        var html = shape.Metadata.ChildContent.Html();
        var result = new StringValue(html, encode: false);
        return ValueTask.FromResult<FluidValue>(result);
    }
}
