using OrchardCore.Search.Elasticsearch.Core.Models;
using OrchardCore.Search.Elasticsearch.Core.Services;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.DependencyInjection;

public class DefaultElasticsearchIndexingService : IElasticsearchIndexingService
{
    private readonly ElasticIndexingService _service;

    public DefaultElasticsearchIndexingService(ElasticIndexingService service) =>
        _service = service;

    public Task RebuildIndexAsync(ElasticIndexSettings elasticIndexSettings) =>
        _service.RebuildIndexAsync(elasticIndexSettings);

    public Task ProcessContentItemsAsync(params string[] indexNames) =>
        _service.ProcessContentItemsAsync(indexNames);

    public Task CreateIndexAsync(ElasticIndexSettings elasticIndexSettings) =>
        _service.CreateIndexAsync(elasticIndexSettings);
}
