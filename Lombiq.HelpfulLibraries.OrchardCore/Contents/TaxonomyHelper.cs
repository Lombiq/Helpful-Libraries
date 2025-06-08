using OrchardCore.ContentManagement;
using OrchardCore.Taxonomies.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Contents;

public class TaxonomyHelper : ITaxonomyHelper
{
    private readonly IContentHandleManager _contentHandleManager;
    private readonly IContentManager _contentManager;

    public TaxonomyHelper(IContentHandleManager contentHandleManager, IContentManager contentManager)
    {
        _contentHandleManager = contentHandleManager;
        _contentManager = contentManager;
    }

    public async Task<ContentItem> GetTermContentItemByTaxonomyAliasAsync(string alias, string termId) =>
        (await GetTermsOfTaxonomyByAliasAsync(alias, [termId])).FirstOrDefault();

    public async Task<IEnumerable<ContentItem>> GetTermsOfTaxonomyByIdAsync(string taxonomyId, IEnumerable<string> termIds)
    {
        if (string.IsNullOrWhiteSpace(taxonomyId) ||
            await _contentManager.GetAsync(taxonomyId) is not { } contentItem)
        {
            return [];
        }

        var ids = termIds?.AsList() ?? [];
        return ids.Count == 0
            ? GetAllChildren(contentItem)
            : GetSelected(contentItem, ids);
    }

    public async Task<IEnumerable<ContentItem>> GetTermsOfTaxonomyByAliasAsync(string taxonomyAlias, IEnumerable<string> termIds) =>
        await GetTermsOfTaxonomyByIdAsync(
            await _contentHandleManager.GetContentItemIdAsync($"alias:{taxonomyAlias}"),
            termIds);

    /// <summary>
    /// Returns all child content items in a taxonomy tree.
    /// </summary>
    /// <param name="contentItem">The root of the tree to enumerate.</param>
    /// <param name="includeSelf">
    /// If <see langword="true"/> the <paramref name="contentItem"/> will be the first result so the collection is never
    /// empty as long as  <paramref name="contentItem"/> isn't <see langword="null"/>.
    /// </param>
    /// <returns>An unsorted list of all child items.</returns>
    public static IList<ContentItem> GetAllChildren(ContentItem contentItem, bool includeSelf = false)
    {
        var results = new List<ContentItem>();
        if (contentItem == null) return results;
        if (includeSelf) results.Add(contentItem);

        var partTerms = contentItem.As<TaxonomyPart>()?.Terms ?? Enumerable.Empty<ContentItem>();
        var itemTerms = contentItem.GetProperty<List<ContentItem>>(nameof(TaxonomyPart.Terms)) ?? Enumerable.Empty<ContentItem>();
        foreach (var child in partTerms.Concat(itemTerms))
        {
            results.AddRange(GetAllChildren(child, includeSelf: true));
        }

        return results;
    }

    /// <summary>
    /// Returns the child content items in a taxonomy tree with the IDs listed in <paramref name="selectedIds"/>.
    /// </summary>
    public static IEnumerable<ContentItem> GetSelected(ContentItem contentItem, ICollection<string> selectedIds) =>
        GetAllChildren(contentItem)
            .Where(term => selectedIds.Contains(term.ContentItemId));
}
