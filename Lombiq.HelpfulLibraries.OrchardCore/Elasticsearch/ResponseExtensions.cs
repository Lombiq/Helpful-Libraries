using Elastic.Transport.Products.Elasticsearch;
using System;
namespace Elastic.Clients.Elasticsearch.Core;

public static class ResponseExtensions
{
    public static T ThrowIfFailed<T>(this T response, string? message = null)
        where T : ElasticsearchResponse
    {
        if (response.IsValidResponse) return response;

        if (!string.IsNullOrWhiteSpace(message)) message = $" ({message.Trim()})";
        var error = $"Elasticsearch operation failed{message}. {response.DebugInformation}";
        throw new InvalidOperationException(error);
    }
}
