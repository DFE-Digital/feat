using System.Text.RegularExpressions;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using feat.api.Data;
using feat.api.Enums;
using feat.api.Extensions;
using feat.api.Models;
using feat.api.Models.External;
using feat.common;
using feat.common.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenAI.Embeddings;
using ZiggyCreatures.Caching.Fusion;
using Location = feat.common.Models.Location;

namespace feat.api.Services;

public class SearchService(
    IOptionsMonitor<AzureOptions> options,
    IApiClient apiClient,
    SearchClient aiSearchClient,
    EmbeddingClient embeddingClient,
    CourseDbContext dbContext,
    IFusionCache cache)
    : ISearchService
{
    private readonly AzureOptions _azureOptions = options.CurrentValue;

    public async Task<(ValidationResult validation, SearchResponse? response)>
        SearchAsync(SearchRequest request)
    {
        var validation = new ValidationResult();

        GeoLocation? userLocation = null;

        if (!string.IsNullOrWhiteSpace(request.Location))
        {
            var locationResult = await GetGeoLocationAsync(request.Location!);

            if (!locationResult.IsValid)
            {
                validation.AddError("location", locationResult.ErrorMessage!);
            }
            else
            {
                userLocation = locationResult.Location;
            }
        }
        
        if (!validation.IsValid)
        {
            return (validation, null);
        }

        var (searchResults, facets, totalCount) = await AiSearchAsync(request, userLocation);
        
        var courses = await dbContext.EntryInstances
            .Where(ei => searchResults.Select(sr => sr.InstanceId).Contains(ei.Id))
            .Include(ei => ei.Location)
            .Include(ei => ei.Entry)
            .Include(ei => ei.Entry.Provider)
            .Include(ei => ei.Entry.Provider.ProviderLocations)
            .ThenInclude(pl => pl.Location)
            .AsSplitQuery()
            .Select(ei => new Course
            {
                Id = ei.Entry.Id,
                InstanceId = ei.Id,
                Title = ei.Entry.Title,
                Provider = ei.Entry.Provider.Name,
                CourseType = ei.Entry.CourseType,
                Requirements = ei.Entry.EntryRequirements,
                Overview = ei.Entry.Description,
                Location = ei.Location != null ? ei.Location.ToLocation().GeoLocation :
                        ei.Entry.Provider.ProviderLocations.FirstOrDefault() != null ?
                ei.Entry.Provider.ProviderLocations.First().Location.ToLocation().GeoLocation : null,
                LocationName =
                    ei.Location != null ? GetLocationName(ei.Location) :
                        ei.Entry.Provider.ProviderLocations.FirstOrDefault() != null ?
                            GetLocationName(ei.Entry.Provider.ProviderLocations.First().Location) : null
            })
            .ToListAsync();
        
        var courseDictionary = courses.ToDictionary(c => c.InstanceId);
        
        foreach (var result in searchResults)
        {
            var course = courseDictionary[result.InstanceId];
            
            course.Score = result.RerankerScore;
            course.CalculateDistance(userLocation);
        }

        RemoveDuplicateCourses(courses);
        
        courses = courses
            .OrderBy(c => searchResults.FindIndex(sr => sr.InstanceId == c.InstanceId))
            .ToList();

        var response = new SearchResponse
        {
            Courses = courses,
            Facets = facets,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
        
        return (validation, response);
    }
    
    public async Task<GeoLocationResponse> GetGeoLocationAsync(string location)
    {
        return await cache.GetOrSetAsync(
            $"location:{location}",
            async _ => await ResolveLocationAsync(location)
        );
    }
    
    private static void RemoveDuplicateCourses(List<Course> courses)
    {
        if (courses.Count == 0)
        {
            return;
        }
        
        // If the user has specified a search location, Distance will have a value,
        // so take the closest course to display.
        // If there is no search location, just take the first course.
        var deduplicatedCourses = courses
            .GroupBy(c => c.Id)
            .Select(g => g
                .Where(c => c.Distance != null)
                .OrderBy(c => c.Distance)
                .FirstOrDefault() ?? g.First())
            .ToList();
        
        courses.Clear();
        courses.AddRange(deduplicatedCourses);
    }

    private async Task<(List<AiSearchResult>, List<Facet>, int)> AiSearchAsync(
        SearchRequest request, GeoLocation? userLocation)
    {
        var searchOptions = await BuildSearchOptions(request, userLocation);

        var search = await aiSearchClient.SearchAsync<AiSearchResponse>(request.Query, searchOptions);
        var searchResults = search.Value;
        
        var results = new List<AiSearchResult>();

        await foreach (var searchResult in searchResults.GetResultsAsync())
        {
            results.Add(new AiSearchResult
            {
                Id = Guid.Parse(searchResult.Document.Id),
                InstanceId = Guid.Parse(searchResult.Document.InstanceId),
                RerankerScore = searchResult.SemanticSearch?.RerankerScore
            });
        }
        
        var facets = searchResults.Facets?
            .Where(f => f.Value?.Any() == true)
            .Select(f => new Facet(f.Key, f.Value))
            .ToList() ?? [];

        var totalCount = (int)search.Value.TotalCount!;
        
        return (results, facets, totalCount);
    }

    private Task<SearchOptions> BuildSearchOptions(SearchRequest request, GeoLocation? userLocation)
    {
        SearchOptions searchOptions;

        if (request.Query == "*")
        {
            searchOptions = new SearchOptions
            {
                SearchFields =
                {
                    nameof(SearchIndexFields.Title), 
                    nameof(SearchIndexFields.Description)
                },
                Facets =
                {
                    nameof(SearchIndexFields.CourseType), 
                    nameof(SearchIndexFields.QualificationLevel), 
                    nameof(SearchIndexFields.LearningMethod), 
                    nameof(SearchIndexFields.CourseHours), 
                    nameof(SearchIndexFields.StudyTime)
                }
            };
        }
        else
        {
            searchOptions = new SearchOptions
            {
                SearchFields =
                {
                    nameof(SearchIndexFields.Title), 
                    nameof(SearchIndexFields.Description)
                },
                Facets =
                {
                    nameof(SearchIndexFields.CourseType), 
                    nameof(SearchIndexFields.QualificationLevel), 
                    nameof(SearchIndexFields.LearningMethod), 
                    nameof(SearchIndexFields.CourseHours), 
                    nameof(SearchIndexFields.StudyTime)
                },
                HighlightFields =
                {
                    nameof(SearchIndexFields.Title),
                    nameof(SearchIndexFields.Description)
                }
            };
        }

        var embedding = cache.GetOrSet(
            $"query:{request.Query.ToLowerInvariant()}",
            entry =>
            {
                var result = embeddingClient.GenerateEmbedding(request.Query.ToLowerInvariant(), cancellationToken: entry);
                return result.Value.ToFloats();
            }
        );
        
        var filterExpression = BuildFilterExpression(request, userLocation);
        
        if (request.OrderBy == OrderBy.Distance && userLocation != null)
        {
            searchOptions.OrderBy.Add(
                $"geo.distance(Location, geography'POINT({userLocation.Longitude} {userLocation.Latitude})') asc"
            );
            
            searchOptions.QueryType = SearchQueryType.Simple;
        }
        else
        {
            searchOptions.QueryType = SearchQueryType.Semantic;
            searchOptions.SemanticSearch = new SemanticSearchOptions
            {
                SemanticConfigurationName = "semantic-title-description"
            };
        }
        
        searchOptions.SessionId = request.SessionId;
        searchOptions.Size = request.PageSize;
        searchOptions.Skip = (request.Page - 1) * request.PageSize;
        searchOptions.IncludeTotalCount = true;
        searchOptions.Filter = filterExpression;
        searchOptions.SearchMode = SearchMode.Any;
        searchOptions.VectorSearch = new VectorSearchOptions
        {
            Queries =
            {
                new VectorizedQuery(embedding)
                {
                    KNearestNeighborsCount = _azureOptions.Knn,
                    Fields = { "TitleVector", "LearningAimTitleVector", "DescriptionVector", "SectorVector" },
                    Weight = _azureOptions.Weight,
                }
            },
        };

        return Task.FromResult(searchOptions);
    }
    
    private async Task<GeoLocationResponse> ResolveLocationAsync(string location)
    {
        const string postcodePattern = @"^[a-z]{1,2}\d[a-z\d]?\s*\d[a-z]{2}$";
        var isPostcode = Regex.IsMatch(location, postcodePattern, RegexOptions.IgnoreCase);

        if (isPostcode)
        {
            var postcode = await dbContext.LookupPostcodes.FirstOrDefaultAsync(p =>
                    p.Postcode.ToLower().Replace(" ", "") == location.ToLower().Replace(" ", "")
                    && p.Valid
                );

            if (postcode is { Latitude: not null, Longitude: not null })
            {
                return new GeoLocationResponse(
                    new GeoLocation
                    {
                        Latitude = postcode.Latitude.Value,
                        Longitude = postcode.Longitude.Value
                    },
                    true
                );
            }

            return new GeoLocationResponse(null, false, "Postcode not found.");
        }

        var response = await apiClient
            .GetAsync<PlaceResult>(ApiClientNames.Postcode, $"places/?q={location}&limit=1");

        if (response.Result?.Count > 0)
        {
            var place = response.Result[0];

            return new GeoLocationResponse(
                new GeoLocation
                {
                    Latitude = place.Latitude.GetValueOrDefault(),
                    Longitude = place.Longitude.GetValueOrDefault()
                },
                true
            );
        }

        return new GeoLocationResponse(null, false, "Location not found.");
    }
    
    private static string? BuildFilterExpression(SearchRequest request, GeoLocation? userLocation)
    {
        var filters = new List<string>();

        AddFacet(nameof(SearchIndexFields.CourseType), request.CourseType);
        AddFacet(nameof(SearchIndexFields.QualificationLevel), request.QualificationLevel);
        AddFacet(nameof(SearchIndexFields.LearningMethod), request.LearningMethod);
        AddFacet(nameof(SearchIndexFields.CourseHours), request.CourseHours);
        AddFacet(nameof(SearchIndexFields.StudyTime), request.StudyTime);
        
        if (userLocation != null)
        {
            var radius = RadiusInKilometers(request.Radius);
            
            filters.Add(
                $"(geo.distance(Location, geography'POINT({userLocation.Longitude} {userLocation.Latitude})') le {radius} or Location eq null)"
            );
        }
        
        return filters.Count != 0
            ? string.Join(" and ", filters)
            : null;

        void AddFacet(string field, IEnumerable<string>? values)
        {
            var valueList = (values ?? []).ToList();
            if (valueList.Count == 0)
            {
                return;
            }
            
            var ors = valueList.Select(v => $"{field} eq '{v.Replace("'", "''")}'");
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
               ?? location?.Name;
    }

    private static double RadiusInKilometers(double radius)
    {
        return radius * 1.60934;
    }
}
