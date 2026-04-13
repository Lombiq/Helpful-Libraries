using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using OrchardCore.Elasticsearch.Core.Models;
using OrchardCore.Elasticsearch.Core.Services;
using System;

namespace OrchardCore.Environment.Shell.Configuration;

public static class ConfigurationExtensions
{
    [Obsolete($"Use {nameof(CreateElasticsearchClient)} instead.")]
    public static ElasticsearchClient CreateElasticClient(this IShellConfiguration shellConfiguration) =>
        shellConfiguration.CreateElasticsearchClient();

    /// <summary>
    /// Returns a new instance of the client.
    /// </summary>
    /// <remarks><para>
    /// Same as the code found in <see cref="OrchardCore.Elasticsearch.Startup.ConfigureServices"/>.
    /// </para></remarks>
    public static ElasticsearchClient CreateElasticsearchClient(
        this IShellConfiguration shellConfiguration,
        IElasticsearchClientFactory? factory = null)
    {
        factory ??= new ElasticsearchClientFactory(NullLogger<ElasticsearchClientFactory>.Instance);

        var configuration = shellConfiguration.GetSection(ElasticsearchConnectionOptionsConfigurations.ConfigSectionName);
        var connectionOptions = configuration.Get<ElasticsearchConnectionOptions>() ?? new ElasticsearchConnectionOptions();

        return factory.Create(connectionOptions);
    }
}
