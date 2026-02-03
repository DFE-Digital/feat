using System.Text.RegularExpressions;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using feat.api.Data;
using feat.api.Enums;
using feat.api.Extensions;
using feat.api.Models;
using feat.common.Configuration;
using feat.common.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenAI.Embeddings;
using ZiggyCreatures.Caching.Fusion;
using Location = feat.common.Models.Location;

namespace feat.api.Services;

public class SearchService(
    IOptionsMonitor<AzureOptions> options,
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
        
        if (!string.IsNullOrWhiteSpace(request.Location) && request.Radius <= 0)
        {
            validation.AddError("radius", "Radius must be greater than 0.");
            return (validation, null);
        }

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
                DeliveryMode = ei.StudyMode,
                Requirements = ei.Entry.EntryRequirements,
                Overview = ei.Entry.Description,
                Location = ei.Location != null
                    ? ei.Location.ToLocation().GeoLocation
                    : null,
                LocationName = ei.Location != null
                    ? GetLocationName(ei.Location)
                    : null,
                IsNational = ei.Entry.Vacancies.FirstOrDefault() != null
                    ? ei.Entry.Vacancies.First().NationalVacancy
                    : null
            })
            .ToListAsync();
        
        var courseDictionary = courses.ToDictionary(c => c.InstanceId);
        
        foreach (var result in searchResults.Where(c => courseDictionary.ContainsKey(c.InstanceId)))
        {
            var course = courseDictionary[result.InstanceId];
            
            course.Score = result.RerankerScore;
            course.CalculateDistance(userLocation);
        }

        RemoveDuplicateCourses(courses);
        
        courses = ApplySearchOrdering(courses, searchResults);

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

    public async Task<AutoCompleteLocation[]> GetAutoCompleteLocationsAsync(string location)
    {
        if (string.IsNullOrWhiteSpace(location) || location.Length < 3)
            return [];

        var searchTerm = location.Trim();
        searchTerm = searchTerm.Replace("'", "").Replace("(", "").Replace(")", "").Replace("-", " ");
        
        var fromLocations =
            dbContext.LookupLocations
                .AsNoTracking()
                .Where(l =>
                    (EF.Functions.Like(l.CleanName, $"{searchTerm}%") || EF.Functions.Like(l.CleanName, $"%{searchTerm}%")) &&
                    l.Latitude != null && l.Longitude != null)
                .Select(l => new
                {
                    Text = l.Name,
                    l.Latitude,
                    l.Longitude,
                    Rank = l.Name == searchTerm
                        ? 1
                        : EF.Functions.Like(l.CleanName, $"{searchTerm}%") ? 2 : 3
                })
                .OrderBy(x => x.Rank)
                .ThenBy(x => x.Text)
                .Take(5);
        
        var fromPostcodes =
            dbContext.LookupPostcodes
                .AsNoTracking()
                .Where(p =>
                    p.CleanPostcode.StartsWith(searchTerm.Replace(" ", "")) &&
                    (p.Expired == null || p.Expired > DateTime.Today) &&
                    p.Latitude != null && p.Longitude != null)
                .Select(p => new
                {
                    Text = p.Postcode,
                    p.Latitude,
                    p.Longitude,
                    Rank = p.CleanPostcode == searchTerm.Replace(" ", "")
                        ? 0
                        : (EF.Functions.Like(p.CleanPostcode, $"{searchTerm.Replace(" ", "")}%")) ? 1 : 2
                })
                .OrderBy(x => x.Rank)
                .ThenBy(x => x.Text)
                .Take(5);

        var query =
            fromLocations
                .Concat(fromPostcodes)
                .GroupBy(x => new { x.Text, x.Latitude, x.Longitude })
                .Select(g => new
                {
                    g.Key.Text,
                    g.Key.Latitude,
                    g.Key.Longitude,
                    Rank = g.Min(x => x.Rank)
                })
                .OrderBy(x => x.Rank)
                .ThenBy(x => x.Text)
                .Take(5);

        var results = await query.ToListAsync();

        if (results.Any(x => x.Rank == 0))
        {
            var firstResult = results.First(x => x.Rank == 0);
            
            return
            [
                new AutoCompleteLocation
                {
                    Name = firstResult.Text,
                    Latitude = firstResult.Latitude!.Value,
                    Longitude = firstResult.Longitude!.Value
                }
            ];
        }

        return results
            .OrderBy(x => x.Rank)
            .ThenBy(x => x.Text)
            .Select(x => new AutoCompleteLocation
        {
            Name = x.Text,
            Latitude = x.Latitude!.Value,
            Longitude = x.Longitude!.Value
        }).ToArray();
    }
    
    public static string? BuildFilterExpression(SearchRequest request, GeoLocation? userLocation)
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
                $"""
                 (
                     geo.distance(
                         Location,
                         geography'POINT({userLocation.Longitude} {userLocation.Latitude})'
                     ) le {radius}
                     or LearningMethod eq 'Online'
                     or IsNational eq true
                 )
                 """
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
    
    public static List<Course> ApplySearchOrdering(
        List<Course> courses,
        List<AiSearchResult> searchResults)
    {
        return courses
            .OrderBy(c => searchResults.FindIndex(sr => sr.InstanceId == c.InstanceId))
            .ToList();
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

        var search = await aiSearchClient.SearchAsync<AiSearchResponse>(request.LuceneQuery, searchOptions);
        
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
        var searchOptions = new SearchOptions
        {
            Select =
            {
                nameof(SearchIndexFields.Id),
                nameof(SearchIndexFields.InstanceId),
            },
            SearchFields =
            {
                nameof(SearchIndexFields.Title),
                nameof(SearchIndexFields.Description),
                nameof(SearchIndexFields.LearningAimTitle),
                nameof(SearchIndexFields.Sector)
            },
            Facets =
            {
                nameof(SearchIndexFields.CourseType),
                nameof(SearchIndexFields.QualificationLevel),
                nameof(SearchIndexFields.LearningMethod),
                nameof(SearchIndexFields.CourseHours),
                nameof(SearchIndexFields.StudyTime)
            },
            QueryType = 
                request.Query.Length == 0 || request.OrderBy == OrderBy.Distance ? SearchQueryType.Simple : SearchQueryType.Semantic
        };

        var embeddings = new Dictionary<string, ReadOnlyMemory<float>>();
        
        foreach (var query in request.Query)
        {
            var embedding = cache.GetOrSet(
                $"query:{query.ToLowerInvariant()}",
                entry =>
                {
                    var result = embeddingClient.GenerateEmbedding(query.ToLowerInvariant(), cancellationToken: entry);
                    return result.Value.ToFloats();
                }
            );
            embeddings[query.ToLowerInvariant()] = embedding;
        }
        
        

        var filterExpression = BuildFilterExpression(request, userLocation)?.ReplaceLineEndings("");
        
        if (request.OrderBy == OrderBy.Distance && userLocation != null)
        {
            searchOptions.OrderBy.Add(
                $"geo.distance(Location, geography'POINT({userLocation.Longitude} {userLocation.Latitude})') asc"
            );
        }
        else if (request.Query.Length > 0)
        {
            searchOptions.QueryLanguage = QueryLanguage.EnGb;
            searchOptions.SemanticSearch = new SemanticSearchOptions
            {
                SemanticConfigurationName = "semantic-title-description",
                SemanticQuery = request.LuceneQuery
            };
        }
        
        searchOptions.SessionId = request.SessionId;
        searchOptions.Size = request.PageSize;
        searchOptions.Skip = (request.Page - 1) * request.PageSize;
        searchOptions.IncludeTotalCount = true;
        searchOptions.Filter = filterExpression;
        searchOptions.SearchMode = SearchMode.All;
        searchOptions.VectorSearch = new VectorSearchOptions();
        
        foreach (var embedding in embeddings.Values)
        {
            searchOptions.VectorSearch.Queries.Add(new VectorizedQuery(embedding)
            {
                KNearestNeighborsCount = _azureOptions.Knn,
                Fields = { 
                    $"{nameof(SearchIndexFields.Title)}Vector", 
                    $"{nameof(SearchIndexFields.Description)}Vector",
                    $"{nameof(SearchIndexFields.LearningAimTitle) }Vector"
                },
                Weight = _azureOptions.Weight,
                //Exhaustive = true,
                //Threshold = new VectorSimilarityThreshold(0.3)
            });
        }
        
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
                    && (p.Expired == null || p.Expired > DateTime.Today)
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

        var locationLatLong = await dbContext.LookupLocations.FirstOrDefaultAsync(l =>
            l.Name == location);

        if (locationLatLong is { Latitude: not null, Longitude: not null })
        {
            return new GeoLocationResponse(
                new GeoLocation
                {
                    Latitude = locationLatLong.Latitude.Value,
                    Longitude = locationLatLong.Longitude.Value
                },
                true
            );
        }

        return new GeoLocationResponse(null, false, "Location not found.");
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
