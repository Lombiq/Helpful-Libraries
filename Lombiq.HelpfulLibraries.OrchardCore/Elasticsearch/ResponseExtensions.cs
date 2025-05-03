using System;

namespace Nest;

public static class ResponseExtensions
{
    public static T ThrowIfFailed<T>(this T response, string message = null)
        where T : ResponseBase
    {
        if (response.IsValid) return response;

        if (!string.IsNullOrWhiteSpace(message)) message = $" ({message.Trim()})";
        var error = $"Elasticsearch operation failed{message}. {response.DebugInformation}";
        throw new InvalidOperationException(error);
    }
}
