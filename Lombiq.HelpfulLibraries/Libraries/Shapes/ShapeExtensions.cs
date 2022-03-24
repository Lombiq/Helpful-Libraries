using OrchardCore.DisplayManagement;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.HelpfulLibraries.Libraries.Shapes;

public static class ShapeExtensions
{
    /// <summary>
    /// Creates a new instance of <typeparamref name="T"/> and sets its properties from the <paramref name="shape"/>'s
    /// <see cref="IShape.Properties"/> with the same keys.
    /// </summary>
    public static T As<T>(this IShape shape)
        where T : new()
    {
        var result = new T();

        var shapeProperties = shape.GetType().GetProperties().ToDictionary(property => property.Name);
        foreach (var property in typeof(T).GetProperties())
        {
            if ((shape.Properties.GetMaybe(property.Name) ?? shapeProperties.GetMaybe(property.Name)?.GetValue(shape)) is { } value)
            {
                property.SetValue(result, value);
            }
        }

        return result;
    }
}
