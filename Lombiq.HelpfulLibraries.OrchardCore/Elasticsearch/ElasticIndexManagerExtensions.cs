using Elastic.Clients.Elasticsearch;
using OrchardCore.Elasticsearch.Core.Services;
using System;
using System.Threading.Tasks;

namespace OrchardCore.Search.Elasticsearch.Core.Services;

public static class ElasticIndexManagerExtensions
{
    /// <summary>
    /// Clear all indexes for the tenant (within the prefix, if there is one) by passing a wildcard
    /// character (<c>*</c>) as the index name.
    /// </summary>
    [Obsolete($"Use the equivalent extension method for {nameof(ElasticsearchClient)} instead.")]
    public static Task<bool> DeleteAllIndexesAsync(this ElasticsearchIndexManager manager) =>
        throw new NotSupportedException($"Use the equivalent extension method for {nameof(ElasticsearchClient)} instead.");
}
