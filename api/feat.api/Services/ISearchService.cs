using feat.api.Models;

namespace feat.api.Services;

public interface ISearchService
{
    Task<SearchResponse?> SearchAsync(SearchRequest request);

    Task<GeoLocationResponse> GetGeoLocationAsync(string location);
}