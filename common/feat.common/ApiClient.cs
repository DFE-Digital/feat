using System.Net;
using System.Net.Http.Json;

namespace feat.common;

public class ApiClient : IApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<T?> GetAsync<T>(
        string clientName,
        string url,
        CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient(clientName);

        var response = await client.GetAsync(url, cancellationToken);
        
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return default;
        }
        
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"GET {url} failed with {(int)response.StatusCode} ({response.ReasonPhrase}). Content: {content}"
            );
        }

        var result = await response.Content.ReadFromJsonAsync<T>(cancellationToken)
                     ?? throw new InvalidOperationException($"Failed to deserialize response from {url}");

        return result;
    }

    public async Task<T> PostAsync<T>(
        string clientName,
        string url,
        object body,
        CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient(clientName);

        var response = await client.PostAsJsonAsync(url, body, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"POST {url} failed with {(int)response.StatusCode} ({response.ReasonPhrase}). Content: {content}"
            );
        }

        var result = await response.Content.ReadFromJsonAsync<T>(cancellationToken)
                     ?? throw new InvalidOperationException($"Failed to deserialize response from {url}");

        return result;
    }
}