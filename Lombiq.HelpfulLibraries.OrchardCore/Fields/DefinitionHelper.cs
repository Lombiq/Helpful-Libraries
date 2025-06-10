using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentFields.Settings;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata.Builders;
using OrchardCore.Media.Fields;
using OrchardCore.Media.Settings;
using OrchardCore.Taxonomies.Fields;
using OrchardCore.Taxonomies.Settings;
using System;

namespace Lombiq.HelpfulLibraries.OrchardCore.Fields;

public static class DefinitionHelper
{
    /// <summary>
    /// Configures the field of type <typeparamref name="TField"/> to be required.
    /// </summary>
    /// <remarks><para>Only the Orchard Core's built-in fields are supported.</para></remarks>
    public static void ConfigureRequired<TField>(ContentPartFieldDefinitionBuilder builder)
        where TField : ContentField
    {
        switch (typeof(TField).Name)
        {
            case nameof(ContentPickerField):
                builder.WithSettings<ContentPickerFieldSettings>(new() { Required = true });
                break;
            case nameof(DateField):
                builder.WithSettings<DateFieldSettings>(new() { Required = true });
                break;
            case nameof(DateTimeField):
                builder.WithSettings<DateTimeFieldSettings>(new() { Required = true });
                break;
            case nameof(LinkField):
                builder.WithSettings<LinkFieldSettings>(new() { Required = true });
                break;
            case nameof(LocalizationSetContentPickerField):
                builder.WithSettings<LocalizationSetContentPickerFieldSettings>(new() { Required = true });
                break;
            case nameof(MultiTextField):
                builder.WithSettings<MultiTextFieldSettings>(new() { Required = true });
                break;
            case nameof(NumericField):
                builder.WithSettings<NumericFieldSettings>(new() { Required = true });
                break;
            case nameof(TextField):
                builder.WithSettings<TextFieldSettings>(new() { Required = true });
                break;
            case nameof(TimeField):
                builder.WithSettings<TimeFieldSettings>(new() { Required = true });
                break;
            case nameof(UserPickerField):
                builder.WithSettings<UserPickerFieldSettings>(new() { Required = true });
                break;
            case nameof(YoutubeField):
                builder.WithSettings<YoutubeFieldSettings>(new() { Required = true });
                break;
            case nameof(TaxonomyField):
                builder.WithSettings<TaxonomyFieldSettings>(new() { Required = true });
                break;
            case nameof(MediaField):
                builder.WithSettings<MediaFieldSettings>(new() { Required = true });
                break;
            default:
                throw new InvalidOperationException($"Unsupported type: {typeof(TField).Name}.");
        }
    }
}
