using Fluid.Values;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Liquid;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Liquid;

/// <summary>
/// A service that supplies the getter for a Liquid property. Use it along with the <see
/// cref="LiquidServiceCollectionExtensions.RegisterLiquidPropertyAccessor{TService}"/> extension.
/// </summary>
public interface ILiquidPropertyRegistrar
{
    /// <summary>
    /// Gets the name of the property to be defined.
    /// </summary>
    string PropertyName { get; }

    /// <summary>
    /// Returns the <see cref="ObjectValue"/> that this property gets.
    /// </summary>
    Task<object> GetObjectAsync(LiquidTemplateContext context);
}
