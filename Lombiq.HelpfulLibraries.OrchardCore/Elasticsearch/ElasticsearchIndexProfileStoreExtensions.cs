using OrchardCore.Elasticsearch;
using OrchardCore.Indexing.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrchardCore.Indexing;

public static class ElasticsearchIndexProfileStoreExtensions
{
    /// <summary>
    /// Returns only the Elasticsearch indexes from the <paramref name="store"/>.
    /// </summary>
    public static async Task<IEnumerable<IndexProfile>> GetAllElasticsearchIndexesAsync(this IIndexProfileStore store) =>
        (await store.GetAllAsync())
        .Where(profile => profile.ProviderName == ElasticsearchConstants.ProviderName);
}
