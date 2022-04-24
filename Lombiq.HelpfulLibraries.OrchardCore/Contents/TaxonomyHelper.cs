using OrchardCore.ContentManagement;
using OrchardCore.Taxonomies.Models;
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
        await _contentHandleManager.GetContentItemIdAsync($"alias:{alias}") is { } contentItemId &&
        await _contentManager.GetAsync(contentItemId) is { } contentItem
            ? contentItem.As<TaxonomyPart>()?.Terms?.FirstOrDefault(term => term.ContentItemId == termId)
            : null;
}
