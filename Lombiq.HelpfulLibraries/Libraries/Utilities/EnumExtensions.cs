using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace System
{
    public static class EnumExtensions
    {
        public static InvalidOperationException UnknownEnumException<T>(this T other)
            where T : Enum =>
            new($"Unknown {other.GetType().Name}: '{other}'");

        public static string GetDisplayNameAttribute(this Enum enumValue) =>
            enumValue == null
                ? string.Empty
                : enumValue.GetType()
                    .GetMember(enumValue.ToString())
                    .First()
                    .GetCustomAttribute<DisplayAttribute>()?
                    .Name;
    }
}
