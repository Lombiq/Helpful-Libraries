using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Http;

public static class HttpResponseExtensions
{
    /// <summary>
    /// Sets the response status code to <see cref="StatusCodes.Status404NotFound"/>.
    /// </summary>
    public static void NotFound(this HttpResponse response) =>
        response.StatusCode = StatusCodes.Status404NotFound;

    /// <summary>
    /// Sets the response status code to <see cref="StatusCodes.Status404NotFound"/> and completes the response. This
    /// can be used directly to return in middlewares.
    /// </summary>
    public static async Task NotFoundAsync(this HttpContext context)
    {
        context.Response.NotFound();
        context.Response.Headers.Append(HeaderNames.ContentLength, "0");
        await context.Response.Body.FlushAsync(context.RequestAborted);
        await context.Response.CompleteAsync();
    }

    /// <summary>
    /// Sets the response status code to <see cref="StatusCodes.Status502BadGateway"/>.
    /// </summary>
    public static void BadGateway(this HttpResponse response) =>
        response.StatusCode = StatusCodes.Status502BadGateway;
}
