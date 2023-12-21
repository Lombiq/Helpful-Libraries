using OrchardCore.ContentManagement.Metadata.Builders;
using System;

namespace OrchardCore.ContentManagement.Metadata.Settings;

public static class ContentTypePartDefinitionBuilderExtensions
{
    /// <summary>
    /// Sets the part's editor using an <see cref="Enum"/> parameter.
    /// </summary>
    public static ContentTypePartDefinitionBuilder WithEditor(this ContentTypePartDefinitionBuilder builder, Enum editor) =>
        builder.MergeSettings<ContentTypePartSettings>(x => x.Editor = editor.ToString());
}
