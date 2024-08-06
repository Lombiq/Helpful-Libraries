using Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.DisplayManagement.Descriptors;
using OrchardCore.DisplayManagement.Shapes;
using OrchardCore.DisplayManagement.Theming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

    /// <summary>
    /// Creates a new shape with the given <paramref name="type"/>. If the type is not yet in the <paramref
    /// name="shapeTable"/>, then a new descriptor is added with binding that uses <paramref name="displayAsync"/>.
    /// If <paramref name="type"/> is null or empty, a new random unique name is generated.
    /// </summary>
    [Obsolete($"This no longer works with the {nameof(DefaultShapeTableManager)}. Use {nameof(HtmlShape)} instead.")]
    public static IShape CreateAdHocShape(this ShapeTable shapeTable, string type, Func<DisplayContext, Task<IHtmlContent>> displayAsync)
    {
        if (string.IsNullOrEmpty(type)) type = $"AdHocShape_{Guid.NewGuid():D}";

        var shape = new Shape
        {
            Metadata =
            {
                Type = type,
            },
        };

        if (shapeTable.Descriptors.ContainsKey(type)) return shape;

        var shapeDescriptor = new ShapeDescriptor
        {
            ShapeType = type,
            Bindings =
            {
                [type] = new ShapeBinding
                {
                    BindingName = type,
                    BindingAsync = displayAsync,
                },
            },
        };

        shapeTable.Descriptors[shapeDescriptor.ShapeType] = shapeDescriptor;
        foreach (var binding in shapeDescriptor.Bindings)
        {
            shapeTable.Bindings[binding.Key] = binding.Value;
        }

        return shape;
    }

    /// <summary>
    /// Creates a new shape with the given <paramref name="type"/> in the shape table of the current front-end theme. If
    /// the type is not yet in the theme's <see cref="ShapeTable"/>, then a new descriptor is added with binding that
    /// uses <paramref name="displayAsync"/>. If <paramref name="type"/> is null or empty, a new random unique name is
    /// generated.
    /// </summary>
    [Obsolete($"This no longer works with the {nameof(DefaultShapeTableManager)}. Use {nameof(HtmlShape)} instead.")]
    public static async Task<IShape> CreateAdHocShapeForCurrentThemeAsync(
        this IServiceProvider provider,
        string type,
        Func<DisplayContext, Task<IHtmlContent>> displayAsync)
    {
        var themeManager = provider.GetRequiredService<IThemeManager>();
        var shapeTableManager = provider.GetRequiredService<IShapeTableManager>();

        var theme = await themeManager.GetThemeAsync();
        var shapeTable = await shapeTableManager.GetShapeTableAsync(theme.Id);

        return shapeTable.CreateAdHocShape(type, displayAsync);
    }

    /// <summary>
    /// Adds the warning to the screen which says "The website might be restarted upon saving the settings, potentially
    /// leading to temporary unresponsiveness during the process.".
    /// </summary>
    public static void AddTenantReloadWarning(this IShape shape) =>
        shape.Metadata.Wrappers.Add("Settings_Wrapper__Reload");
}
