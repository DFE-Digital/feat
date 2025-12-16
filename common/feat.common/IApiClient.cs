namespace feat.common;

public interface IApiClient
{
    Task<T?> GetAsync<T>(
        string clientName,
        string url,
        CancellationToken cancellationToken = default);

    Task<T> PostAsync<T>(
        string clientName,
        string url,
        object body,
        CancellationToken cancellationToken = default);
}