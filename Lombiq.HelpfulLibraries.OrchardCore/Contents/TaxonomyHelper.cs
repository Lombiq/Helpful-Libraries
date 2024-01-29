using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using OrchardCore.Taxonomies.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Contents;

public class TaxonomyHelper(IContentHandleManager contentHandleManager, IContentManager contentManager) : ITaxonomyHelper
{
    public async Task<ContentItem> GetTermContentItemByTaxonomyAliasAsync(string alias, string termId) =>
        await contentHandleManager.GetContentItemIdAsync($"alias:{alias}") is { } contentItemId &&
        await contentManager.GetAsync(contentItemId) is { } contentItem
            ? contentItem.As<TaxonomyPart>()?.Terms?.Find(term => term.ContentItemId == termId)
            : null;

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
        var itemTerms = (contentItem.Content.Terms as JArray)?.ToObject<List<ContentItem>>() ?? Enumerable.Empty<ContentItem>();
        foreach (var child in partTerms.Concat(itemTerms))
        {
            results.AddRange(GetAllChildren(child, includeSelf: true));
        }

        return results;
    }
}
