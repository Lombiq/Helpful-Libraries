using Microsoft.Extensions.DependencyInjection;
using Nest;
using OrchardCore.Search.Elasticsearch;
using OrchardCore.Search.Elasticsearch.Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.DependencyInjection;

public class DefaultElasticsearchIndexManager : IElasticsearchIndexManager
{
    private readonly ElasticIndexManager _manager;

    public DefaultElasticsearchIndexManager(ElasticIndexManager manager) => _manager = manager;

    public Task<bool> DeleteIndex(string indexName) => _manager.DeleteIndex(indexName);

    public Task<ElasticTopDocs> SearchAsync(string indexName, QueryContainer query, List<ISort> sort, int from, int size) =>
        _manager.SearchAsync(indexName, query, sort, from, size);

    public Task<bool> ExistsAsync(string indexName) => _manager.ExistsAsync(indexName);

    public static void Add(IServiceCollection services)
    {
        services.AddSingleton<ElasticIndexManager>();
        services.AddScoped<IElasticsearchIndexManager, DefaultElasticsearchIndexManager>();
    }
}
