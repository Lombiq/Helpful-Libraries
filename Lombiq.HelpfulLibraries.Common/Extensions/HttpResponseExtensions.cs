using System.Threading.Tasks;

namespace System.Net.Http;

public static class HttpResponseExtensions
{
    public static Task ThrowIfNotSuccessAsync(
        this HttpResponseMessage response,
        string errorMessage,
        HttpContent? requestBody = null)
    {
        static async Task ThrowIfNotSuccessInnerAsync(
            HttpResponseMessage response,
            string errorMessage,
            HttpContent? requestBody)
        {
            var requestContent = requestBody == null ? "<NULL>" : await requestBody.ReadAsStringAsync();
            throw new InvalidOperationException(string.Join(
                separator: '\n',
                errorMessage,
                $"Response: {response}",
                $"Response Content: {await response.Content.ReadAsStringAsync()}",
                $"Request Content: {requestContent}"));
        }

        return response.IsSuccessStatusCode
            ? Task.CompletedTask
            : ThrowIfNotSuccessInnerAsync(response, errorMessage, requestBody);
    }
}
