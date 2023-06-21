using Fluid;
using Fluid.Values;
using Microsoft.AspNetCore.Html;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Implementation;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Liquid;

public class LiquidContentDisplayService : ILiquidContentDisplayService
{
    private readonly IDisplayHelper _displayHelper;
    private readonly IShapeFactory _shapeFactory;
    public LiquidContentDisplayService(IDisplayHelper displayHelper, IShapeFactory shapeFactory)
    {
        _displayHelper = displayHelper;
        _shapeFactory = shapeFactory;
    }

    public async ValueTask<FluidValue> DisplayNewAsync(
        string shapeType,
        Func<ValueTask<IShape>> shapeFactory,
        Action<ShapeCreatingContext> creating = null,
        Action<ShapeCreatedContext> created = null)
    {
        var shape = await _shapeFactory.CreateAsync(shapeType, shapeFactory, creating, created);
        return await DisplayShapeAsync(shape);
    }

    public async ValueTask<FluidValue> DisplayNewAsync<TModel>(string shapeType, Action<TModel> initialize)
    {
        var shape = await _shapeFactory.CreateAsync(shapeType, initialize);
        return await DisplayShapeAsync(shape);
    }

    public async ValueTask<FluidValue> DisplayShapeAsync(IShape shape)
    {
        var content = await _displayHelper.ShapeExecuteAsync(shape);

        await using var stringWriter = new StringWriter();
        content.WriteTo(stringWriter, HtmlEncoder.Default);

        return FluidValue.Create(new HtmlString(stringWriter.ToString()), TemplateOptions.Default);
    }
}
