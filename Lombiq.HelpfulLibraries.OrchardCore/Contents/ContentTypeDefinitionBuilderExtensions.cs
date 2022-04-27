using OrchardCore.Autoroute.Models;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Title.Models;

namespace OrchardCore.ContentManagement.Metadata.Builders;

public static class ContentTypeDefinitionBuilderExtensions
{
    /// <summary>
    /// Merges all <see langword="bool"/> content type settings; the same ones you have checkboxes for in the admin
    /// dashboard's content type editor in the same order. All values have a default <see langword="null"/> value, which
    /// means they don't get altered. If you use this in itself they are left as the default <see langword="false"/>. So
    /// you can set any combination using parameter names in a way that signals intentionality (if you explicitly  want
    /// to deny an ability or you just don't care). Useful clarification for later updates in the migration.
    /// </summary>
    public static ContentTypeDefinitionBuilder SetAbilities(
        this ContentTypeDefinitionBuilder builder,
        bool? creatable = null,
        bool? listable = null,
        bool? draftable = null,
        bool? versionable = null,
        bool? securable = null) =>
        builder.MergeSettings<ContentTypeSettings>(x =>
        {
            x.Creatable = creatable ?? x.Creatable;
            x.Listable = listable ?? x.Listable;
            x.Draftable = draftable ?? x.Draftable;
            x.Versionable = versionable ?? x.Versionable;
            x.Securable = securable ?? x.Securable;
        });

    /// <summary>
    /// Resets all content type settings (creatable, listable, etc.) to <see langword="false"/>. Since all of them are
    /// false by default anyway, the purpose of this method is to signal intention to the reader.
    /// </summary>
    public static ContentTypeDefinitionBuilder NoAbilities(this ContentTypeDefinitionBuilder builder) =>
        builder.SetAbilities();

    /// <summary>
    /// Adds <see cref="TitlePart"/> to the content type.
    /// </summary>
    public static ContentTypeDefinitionBuilder WithTitlePart(
        this ContentTypeDefinitionBuilder builder,
        bool required = true) =>
        builder
            .WithPart(nameof(TitlePart), part =>
            {
                if (required)
                {
                    part.WithSettings(new TitlePartSettings { Options = TitlePartOptions.EditableRequired });
                }
            });

    /// <summary>
    /// Adds <see cref="AutoroutePart"/> to the content type and sets the <see cref="AutoroutePartSettings.Pattern"/>
    /// accordingly.
    /// </summary>
    public static ContentTypeDefinitionBuilder WithContentTypeAutoroute(
        this ContentTypeDefinitionBuilder builder,
        string contentType) =>
        builder
            .WithPart(nameof(AutoroutePart), part => part
                .WithSettings(new AutoroutePartSettings
                {
                    Pattern = $"{contentType}s/" + "{{ ContentItem.ContentItemId | slugify }}",
                    AllowUpdatePath = true,
                    ManageContainedItemRoutes = true,
                }));
}
