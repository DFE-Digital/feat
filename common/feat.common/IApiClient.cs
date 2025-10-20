namespace feat.common;

public interface IApiClient
{
    Task<T> GetAsync<T>(string clientName, string url);

    Task<T> PostAsync<T>(string clientName, string url, object body);
}