using System.Threading.Tasks;

namespace OrchardCore.Search.Elasticsearch.Core.Services;

public static class ElasticIndexManagerExtensions
{
    /// <summary>
    /// Clear all indexes for the tenant (within the prefix, if there is one) by passing a wildcard
    /// character (<c>*</c>) as the index name.
    /// </summary>
    public static Task<bool> DeleteAllIndexesAsync(this ElasticIndexManager manager) =>
        manager.DeleteIndex("*");
}
