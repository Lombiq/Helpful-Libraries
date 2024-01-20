using Fluid.Values;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Implementation;
using OrchardCore.DisplayManagement.Liquid;
using System;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Liquid;

public class LiquidContentDisplayService(IDisplayHelper displayHelper, IShapeFactory shapeFactory) : ILiquidContentDisplayService
{
    private readonly IDisplayHelper _displayHelper = displayHelper;
    private readonly IShapeFactory _shapeFactory = shapeFactory;

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
        return new HtmlContentValue(content);
    }
}
