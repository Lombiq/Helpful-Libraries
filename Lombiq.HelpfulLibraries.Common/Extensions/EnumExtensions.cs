using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace System;

public static class EnumExtensions
{
    /// <summary>
    /// Raises a standardized exception on the default arm of a <see langword="switch"/>.
    /// </summary>
    public static InvalidOperationException UnknownEnumException<T>(this T other)
        where T : Enum =>
        new($"Unknown {other.GetType().Name}: '{other}'");

    /// <summary>
    /// Attempts to retrieve an <see cref="Enum"/> object's <see cref="DisplayAttribute.Name"/> value.
    /// </summary>
    /// <returns>The display attribute's name if found, otherwise an empty string.</returns>
    public static string GetDisplayNameAttribute(this Enum enumValue) =>
        enumValue == null
            ? string.Empty
            : enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()?
                .Name;
}
