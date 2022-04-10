using OrchardCore.Taxonomies.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrchardCore.ContentManagement;

public static class ContentManagerExtensions
{
    public static async Task<T> GetAsync<T>(this IContentManager contentManager, string id)
        where T : ContentPart
        => (await contentManager.GetAsync(id))?.As<T>();

    public static async Task<T> GetAsync<T>(this IContentManager contentManager, string id, VersionOptions versionOptions)
        where T : ContentPart
        => (await contentManager.GetAsync(id, versionOptions))?.As<T>();

    public static Task CreateOrUpdateAsync(this IContentManager contentManager, ContentItem contentItem) =>
        contentItem.Id == 0
            ? contentManager.CreateAsync(contentItem, VersionOptions.Published)
            : contentManager.UpdateAsync(contentItem);

    public static Task<ContentItem> NewOrLoadAsync(
        this IContentManager contentManager,
        ContentItem contentItem,
        string name) =>
        contentItem == null ? contentManager.NewAsync(name) : contentManager.LoadAsync(contentItem);

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
