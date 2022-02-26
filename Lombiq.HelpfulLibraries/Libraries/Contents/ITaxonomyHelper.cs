using OrchardCore.ContentManagement;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.Libraries.Contents
{
    /// <summary>
    /// Taxonomy-related helper functions.
    /// </summary>
    public interface ITaxonomyHelper
    {
        /// <summary>
        /// Returns <see cref="ContentItem"/> by the given <paramref name="alias"/> and <paramref name="termId"/>.
        /// </summary>
        Task<ContentItem> GetTermContentItemByTaxonomyAliasAsync(string alias, string termId);
    }
}
