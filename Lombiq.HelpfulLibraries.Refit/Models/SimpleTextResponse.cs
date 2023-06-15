using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Lombiq.HelpfulLibraries.Refit.Models;

/// <summary>
/// An alternative container to <see cref="ApiResponse{T}"/> of <see cref="string"/> which is not <see
/// cref="IDisposable"/>. It stores the text content, and some important metadata. Pass the <see cref="ApiResponse{T}"/>
/// result of an API endpoint directly into the <see cref="ConvertAndDisposeApiResponse"/> which creates an instance and
/// safely disposes the original response so you don't have to worry about memory leaks from storing or passing around
/// an <see cref="IDisposable"/>.
/// </summary>
public class SimpleTextResponse
{
    /// <summary>
    /// Gets the string content of the API response.
    /// </summary>
    public string Content { get; }

    /// <summary>
    /// Gets a read-only dictionary of all headers and their first values.
    /// </summary>
    public IReadOnlyDictionary<string, string> Headers { get; }

    /// <summary>
    /// Gets a value indicating whether the response had no error and its status was <see cref="HttpStatusCode.OK"/>.
    /// </summary>
    public bool IsOk { get; }

    /// <summary>
    /// Gets the error captured by the original <see cref="ApiResponse{T}"/> or <see langword="null"/>.
    /// </summary>
    public ApiException Error { get; }

    /// <summary>
    /// Gets the location header in <see cref="Headers"/>.
    /// </summary>
    public string Location => Headers.TryGetValue(nameof(Location), out var value) ? value : null;

    internal SimpleTextResponse(IApiResponse<string> response)
    {
        Content = response.Content;
        Headers = response.Headers.ToDictionary(header => header.Key, header => header.Value.First());
        IsOk = response.Error == null && response.StatusCode == HttpStatusCode.OK;
        Error = response.Error;
    }

    /// <summary>
    /// Creates a new instance of <see cref="SimpleTextResponse"/> from <paramref name="response"/> and then disposes
    /// the input.
    /// </summary>
    public static SimpleTextResponse ConvertAndDisposeApiResponse(ApiResponse<string> response)
    {
        if (response == null) return null;

        using (response)
        {
            return new SimpleTextResponse(response);
        }
    }
}
