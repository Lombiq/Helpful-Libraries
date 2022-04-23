using OrchardCore.ContentManagement;
using System.Linq;

namespace System.Collections.Generic;

public static class ContentsEnumerableExtensions
{
    /// <summary>
    /// Re-flattens <see cref="ILookup{TKey, ContentItem}"/> or <c>GroupBy</c> collections and eliminates duplicates
    /// using <see cref="ContentItem.ContentItemVersionId"/>.
    /// </summary>
    public static IEnumerable<ContentItem> GetUniqueValues<TKey>(
        this IEnumerable<IGrouping<TKey, ContentItem>> lookup) =>
        lookup
            .SelectMany(grouping => grouping)
            .Unique(contentItem => contentItem.ContentItemVersionId);

    /// <summary>
    /// Re-flattens <see cref="ILookup{TKey, ContentItem}"/> or <c>GroupBy</c> collections and ensures that each
    /// grouping only had one item (i.e. one-to-one relationships).
    /// </summary>
    public static IEnumerable<ContentItem> GetSingleValues<TKey>(
        this IEnumerable<IGrouping<TKey, ContentItem>> lookup) =>
        lookup.Select(item => item.Single());
}
