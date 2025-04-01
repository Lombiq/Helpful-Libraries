using OrchardCore.Search.Elasticsearch.Core.Models;
using OrchardCore.Search.Elasticsearch.Core.Services;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.DependencyInjection;

/// <summary>
/// Interface for a minimal wrapper around <see cref="ElasticIndexingService"/> so it can be replaced or decorated.
/// </summary>
public interface IElasticsearchIndexingService
{
    /// <summary>
    /// Deletes and recreates the full index content.
    /// </summary>
    Task RebuildIndexAsync(ElasticIndexSettings elasticIndexSettings);

    /// <summary>
    /// Processes the content items associated with the provided <paramref name="indexNames"/>.
    /// </summary>
    Task ProcessContentItemsAsync(params string[] indexNames);

    /// <summary>
    /// Creates a new index.
    /// </summary>
    Task CreateIndexAsync(ElasticIndexSettings elasticIndexSettings);
}
