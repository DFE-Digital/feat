using System.Text.RegularExpressions;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using feat.api.Data;
using feat.api.Enums;
using feat.api.Models;
using feat.api.Models.External;
using feat.common;
using feat.common.Configuration;
using feat.common.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenAI.Embeddings;
using Location = feat.common.Models.Location;

namespace feat.api.Services;

public class SearchService(
    IOptionsMonitor<AzureOptions> options,
    IApiClient apiClient,
    SearchClient aiSearchClient,
    EmbeddingClient embeddingClient,
    CourseDbContext dbContext)
    : ISearchService
{
    private readonly AzureOptions _azureOptions = options.CurrentValue;

    public async Task<SearchResponse?> SearchAsync(SearchRequest request)
    {
        GeoLocation? userLocation = null;

        if (request.Location != null)
        {
            userLocation = await GetGeoLocationAsync(request.Location);
        }

        var (uniqueCourses, facets, totalCount) = await AiSearchAsync(request, userLocation);
        
        var courseIds = uniqueCourses
            .Select(x => x.Id)
            .ToList();

        var courses = await dbContext.Entries
            .AsNoTracking()
            .Where(e => courseIds.Contains(e.Id))
            .Select(e => new Course
            {
                Id = e.Id,
                Title = e.Title,
                Provider = e.Provider.Name,
                CourseType = e.CourseType,
                Requirements = e.EntryRequirements,
                Overview = e.Description
            }).ToListAsync();
        
        var locationIds = uniqueCourses
            .Select(x => x.InstanceId.Split('_'))
            .Where(parts => parts.Length == 2)
            .Select(parts => parts[1])
            .ToList();

        var locations = await dbContext.Locations
            .AsNoTracking()
            .Where(l => locationIds.Contains(l.Id.ToString()))
            .ToListAsync();
        
        var courseDictionary = courses.ToDictionary(c => c.Id);

        var orderedCourses = uniqueCourses
            .Where(c => courseDictionary.ContainsKey(c.Id))
            .Select(c =>
            {
                var course = courseDictionary[c.Id];
                course.Score = c.RerankerScore;
                
                var locationIdParts = c.InstanceId.Split('_');
                var locationId = locationIdParts.Length > 1 ? locationIdParts[1] : null;

                if (locationId != null)
                {
                    var location = locations.FirstOrDefault(l => l.Id.ToString() == locationId);

                    var courseLocation = location?.GeoLocation != null
                        ? new GeoLocation { Longitude = location.GeoLocation.X, Latitude = location.GeoLocation.Y }
                        : null;

                    var locationName = GetLocationName(location);

                    course.SetLocation(courseLocation, locationName, userLocation);
                }
                else
                {
                    course.SetLocation(null, null, userLocation);
                }

                return course;
            });

        if (request.OrderBy == OrderBy.Distance)
        {
            orderedCourses = orderedCourses
                .Where(c => c.Distance.HasValue && c.Distance.Value <= request.Radius)
                .OrderBy(c => c.Distance)
                .ToList();
        }
        else if (request.OrderBy == OrderBy.Relevance)
        {
            orderedCourses = orderedCourses
                .OrderByDescending(c => c.Score)
                .ToList();
        }

        orderedCourses = orderedCourses
            .Take(request.PageSize)
            .ToList();

        return new SearchResponse
        {
            Courses = orderedCourses,
            Facets = facets,
            Page = request.Page,
            PageSize = request.PageSize,
            CurrentPageSize = uniqueCourses.Count,
            TotalCount = totalCount
        };
    }

    private async Task<(List<AiSearchResult>, List<Facet>, int)> AiSearchAsync(SearchRequest request, GeoLocation? userLocation)
    {
        var embedding = await embeddingClient.GenerateEmbeddingAsync(request.Query);
        var vector = embedding.Value.ToFloats();

        var filterExpression = BuildFacetFilterExpression(request, userLocation);

        var searchOptions = new SearchOptions
        {
            //ScoringProfile = _azureOptions.AiSearchIndexScoringProfile,
            //ScoringParameters = { _azureOptions.AiSearchIndexScoringParameters },
            SessionId = request.SessionId,
            SearchMode = SearchMode.Any,
            QueryType = SearchQueryType.Semantic,
            SemanticSearch = new SemanticSearchOptions
            {
                SemanticConfigurationName = "semantic-title-description",
            },
            Size = request.PageSize,
            Skip = (request.Page - 1) * request.PageSize,
            IncludeTotalCount = true,
            VectorSearch = new VectorSearchOptions
            {
                Queries =
                {
                    new VectorizedQuery(vector)
                    {
                        KNearestNeighborsCount = _azureOptions.Knn,
                        Fields = { "TitleVector", "LearningAimTitleVector", "DescriptionVector", "SectorVector" },
                        Weight = _azureOptions.Weight,
                    }
                },
            },
            SearchFields =
            {
                nameof(SearchIndexFields.Title), 
                nameof(SearchIndexFields.Description)
            },
            HighlightFields =
            {
                nameof(SearchIndexFields.Title), 
                nameof(SearchIndexFields.Description)
            },
            Facets =
            {
                nameof(SearchIndexFields.EntryType), 
                nameof(SearchIndexFields.QualificationLevel), 
                nameof(SearchIndexFields.LearningMethod), 
                nameof(SearchIndexFields.CourseHours), 
                nameof(SearchIndexFields.StudyTime),
            },
            Filter = filterExpression
        };

        var search = await aiSearchClient.SearchAsync<AiSearchResponse>(request.Query, searchOptions);
        var searchResults = search.Value;

        var checkedIds = new HashSet<Guid>();
        var uniqueCourses = new List<AiSearchResult>();

        await foreach (var searchResult in searchResults.GetResultsAsync())
        {
            var result = new AiSearchResult
            {
                Id = searchResult.Document.Id,
                InstanceId = searchResult.Document.InstanceId,
                RerankerScore = searchResult.SemanticSearch?.RerankerScore
            };
            
            if (checkedIds.Add(result.Id)) 
            {
                uniqueCourses.Add(result);
            }
        }
        
        var facets = searchResults.Facets?
                         .Where(f => f.Value?.Any() == true)
                         .Select(f => new Facet(f.Key, f.Value))
                         .ToList()
                     ?? [];

        var totalCount = (int)search.Value.TotalCount!;
        
        return (uniqueCourses, facets, totalCount);
    }
    
    private async Task<GeoLocation?> GetGeoLocationAsync(string location)
    {
        const string postcodePattern = @"^[a-z]{1,2}\d[a-z\d]?\s*\d[a-z]{2}$";
        var isPostcode = Regex.IsMatch(location, postcodePattern, RegexOptions.IgnoreCase);
        
        if (isPostcode)
        {
            var postcode = dbContext.Postcodes.FirstOrDefault(p =>
                p.Postcode.ToLower().Replace(" ", "") == location.ToLower().Replace(" ", "")
            );

            if (postcode is { Latitude: not null, Longitude: not null })
            {
                return new GeoLocation
                {
                    Latitude = postcode.Latitude.Value,
                    Longitude = postcode.Longitude.Value
                };
            }
        }
        else
        {
            var response = await apiClient
                .GetAsync<PlaceResult>(ApiClientNames.Postcode, $"places/?q={location}&limit=1");
            
            if (response.Result?.Count > 0)
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
    
    private static string? BuildFacetFilterExpression(SearchRequest request, GeoLocation? userLocation)
    {
        var filters = new List<string>();

        AddFacet(nameof(SearchIndexFields.EntryType), request.EntryType);
        AddFacet(nameof(SearchIndexFields.QualificationLevel), request.QualificationLevel);
        AddFacet(nameof(SearchIndexFields.LearningMethod), request.LearningMethod);
        AddFacet(nameof(SearchIndexFields.CourseHours), request.CourseHours);
        AddFacet(nameof(SearchIndexFields.StudyTime), request.StudyTime);
        
        if (userLocation != null)
        {
            var radiusKm = request.Radius * 1.60934;
            
            filters.Add(
                $"geo.distance(Location, geography'POINT({userLocation.Longitude} {userLocation.Latitude})') le {radiusKm}"
            );
        }
        
        return filters.Count != 0
            ? string.Join(" and ", filters)
            : null;

        void AddFacet(string field, IEnumerable<string>? values)
        {
            if (values == null || !values.Any())
            {
                return;
            }
            
            var ors = values.Select(v => $"{field} eq '{v.Replace("'", "''")}'");
            filters.Add("(" + string.Join(" or ", ors) + ")");
        }
    }
    
    private static string? GetLocationName(Location? location)
    {
        return location?.Town
               ?? location?.Address4
               ?? location?.Address3
               ?? location?.Address2
               ?? location?.Address1
               ?? null;
    }
}
