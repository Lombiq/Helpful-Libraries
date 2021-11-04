using OrchardCore.ContentManagement;
using OrchardCore.Taxonomies.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.Libraries.Contents
{
    public class TaxonomyHelper : ITaxonomyHelper
    {
        private readonly IContentAliasManager _contentAliasManager;
        private readonly IContentManager _contentManager;

        public TaxonomyHelper(IContentAliasManager contentAliasManager, IContentManager contentManager)
        {
            _contentAliasManager = contentAliasManager;
            _contentManager = contentManager;
        }

        public async Task<ContentItem> GetTermContentItemAsync(string alias, string termId)
        {
            var contentItemId = await _contentAliasManager.GetContentItemIdAsync($"alias:{alias}");
            var terms = (await _contentManager.GetAsync(contentItemId)).As<TaxonomyPart>().Terms;
            return terms.FirstOrDefault(term => term.ContentItemId == termId);
        }
    }
}
