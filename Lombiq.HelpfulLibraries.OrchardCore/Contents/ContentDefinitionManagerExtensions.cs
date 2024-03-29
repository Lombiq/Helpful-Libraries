using OrchardCore.ContentManagement.Metadata.Builders;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OrchardCore.ContentManagement.Metadata;

public static class ContentDefinitionManagerExtensions
{
    /// <summary>
    /// Returns the content part settings object defined for the given content part on the given content type.
    /// </summary>
    /// <typeparam name="T">Type of the content part settings.</typeparam>
    /// <param name="contentType">Technical name of the content type.</param>
    /// <param name="contentPartName">Technical name of the content part.</param>
    /// <returns>Content part settings object.</returns>
    public static async Task<T> GetContentPartSettingsAsync<T>(
        this IContentDefinitionManager contentDefinitionManager,
        string contentType,
        string contentPartName)
        where T : class, new()
    {
        var contentTypeDefinition = await contentDefinitionManager.GetTypeDefinitionAsync(contentType);
        var contentTypePartDefinition = contentTypeDefinition.Parts
            .FirstOrDefault(part => part.PartDefinition.Name == contentPartName);

        return contentTypePartDefinition?.GetSettings<T>();
    }

    /// <summary>
    /// Alters the definition of a content part whose technical name is its model's type name. It uses the typed wrapper
    /// <see cref="ContentPartDefinitionBuilder{TPart}"/> for configuration.
    /// </summary>
    public static async Task<string> AlterPartDefinitionAsync<TPart>(
        this IContentDefinitionManager manager,
        Action<ContentPartDefinitionBuilder<TPart>> configure)
        where TPart : ContentPart
    {
        var name = typeof(TPart).Name;
        await manager.AlterPartDefinitionAsync(name, part => configure(part.AsPart<TPart>()));
        return name;
    }

    /// <summary>
    /// Prepares a <paramref name="contentType"/> to be used as a Taxonomy by resetting all content type settings to
    /// <see langword="false"/> and adding <c>TitlePart</c>.
    /// </summary>
    public static Task AlterTypeDefinitionForTaxonomyAsync(this IContentDefinitionManager manager, string contentType) =>
        manager.AlterTypeDefinitionAsync(contentType, type => type.NoAbilities().WithTitlePart());
}
