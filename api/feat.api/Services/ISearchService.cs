using feat.api.Models;

namespace feat.api.Services;

public interface ISearchService
{
    Task<(ValidationResult validation, SearchResponse? response)>
        SearchAsync(SearchRequest request);

    Task<GeoLocationResponse> GetGeoLocationAsync(string location);
}