using Microsoft.Extensions.Configuration;
using Nest;
using OrchardCore.Environment.Shell.Configuration;
using OrchardCore.Search.Elasticsearch.Core.Models;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigurationExtensions
{
    public static IElasticClient CreateElasticClient(this IShellConfiguration shellConfiguration)
    {
        var configuration = shellConfiguration.GetSection("OrchardCore_Elasticsearch");
        var elasticConfiguration = configuration.Get<ElasticConnectionOptions>();

        return new ElasticClient(elasticConfiguration?.GetConnectionSettings());
    }
}
