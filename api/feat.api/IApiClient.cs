namespace feat.api;

public interface IApiClient
{
    Task<T> GetAsync<T>(string clientName, string url);
}