using OrchardCore.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace OrchardCore.ContentManagement;

/// <summary>
/// Uses the <see cref="IContentManagerSession"/> scoped cache around any <see cref="ContentItem"/> query done outside
/// <see cref="IContentManager"/>, e.g. if it is not a joined query on <see cref="ContentItemIndex"/>.
/// </summary>
public static class ContentManagerSessionExtensions
{
    /// <summary>
    /// Queries a <see cref="ContentItem"/> and then stores it in the scoped cache.
    /// </summary>
    public static async Task<ContentItem> QueryContentAsync(
        this IContentManagerSession contentManagerSession,
        Func<Task<ContentItem>> queryAsync)
    {
        var contentItem = await queryAsync();
        if (contentItem != null && !contentManagerSession.RecallVersionId(contentItem.Id, out _))
        {
            contentManagerSession.Store(contentItem);
        }

        return contentItem;
    }

    /// <summary>
    /// Queries <see cref="ContentItem"/>s and then stores them in the scoped cache.
    /// </summary>
    public static async Task<IEnumerable<ContentItem>> QueryContentAsync(
        this IContentManagerSession contentManagerSession,
        Func<Task<IEnumerable<ContentItem>>> queryAsync)
    {
        var contentItems = await queryAsync();
        foreach (var contentItem in contentItems)
        {
            if (!contentManagerSession.RecallVersionId(contentItem.Id, out _))
            {
                contentManagerSession.Store(contentItem);
            }
        }

        return contentItems;
    }

    /// <summary>
    /// Gets a published <see cref="ContentItem"/> from the scoped cache or query it.
    /// </summary>
    public static async Task<ContentItem> GetOrQueryContentAsync(
        this IContentManagerSession contentManagerSession,
        string contentItemId,
        Func<string, Task<ContentItem>> queryAsync)
    {
        // If the published version is already stored, we can return it.
        if (contentManagerSession.RecallPublishedItemId(contentItemId, out var contentItem))
        {
            return contentItem;
        }

        contentItem = await queryAsync(contentItemId);
        if (contentItem != null && !contentManagerSession.RecallVersionId(contentItem.Id, out _))
        {
            contentManagerSession.Store(contentItem);
        }

        return contentItem;
    }

    /// <summary>
    /// Gets published <see cref="ContentItem"/>s from the scoped cache or query them.
    /// </summary>
    public static async Task<IEnumerable<ContentItem>> GetOrQueryContentAsync(
        this IContentManagerSession contentManagerSession,
        IEnumerable<string> contentItemIds,
        Func<IEnumerable<string>, Task<IEnumerable<ContentItem>>> queryAsync)
    {
        List<ContentItem> contentItems = null;
        List<ContentItem> storedItems = null;

        foreach (var contentItemId in contentItemIds)
        {
            // If the published version is already stored, we can return it.
            if (contentManagerSession.RecallPublishedItemId(contentItemId, out var contentItem))
            {
                storedItems ??= new List<ContentItem>();
                storedItems.Add(contentItem);
            }
        }

        // Only query the ids not already stored.
        var itemIdsToQuery = storedItems != null
            ? contentItemIds.Except(storedItems.Select(x => x.ContentItemId))
            : contentItemIds;

        if (itemIdsToQuery.Any())
        {
            contentItems = (await queryAsync(itemIdsToQuery)).ToList();
        }

        if (contentItems != null)
        {
            for (var i = 0; i < contentItems.Count; i++)
            {
                if (!contentManagerSession.RecallVersionId(contentItems[i].Id, out _))
                {
                    contentManagerSession.Store(contentItems[i]);
                }
            }

            if (storedItems != null)
            {
                contentItems.AddRange(storedItems);
            }
        }
        else if (storedItems != null)
        {
            contentItems = storedItems;
        }
        else
        {
            return Enumerable.Empty<ContentItem>();
        }

        var contentItemIdsArray = contentItemIds.ToImmutableArray();
        return contentItems.OrderBy(c => contentItemIdsArray.IndexOf(c.ContentItemId));
    }
}
