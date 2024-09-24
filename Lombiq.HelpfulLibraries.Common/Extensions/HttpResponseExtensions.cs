using System.Threading.Tasks;

namespace System.Net.Http;

public static class HttpResponseExtensions
{
    public static async Task ThrowIfNotSuccessAsync(
        this HttpResponseMessage response,
        string errorMessage,
        HttpContent requestBody = null)
    {
        if (!response.IsSuccessStatusCode)
        {
            var requestContent = requestBody == null ? "<NULL>" : await requestBody.ReadAsStringAsync();
            throw new InvalidOperationException(string.Join(
                separator: '\n',
                errorMessage,
                $"Response: {response}",
                $"Response Content: {await response.Content.ReadAsStringAsync()}",
                $"Request Content: {requestContent}"));
        }
    }
}
