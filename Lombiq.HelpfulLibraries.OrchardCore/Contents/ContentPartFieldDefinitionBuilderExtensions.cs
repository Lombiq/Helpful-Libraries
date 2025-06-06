using Lombiq.HelpfulLibraries.OrchardCore.Contents;
using Lombiq.HelpfulLibraries.OrchardCore.Fields;
using OrchardCore.ContentFields.Settings;
using System;
using System.Linq;

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
            DisplayedContentTypes = contentTypes,
        });

    /// <summary>
    /// Configures the field of type <typeparamref name="TField"/> to be required.
    /// </summary>
    public static ContentPartFieldDefinitionBuilder Required<TField>(this ContentPartFieldDefinitionBuilder builder)
        where TField : ContentField
    {
        DefinitionHelper.ConfigureRequired<TField>(builder);
        return builder;
    }

    /// <summary>
    /// Configures the field to use the <see cref="ContentFieldEditorEnums.TextFieldEditors.PredefinedList"/> editor,
    /// sets the default value to <paramref name="defaultValue"/>, and populates its options with the names of the
    /// provided <typeparamref name="TEnum"/> type.
    /// </summary>
    public static ContentPartFieldDefinitionBuilder WithEnumEditor<TEnum>(
        this ContentPartFieldDefinitionBuilder builder,
        TEnum defaultValue = default,
        EditorOption editor = EditorOption.Dropdown)
        where TEnum : struct, Enum =>
        builder
            .WithEditor(ContentFieldEditorEnums.TextFieldEditors.PredefinedList)
            .WithSettings<TextFieldPredefinedListEditorSettings>(new()
            {
                DefaultValue = defaultValue.ToString(),
                Editor = editor,
                Options = Enum.GetValues<TEnum>()
                    .OrderBy(value => (int)(object)value)
                    .Select(value => value.ToString())
                    .Select(name => new ListValueOption(name, name))
                    .ToArray(),
            });
}
