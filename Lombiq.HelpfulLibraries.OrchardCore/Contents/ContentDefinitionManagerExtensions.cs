using OrchardCore.ContentManagement.Metadata.Builders;
using System;
using System.Linq;

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
    public static T GetContentPartSettings<T>(
        this IContentDefinitionManager contentDefinitionManager,
        string contentType,
        string contentPartName)
        where T : class, new()
    {
        var contentTypeDefinition = contentDefinitionManager.GetTypeDefinition(contentType);
        var contentTypePartDefinition = contentTypeDefinition.Parts
            .FirstOrDefault(part => part.PartDefinition.Name == contentPartName);

        return contentTypePartDefinition?.GetSettings<T>();
    }

    /// <summary>
    /// Alters the definition of a content part whose technical name is its model's type name. It uses the typed wrapper
    /// <see cref="ContentPartDefinitionBuilder{TPart}"/> for configuration.
    /// </summary>
    public static string AlterPartDefinition<TPart>(
        this IContentDefinitionManager manager,
        Action<ContentPartDefinitionBuilder<TPart>> configure)
        where TPart : ContentPart
    {
        var name = typeof(TPart).Name;
        manager.AlterPartDefinition(name, part => configure(part.AsPart<TPart>()));
        return name;
    }

    public static void AlterTypeDefinitionForTaxonomy(this IContentDefinitionManager manager, string contentType) =>
        manager.AlterTypeDefinition(contentType, type => type.NoAbilities().WithTitlePart());
}
