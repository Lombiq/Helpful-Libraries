using OrchardCore.ContentManagement;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Contents;

/// <summary>
/// Taxonomy-related helper methods.
/// </summary>
public interface ITaxonomyHelper
{
    /// <summary>
    /// Returns <see cref="ContentItem"/> by the given <paramref name="alias"/> and <paramref name="termId"/>.
    /// </summary>
    Task<ContentItem> GetTermContentItemByTaxonomyAliasAsync(string alias, string termId);

    /// <summary>
    /// Gets the taxonomy content item with the ID indicated by <paramref name="taxonomyId"/> and recursively searches
    /// for each term <see cref="ContentItem"/> with an ID in <paramref name="termIds"/>.
    /// </summary>
    /// <param name="termIds">The selected term IDs. If it's <see langword="null"/> or empty, all terms are returned instead.</param>
    Task<IEnumerable<ContentItem>> GetTermsOfTaxonomyByIdAsync(string taxonomyId, IEnumerable<string> termIds);

    /// <summary>
    /// Gets the taxonomy content item with the alias indicated by <paramref name="taxonomyAlias"/> and recursively
    /// searches for each term <see cref="ContentItem"/> with an ID in <paramref name="termIds"/>.
    /// </summary>
    /// <param name="termIds">The selected term IDs. If it's <see langword="null"/> or empty, all terms are returned instead.</param>
    Task<IEnumerable<ContentItem>> GetTermsOfTaxonomyByAliasAsync(string taxonomyAlias, IEnumerable<string> termIds);
}
