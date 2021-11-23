using OrchardCore.ContentManagement;
using OrchardCore.Taxonomies.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.Libraries.Contents
{
    public class TaxonomyHelper : ITaxonomyHelper
    {
        private readonly IContentHandleManager _contentHandleManager;
        private readonly IContentManager _contentManager;

        public TaxonomyHelper(IContentHandleManager contentHandleManager, IContentManager contentManager)
        {
            _contentHandleManager = contentHandleManager;
            _contentManager = contentManager;
        }

        public async Task<ContentItem> GetTermContentItemByTaxonomyAliasAsync(string alias, string termId)
        {
            var contentItemId = await _contentHandleManager.GetContentItemIdAsync($"alias:{alias}");
            var terms = (await _contentManager.GetAsync(contentItemId)).As<TaxonomyPart>().Terms;
            return terms.FirstOrDefault(term => term.ContentItemId == termId);
        }
    }
}
