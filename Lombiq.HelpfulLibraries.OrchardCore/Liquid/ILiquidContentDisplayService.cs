using Fluid.Values;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Implementation;
using System;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Liquid;

/// <summary>
/// Service that displays shapes as HTML in a <see cref="FluidValue"/>.
/// </summary>
public interface ILiquidContentDisplayService
{
    /// <summary>
    /// Creates new instances of a dynamic shape object and renders it to HTML in a <see cref="FluidValue"/>.
    /// </summary>
    ValueTask<FluidValue> DisplayShapeAsync(IShape shape);

    /// <summary>
    /// Creates new instances of a typed shape object and renders it to HTML in a <see cref="FluidValue"/>.
    /// </summary>
    /// <typeparam name="TModel">The type to instantiate.</typeparam>
    ValueTask<FluidValue> DisplayNewAsync<TModel>(string shapeType, Action<TModel> initialize);

    /// <summary>
    /// Displays an already instantiated <see cref="IShape"/> as a <see cref="FluidValue"/>.
    /// </summary>
    ValueTask<FluidValue> DisplayNewAsync(
        string shapeType,
        Func<ValueTask<IShape>> shapeFactory,
        Action<ShapeCreatingContext> creating = null,
        Action<ShapeCreatedContext> created = null);
}
