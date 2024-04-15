using OrchardCore.ContentFields.Settings;

namespace OrchardCore.ContentManagement.Metadata.Builders;

public static class ContentPartFieldDefinitionBuilderExtensions
{
    /// <summary>
    /// Applies <see cref="ContentPickerFieldSettings"/> to the field. Constrains the displayed types to <paramref
    /// name="contentTypes"/> and only allows single selection.
    /// </summary>
    public static ContentPartFieldDefinitionBuilder ConfigureContentPicker(
        this ContentPartFieldDefinitionBuilder builder,
        params string[] contentTypes) =>
        builder.ConfigureContentPicker(multiple: false, contentTypes);

    /// <summary>
    /// Applies <see cref="ContentPickerFieldSettings"/> to the field. Constrains the displayed types to <paramref
    /// name="contentTypes"/> and defines if <paramref name="multiple"/> selection is permitted.
    /// </summary>
    public static ContentPartFieldDefinitionBuilder ConfigureContentPicker(
        this ContentPartFieldDefinitionBuilder builder,
        bool multiple,
        params string[] contentTypes) =>
        builder.WithSettings(new ContentPickerFieldSettings
        {
            Multiple = multiple,
            DisplayAllContentTypes = false,
            DisplayedContentTypes = contentTypes,,
        });
}