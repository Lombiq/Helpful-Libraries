using OrchardCore.Taxonomies.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrchardCore.ContentManagement;

public static class ContentManagerExtensions
{
    /// <summary>
    /// Gets the published content item with the specified <paramref name="id"/> and returns it as the provided
    /// <typeparamref name="T"/> <see cref="ContentPart"/>.
    /// </summary>
    public static async Task<T> GetAsync<T>(this IContentManager contentManager, string id)
        where T : ContentPart
        => (await contentManager.GetAsync(id))?.As<T>();

    /// <summary>
    /// Gets the published content item with the specified <paramref name="id"/> and version, and returns it as the
    /// provided <typeparamref name="T"/> <see cref="ContentPart"/>.
    /// </summary>
    /// <param name="id">The ID of the content item to retrieve.</param>
    /// <param name="versionOptions">The version data of the content item to retrieve.</param>
    public static async Task<T> GetAsync<T>(this IContentManager contentManager, string id, VersionOptions versionOptions)
        where T : ContentPart
        => (await contentManager.GetAsync(id, versionOptions))?.As<T>();

    /// <summary>
    /// Persists the given <paramref name="contentItem"/> with a new version if it does not yet exist, or updates it
    /// without a new version if it already exists.
    /// </summary>
    public static Task CreateOrUpdateAsync(this IContentManager contentManager, ContentItem contentItem) =>
        contentItem.Id == 0
            ? contentManager.CreateAsync(contentItem, VersionOptions.Published)
            : contentManager.UpdateAsync(contentItem);

    /// <summary>
    /// Creates a new content item with the specified <paramref name="name"/> as the content type if the provided
    /// <paramref name="contentItem"/> is <see langword="null"/>, otherwise loads the provided content item.
    /// </summary>
    /// <param name="contentItem">The content item to load.</param>
    /// <param name="name">The type of the newly created content item.</param>
    /// <returns>The newly created or loaded content item.</returns>
    public static Task<ContentItem> NewOrLoadAsync(
        this IContentManager contentManager,
        ContentItem contentItem,
        string name) =>
        contentItem == null ? contentManager.NewAsync(name) : contentManager.LoadAsync(contentItem);

    /// <summary>
    /// Retrieves the terms of a taxonomy content item with the alias provided in <paramref name="taxonomyAlias"/>.
    /// </summary>
    /// <returns>A read-only list containing the terms of the taxonomy content item.</returns>
    public static async Task<IReadOnlyList<ContentItem>> GetTaxonomyTermsAsync(
        this IContentManager contentManager,
        IContentHandleManager contentHandleManager,
        string taxonomyAlias)
    {
        var taxonomyContentItemId = await contentHandleManager.GetContentItemIdAsync($"alias:{taxonomyAlias}");
        var taxonomy = string.IsNullOrEmpty(taxonomyContentItemId)
            ? null
            : await contentManager.GetAsync(taxonomyContentItemId);

        return taxonomy?.As<TaxonomyPart>()?.Terms;
    }

    /// <summary>
    /// Retrieves the display texts of the terms belonging to a taxonomy content item that is identified by the
    /// provided <paramref name="taxonomyId"/>.
    /// </summary>
    /// <returns>A dictionary where the keys contain each term's ID and the values contain each term's display text.</returns>
    public static async Task<IDictionary<string, string>> GetTaxonomyTermsDisplayTextsAsync(
        this IContentManager contentManager,
        string taxonomyId) =>
        (await contentManager.GetAsync(taxonomyId))
        .As<TaxonomyPart>()
        .Terms
        .ToDictionary(term => term.ContentItemId, term => term.DisplayText);

    /// <summary>
    /// Returns the <see cref="ContentItem.DisplayText"/> of a specific term identified by its <paramref name="termId"/>
    /// within a taxonomy identified by its <paramref name="alias"/>. If none are found <see langword="null"/> is
    /// returned.
    /// </summary>
    public static async Task<string> GetTaxonomyTermDisplayTextAsync(
        this IContentManager contentManager,
        IContentHandleManager contentHandleManager,
        string alias,
        string termId) =>
        (await contentManager.GetTaxonomyTermsAsync(contentHandleManager, alias))
        .FirstOrDefault(term => term.ContentItemId == termId)?
        .DisplayText;

    /// <summary>
    /// Returns a <see cref="ContentItem"/> of the given type identified by the
    /// <see cref="ContentItem.ContentItemId"/>. If the <see cref="VersionOptions"/> is not provided it'll get the
    /// published version.
    /// </summary>
    /// <param name="contentType">Content type of the desired <see cref="ContentItem"/>.</param>
    /// <param name="contentItemId">ID of the <see cref="ContentItem"/>.</param>
    /// <param name="versionOptions">Version of the <see cref="ContentItem"/> (e.g., Published, Latest).</param>
    /// <returns>Acquired or newly created <see cref="ContentItem"/>.</returns>
    public static async Task<ContentItem> GetOfTypeAsync(
        this IContentManager contentManager,
        string contentItemId,
        string contentType,
        VersionOptions versionOptions = null)
    {
        var contentItem = await contentManager.GetAsync(contentItemId, versionOptions ?? VersionOptions.Published);

        return contentItem.ContentType == contentType ? contentItem : null;
    }

    /// <summary>
    /// Returns a <see cref="ContentItem"/> of the given type identified by the
    /// <see cref="ContentItem.ContentItemId"/>. If the ID is not given then it'll create a new one.
    /// </summary>
    /// <param name="contentType">Content type of the desired <see cref="ContentItem"/>.</param>
    /// <param name="contentItemId">ID of the <see cref="ContentItem"/>.</param>
    /// <param name="versionOptions">Version of the <see cref="ContentItem"/> (e.g., Published, Latest).</param>
    /// <returns>Acquired or newly created <see cref="ContentItem"/>.</returns>
    public static Task<ContentItem> GetOrCreateAsync(
        this IContentManager contentManager,
        string contentItemId,
        string contentType,
        VersionOptions versionOptions = null)
    {
        if (string.IsNullOrEmpty(contentType))
        {
            return contentManager.GetOfTypeAsync(contentItemId, contentType, versionOptions);
        }

        return string.IsNullOrEmpty(contentItemId)
            ? contentManager.NewAsync(contentType)
            : contentManager.GetOfTypeAsync(contentItemId, contentType, versionOptions);
    }
}
