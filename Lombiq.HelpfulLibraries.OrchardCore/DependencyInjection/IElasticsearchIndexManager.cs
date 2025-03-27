using Nest;
using OrchardCore.Search.Elasticsearch;
using OrchardCore.Search.Elasticsearch.Core.Services;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.DependencyInjection;

/// <summary>
/// Interface for a minimal wrapper around <see cref="ElasticIndexManager"/> so it can be replaced or decorated.
/// </summary>
[SuppressMessage(
    "Minor Code Smell",
    "S4261:Methods should be named according to their synchronicities",
    Justification = "Compatibility with wrapped class.")]
[SuppressMessage("Design", "MA0016:Prefer using collection abstraction instead of implementation", Justification = "Same.")]
public interface IElasticsearchIndexManager
{
    /// <summary>
    /// Deletes the provided index.
    /// </summary>
    Task<bool> DeleteIndex(string indexName);

    /// <summary>
    /// Returns results from a search made with a NEST QueryContainer query.
    /// </summary>
    Task<ElasticTopDocs> SearchAsync(string indexName, QueryContainer query, List<ISort> sort, int from, int size);

    /// <summary>
    /// Verify if an index exists for the current tenant.
    /// </summary>
    Task<bool> ExistsAsync(string indexName);
}
