using feat.common;
using feat.common.Models.Enums;
using feat.web.Configuration;
using feat.web.Models;
using feat.web.Models.ViewModels;
using Microsoft.Extensions.Options;

namespace feat.web.Services;

public class SudoSearchService : ISearchService
{
    private readonly IApiClient _apiClient;
    private readonly IOptions<SearchOptions> _options;
    
    private List<SearchResult> SearchResultsInner = new();

    public SudoSearchService(
        IApiClient apiClient,
        IOptions<SearchOptions> options)
    {
        _apiClient = apiClient;
        _options = options;
    }

    private IEnumerable<SearchResult> GenerateSearchResults(int iRecords = 50, string sortBy = "")
    {
        List<SearchResult> searchResults = new();
        for (int i = 0; i < iRecords; i++)
        {
            var item0 = new SearchResult
            {
                Id = ($"A{i}"), DistanceSudo = i / 2,
                CourseTitle = "Media A Level",
                ProviderName = "Leeds College",
                Location = "Leeds",
                Distance = $"{(i + 1)} miles",
                CourseType = (i % 2) == 0 ? CourseType.Degree : CourseType.ALevels,
                Requirements = $"{i} Be good at something something  something  something ",
                Overview =
                    $"A nice course {i % 5} something something  something  something  something  something  something  something  something  something  something  something  something  something ",
            };
            searchResults.Add(item0);

            var item1 = new SearchResult
            {
                Id = ($"B{i}"), DistanceSudo = (i + 2) / 2, 
                CourseTitle = "Media A Level",
                ProviderName = "Bradford College",
                Location = "Leeds",
                Distance = $"{i + 2} miles",
                CourseType = (i % 2) == 0 ? CourseType.Apprenticeship : CourseType.TLevels,
                Requirements =
                    $"{i % 3} There are no specific entry requirements however learners should have a minimum of Level 2 in English and Maths or equivalent.",
                Overview =
                    $"{i % 3} If you are looking to improve your knowledge of equality, diversity and inclusivity, and the four British Values, for either your personal or professional life, the Level 2 Certificate in Living in a Fair and Diverse Society is the course for you. This course will enable you to gain an understanding of equality, diversity and inclusion in daily life, the impact of the Equal Pay Act 1970 and the Equality Act 2010. You will also learn about supporting and promoting British Values.",
            };
            searchResults.Add(item1);
        }

        if (sortBy.Equals("distance", StringComparison.InvariantCultureIgnoreCase))
        {
            searchResults = searchResults.OrderBy(x => x.Distance).ToList();
        }
        else if(sortBy.Equals("relevance", StringComparison.InvariantCultureIgnoreCase))
        {
            searchResults = searchResults.OrderBy(x => x.CourseTitle).ToList();
        }

        SearchResultsInner = searchResults;
        return searchResults;
    }
    private List<SearchResult> GetPagedSearchResults(int pageNumber, int pageSize=7)
    {
        if (!SearchResultsInner.Any())
        {
            GenerateSearchResults();
        }
        
        var totalSearchResults = SearchResultsInner.Count;
        var totalPages = (int)Math.Ceiling(totalSearchResults / (double)pageSize);
        
        int currentPage = pageNumber;
        if (currentPage < 1) currentPage = 1;
        if (currentPage > totalPages && totalPages > 0) currentPage = totalPages;

        var searchResults = SearchResultsInner
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        return searchResults;
    }

    
    public async Task<SearchResponse> Search(Search search, string sessionId)
    {
        await Task.Delay(1);
        GenerateSearchResults();
        
        return new SearchResponse()
        {
            SearchResults = SearchResultsInner,
            // selected facets / facets used in the search.
            Facets = new List<Facet>(),

            Page = 1,
            PageSize = 10,
            TotalCount = 100,
            SortBy = "Description"
        };
    }

    public async Task<SearchResponse> GetGlobalFacets()
    {
        var endpoint = new Uri(new Uri(_options.Value.ApiBaseUrl), "api/search/global-facets").ToString();
        return await _apiClient.GetAsync<SearchResponse>(ApiClientNames.Feat, endpoint);
    }

    public async Task<SearchResponse> GetFilteredSortedPagedCourses(Search search, string sessionId, string sortFilter, int pageNumber, int pageSize)
    {
        await Task.Delay(1);
        if (!SearchResultsInner.Any())
        {
            GenerateSearchResults();
        }
        
        var pagedItems = GetPagedSearchResults(pageNumber, pageSize);

        return new SearchResponse()
        {
            SearchResults = pagedItems,
            Facets = new List<Facet>(),
            
            Page = pageNumber,
            PageSize = pageSize,
            TotalCount = SearchResultsInner.Count,
            SortBy = sortFilter
        };
    }
    
    public async Task<SearchResponse> GetCourseDetails(string courseId)
    {
        var endpoint = new Uri(new Uri(_options.Value.ApiBaseUrl), "api/search/courseId").ToString();
        await Task.Delay(1);
        return await _apiClient.GetAsync<SearchResponse>(ApiClientNames.Feat, endpoint);
    }
}