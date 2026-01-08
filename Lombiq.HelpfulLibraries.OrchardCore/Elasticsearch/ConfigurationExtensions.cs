using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using OrchardCore.Search.Elasticsearch.Core.Models;
using OrchardCore.Search.Elasticsearch.Core.Services;
using System;

namespace OrchardCore.Environment.Shell.Configuration;

public static class ConfigurationExtensions
{
    [Obsolete($"Use {nameof(CreateElasticsearchClient)} instead.")]
    public static ElasticsearchClient CreateElasticClient(this IShellConfiguration shellConfiguration) =>
        shellConfiguration.CreateElasticsearchClient();

    public static ElasticsearchClient CreateElasticsearchClient(
        this IShellConfiguration shellConfiguration,
        IElasticsearchClientFactory? factory = null)
    {
        factory ??= new ElasticsearchClientFactory(NullLogger<ElasticsearchClientFactory>.Instance);

        var configuration = shellConfiguration.GetSection(ElasticsearchConnectionOptionsConfigurations.ConfigSectionName);
        var connectionOptions = configuration.Get<ElasticsearchConnectionOptions>();

        return factory.Create(connectionOptions);
    }
}
