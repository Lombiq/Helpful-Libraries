using Fluid;
using Fluid.Values;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Shapes;
using OrchardCore.Liquid;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Liquid;

/// <summary>
/// Tries to convert the input object into <see cref="IShape"/> and render the contents of the <see
/// cref="IShape.Metadata"/>.<see cref="ShapeMetadata.ChildContent"/> property. This is usually not accessible to Liquid
/// if the input is in some other view-model type that obscures <see cref="IShape"/> properties.
/// </summary>
/// <remarks><para>
/// You can register this with the correct name using the <see
/// cref="LiquidServiceCollectionExtensions.AddDisplayChildrenLiquidFilter"/> extension method.
/// </para></remarks>
/// <example>
/// <code>{{ Model | display-children }}</code>
/// </example>
public class DisplayChildrenLiquidFilter : ILiquidFilter
{
    public ValueTask<FluidValue> ProcessAsync(FluidValue? input, FilterArguments arguments, LiquidTemplateContext context)
    {
        if (input is not ObjectValue { Value: IShape shape }) return NilValue.Instance;

        var html = shape.Metadata.ChildContent.Html();
        var result = new StringValue(html, encode: false);
        return ValueTask.FromResult<FluidValue>(result);
    }
}
