using OrchardCore.DisplayManagement;
using System.Linq;

namespace Lombiq.HelpfulLibraries.Libraries.Shapes;

public static class ShapeExtensions
{
    /// <summary>
    /// Creates a new instance of <typeparamref name="T"/> and sets its properties from the <paramref name="shape"/>'s
    /// <see cref="IShape.Properties"/> with the same keys.
    /// </summary>
    public static T PropertiesAs<T>(this IShape shape)
        where T : new()
    {
        var result = new T();

        foreach (var property in typeof(T).GetProperties())
        {
            if (shape.Properties.FirstOrDefault(pair => pair.Key == property.Name) is var (key, value) && key == property.Name)
            {
                property.SetValue(result, value);
            }
        }

        return result;
    }
}
