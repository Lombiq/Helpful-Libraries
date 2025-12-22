using System;
using System.Collections.Generic;
using System.Linq;

namespace OrchardCore.DisplayManagement.Implementation;

public static class ShapeExtensions
{
    public static T Get<T>(this IShape shape, string name = null)
        where T : class =>
        shape.Properties[name ?? typeof(T).Name] as T;

    public static T Get<T>(this ShapeDisplayContext context, string name = null)
        where T : class =>
        Get<T>(context.Shape, name);

    /// <summary>
    /// Creates a new instance of <typeparamref name="T"/> and sets its properties from the <paramref name="shape"/>'s
    /// <see cref="IShape.Properties"/> with the same keys.
    /// </summary>
    public static T As<T>(this IShape shape)
        where T : new()
    {
        var result = new T();

        var shapeProperties = shape.GetType().GetProperties().ToDictionary(property => property.Name);
        foreach (var property in typeof(T).GetProperties().Where(property => property.CanWrite))
        {
            if ((shape.Properties.GetMaybe(property.Name) ?? shapeProperties.GetMaybe(property.Name)?.GetValue(shape))
                is not { } value)
            {
                continue;
            }

            if (!property.PropertyType.IsInstanceOfType(value))
            {
                value = property.PropertyType == typeof(string)
                    ? value.ToString()
                    : throw new ArgumentException(
                        $"Type mismatch while attempting to convert the shape into \"{typeof(T).FullName}\". The " +
                        $"property \"{property.Name}\" expects type \"{property.PropertyType.FullName}\", but the " +
                        $"received value is \"{value.GetType().FullName}\".",
                        nameof(shape));
            }

            property.SetValue(result, value);
        }

        return result;
    }
}
