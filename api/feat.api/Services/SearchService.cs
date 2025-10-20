using System.Text.RegularExpressions;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using feat.api.Configuration;
using feat.api.Enums;
using feat.api.Models;
using feat.api.Models.External;
using feat.common;
using Microsoft.Extensions.Options;
using OpenAI.Embeddings;

namespace feat.api.Services;

public class SearchService : ISearchService
{
    private readonly AzureOptions _azureOptions;
    private readonly IApiClient _apiClient;
    private readonly SearchClient _aiSearchClient;
    private readonly EmbeddingClient _embeddingClient;
    
    public SearchService(
        IOptionsMonitor<AzureOptions> options,
        IApiClient apiClient,
        SearchClient aiSearchClient,
        EmbeddingClient embeddingClient)
    {
        _azureOptions = options.CurrentValue;
        _apiClient = apiClient;
        _aiSearchClient = aiSearchClient;
        _embeddingClient = embeddingClient;
    }
    
    public async Task<SearchResponse?> SearchAsync(SearchRequest request)
    {
        GeoLocation? geoLocation = null;

        if (request.Location != null)
        {
            geoLocation = await GetGeoLocationAsync(request.Location);
        }
        
        string filter;
        var orderBy = string.Empty;
        var radiusInKm = request.Radius * 1.60934;
        
        if (geoLocation != null)
        {
            filter = $"(geo.distance(GEOPOINT_LATLONG, geography'POINT({geoLocation.Latitude} {geoLocation.Longitude})') lt {radiusInKm})";

            if (request.IncludeOnlineCourses)
            {
                filter += " or (DELIVERY_MODE eq 'Online')";
            }
            else
            {
                filter += " and (DELIVERY_MODE ne 'Online')";
            }
        }
        else
        {
            filter = request.IncludeOnlineCourses ? "DELIVERY_MODE eq 'Online'" : "DELIVERY_MODE ne 'Online'";
        }

        if (geoLocation != null && request.OrderBy == OrderBy.Distance)
        {
            orderBy =
                $"geo.distance(GEOPOINT_LATLONG, geography'POINT({geoLocation.Latitude} {geoLocation.Longitude})')";
        }
        
        var search = await AiSearchAsync(
            request.Query,
            filter,
            orderBy,
            request.Page,
            request.PageSize,
            request.SessionId);
        
        var searchResults = search.Value;
        
        var result = new SearchResponse
        {
            Page = request.Page,
            PageSize = request.PageSize,
            Courses = [],
            Facets = searchResults.Facets?
                         .Where(f => f.Value?.Any() == true)
                         .Select(f => new Facet(f.Key, f.Value))
                         .ToList()
                     ?? new List<Facet>()
        };

        await foreach (var searchResult in search.Value.GetResultsAsync())
        {
            var course = new Course(searchResult.Document, searchResult.SemanticSearch?.RerankerScore, geoLocation);
            result.Courses.Add(course);
        }

        result.TotalCount = searchResults.TotalCount;

        return result;
    }

    private async Task<Response<SearchResults<AiSearchCourse>>> AiSearchAsync(
        string query,
        string filter,
        string orderBy,
        int page,
        int pageSize,
        string? sessionId)
    {
        var embedding = await _embeddingClient.GenerateEmbeddingAsync(query);
        var vector = embedding.Value.ToFloats();
        
        return await _aiSearchClient.SearchAsync<AiSearchCourse>(
            query,
            new SearchOptions
            {
                ScoringProfile = _azureOptions.AiSearchIndexScoringProfile,
                ScoringParameters = { _azureOptions.AiSearchIndexScoringParameters },
                VectorSearch = new VectorSearchOptions
                {
                    Queries =
                    {
                        new VectorizedQuery(vector)
                        {
                            KNearestNeighborsCount = _azureOptions.Knn,
                            Fields = { "COURSE_NAME_Vector", "DESCRIPTION_Vector", "ENTRY_Vector", "SECTOR_Vector", "SSAT1_Vector", "SSAT2_Vector"},
                            Weight = _azureOptions.Weight,
                        }
                    },
                },
                SearchFields =
                {
                    nameof(AiSearchCourse.COURSE_NAME), 
                    nameof(AiSearchCourse.WHO_THIS_COURSE_IS_FOR)
                },
                Facets =
                {
                    nameof(AiSearchCourse.DELIVERY_MODE), 
                    nameof(AiSearchCourse.STUDY_MODE), 
                    nameof(AiSearchCourse.SECTOR), 
                    nameof(AiSearchCourse.DATA_SOURCE), 
                    nameof(AiSearchCourse.LEVEL),
                    nameof(AiSearchCourse.QUALIFICATION_TYPE)
                },
                Filter = filter,
                IncludeTotalCount = true,
                Size = pageSize,
                Skip = (page - 1) * pageSize,
                SessionId = sessionId,
                OrderBy = { orderBy },
                SemanticSearch = new SemanticSearchOptions()
                {
                    SemanticConfigurationName = "semantic-title-description",
                },
                SearchMode = SearchMode.Any,
                HighlightFields =
                {
                    nameof(AiSearchCourse.COURSE_NAME), 
                    nameof(AiSearchCourse.WHO_THIS_COURSE_IS_FOR)
                },
                QueryType = SearchQueryType.Semantic,
            }
        );
    }

    private async Task<GeoLocation?> GetGeoLocationAsync(string location)
    {
        const string postcodePattern = @"^[a-z]{1,2}\d[a-z\d]?\s*\d[a-z]{2}$";
        var isPostcode = Regex.IsMatch(location, postcodePattern, RegexOptions.IgnoreCase);
        
        if (isPostcode)
        {
            var response = await _apiClient
                .GetAsync<PostcodeResult>(ApiClientNames.Postcode, $"postcodes/{location}");
            
            if (response.Result != null)
            {
                return new GeoLocation
                {
                    Latitude = response.Result.Latitude.GetValueOrDefault(),
                    Longitude = response.Result.Longitude.GetValueOrDefault()
                };
            }
        }
        else
        {
            var response = await _apiClient
                .GetAsync<PlaceResult>(ApiClientNames.Postcode, $"places/?q={location}&limit=1");
            
            if (response?.Result?.Count > 0)
            {
                var place = response.Result[0];
                
                return new GeoLocation
                {
                    Latitude = place.Latitude.GetValueOrDefault(),
                    Longitude = place.Longitude.GetValueOrDefault()
                };
            }
        }

        return null;
    }
}