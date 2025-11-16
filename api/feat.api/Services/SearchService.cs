using System.Text.RegularExpressions;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using feat.api.Enums;
using feat.api.Models;
using feat.api.Models.External;
using feat.common;
using feat.common.Configuration;
using feat.common.Models.AiSearch;
using Microsoft.Extensions.Options;
using OpenAI.Embeddings;

namespace feat.api.Services;

public class SearchService(
    IOptionsMonitor<AzureOptions> options,
    IApiClient apiClient,
    SearchClient aiSearchClient,
    EmbeddingClient embeddingClient)
    : ISearchService
{
    private readonly AzureOptions _azureOptions = options.CurrentValue;

    public async Task<SearchResponse?> SearchAsync(SearchRequest request)
    {
        GeoLocation? geoLocation = null;

        if (request.Location != null)
        {
            geoLocation = await GetGeoLocationAsync(request.Location);
        }
        
        var filter = String.Empty;
        var orderBy = string.Empty;
        var radiusInKm = request.Radius * 1.60934;
        
        if (geoLocation != null)
        {
            filter = $"(geo.distance(Location, geography'POINT({geoLocation.Latitude} {geoLocation.Longitude})') lt {radiusInKm})";

            /*
            if (request.IncludeOnlineCourses)
            {
                filter += " or (LearningMethod eq 'Online')";
            }
            else
            {
                filter += " and (LearningMethod ne 'Online')";
            }
             */
        }
        else
        {
            // filter = request.IncludeOnlineCourses ? "LearningMethod eq 'Online'" : "LearningMethod ne 'Online'";
        }

        if (geoLocation != null && request.OrderBy == OrderBy.Distance)
        {
            orderBy =
                $"geo.distance(Location, geography'POINT({geoLocation.Latitude} {geoLocation.Longitude})')";
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

    private async Task<Response<SearchResults<AiSearchEntry>>> AiSearchAsync(
        string query,
        string filter,
        string orderBy,
        int page,
        int pageSize,
        string? sessionId)
    {
        var embedding = await embeddingClient.GenerateEmbeddingAsync(query);
        var vector = embedding.Value.ToFloats();
        
        return await aiSearchClient.SearchAsync<AiSearchEntry>(
            query,
            new SearchOptions
            {
                // ScoringProfile = _azureOptions.AiSearchIndexScoringProfile,
                // ScoringParameters = { _azureOptions.AiSearchIndexScoringParameters },
                VectorSearch = new VectorSearchOptions
                {
                    Queries =
                    {
                        new VectorizedQuery(vector)
                        {
                            KNearestNeighborsCount = _azureOptions.Knn,
                            Fields = { "TitleVector", "DescriptionVector", "SectorVector", "LearningAimTitleVector"},
                            Weight = _azureOptions.Weight,
                        }
                    },
                },
                SearchFields =
                {
                    nameof(AiSearchEntry.Title), 
                    nameof(AiSearchEntry.Description)
                },
                Facets =
                {
                    nameof(AiSearchEntry.EntryType), 
                    nameof(AiSearchEntry.QualificationLevel), 
                    nameof(AiSearchEntry.LearningMethod), 
                    nameof(AiSearchEntry.CourseHours), 
                    nameof(AiSearchEntry.StudyTime)
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
                    nameof(AiSearchEntry.Title), 
                    nameof(AiSearchEntry.Description)
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
            var response = await apiClient
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
            var response = await apiClient
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