namespace feat.web.Repositories;

public class HttpClientRepository(IHttpClientFactory httpClientFactory)
{
    public async Task<T?> GetAsync<T>(string url)
    {
        var client = httpClientFactory.CreateClient("HttpClient");
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            throw new Exception($"{response.StatusCode}: {response.ReasonPhrase}");
        var responseObject = await response.Content.ReadFromJsonAsync<T>();
        return responseObject;
    }
 
}