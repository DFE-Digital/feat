using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.Net.Http.Headers;

namespace feat.web.Repositories;

public class HttpClientRepository(IHttpClientFactory httpClientFactory)
{
    public async Task<T?> GetAsync<T>(string url)
    {
        var client = httpClientFactory.CreateClient("HttpClient");
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);
        var response = await client.SendAsync(httpRequestMessage);
        if (!response.IsSuccessStatusCode)
            throw new Exception($"{response.StatusCode}: {response.ReasonPhrase}");
        var responseObject = await response.Content.ReadFromJsonAsync<T>();
        return responseObject;
    }
    
    public async Task<T?> PostAsync<T>(string url, object body)
    {
        var client = httpClientFactory.CreateClient("HttpClient");
        
        var response = await client.PostAsJsonAsync(url, body);
        if (!response.IsSuccessStatusCode)
            throw new Exception($"{response.StatusCode}: {response.ReasonPhrase}");
        var responseObject = await response.Content.ReadFromJsonAsync<T>();
        return responseObject;
    }
 
}