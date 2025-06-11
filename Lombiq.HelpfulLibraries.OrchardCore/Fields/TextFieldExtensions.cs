using System;

namespace OrchardCore.ContentFields.Fields;

public static class TextFieldExtensions
{
    /// <summary>
    /// Tries to convert the <see cref="TextField.Text"/> of the provided <paramref name="field"/> into the
    /// <typeparamref name="TEnum"/> <see langword="enum"/> type. If it can't be parsed, <paramref name="defaultValue"/>
    /// is returned instead.
    /// </summary>
    public static TEnum ParseEnum<TEnum>(this TextField field, TEnum defaultValue = default)
        where TEnum : struct =>
        string.IsNullOrWhiteSpace(field.Text) || !Enum.TryParse<TEnum>(field.Text, out var value)
            ? defaultValue
            : value;
}
