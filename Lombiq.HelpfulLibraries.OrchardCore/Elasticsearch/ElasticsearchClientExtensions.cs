using Elastic.Clients.Elasticsearch.Core;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Microsoft.Extensions.Configuration;
using OrchardCore.Elasticsearch;
using OrchardCore.Elasticsearch.Core.Services;
using OrchardCore.Environment.Shell.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Elastic.Clients.Elasticsearch;

public static class ElasticIndexManagerExtensions
{
    /// <summary>
    /// Clear all indexes for the tenant (within the prefix, if there is one) by passing a wildcard character (<c>*</c>)
    /// as the index name.
    /// </summary>
    public static async Task DeleteAllIndexesAsync(this ElasticsearchClient client, string? prefix)
    {
        var index = string.IsNullOrWhiteSpace(prefix) ? Indices.All : Indices.Index($"{prefix}_*");
        var getRequest = new GetIndexRequest(index) { ExpandWildcards = [ExpandWildcard.All], AllowNoIndices = true };
        var getResponse = (await client.Indices.GetAsync(getRequest)).ThrowIfFailed($"get index \"{index}\"");

        if (getResponse.Indices.Count == 0) return;
        (await client.Indices.DeleteAsync(getResponse.Indices.Keys.ToArray())).ThrowIfFailed($"delete index \"{index}\"");
    }

    public static Task DeleteAllIndexesAsync(this ElasticsearchClient client, IShellConfiguration shellConfiguration)
    {
        var prefix = shellConfiguration
            .GetSection(ElasticsearchConnectionOptionsConfigurations.ConfigSectionName)
            .GetValue<string>(nameof(ElasticsearchOptions.IndexPrefix));
        return client.DeleteAllIndexesAsync(prefix);
    }
}
