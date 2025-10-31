using feat.common;
using feat.common.Models.Enums;
using feat.web.Configuration;
using feat.web.Enums;
using feat.web.Models;
using feat.web.Models.ViewModels;
using Microsoft.Extensions.Options;
using CourseType = feat.common.Models.Enums.CourseType;

namespace feat.web.Services;

public class SudoSearchService : ISearchService
{
    private readonly IApiClient _apiClient;
    private readonly IOptions<SearchOptions> _options;
    
    private List<SearchResult> _searchResultsInner = new();
    private OrderBy _orderBy = OrderBy.Relevance;
    private int _pageSize = 2;   // No. of courses on a pages
    private int _pageNumber = 1; 

    public SudoSearchService(
        IApiClient apiClient,
        IOptions<SearchOptions> options)
    {
        _apiClient = apiClient;
        _options = options;
    }

    private IEnumerable<SearchResult> GenerateSearchResults(int iRecords, OrderBy orderBy)
    {
        List<SearchResult> searchResults = new();
        for (int i = 0; i < iRecords; i++)
        {
            var item0 = new SearchResult
            {
                CourseId = ($"a{i}"), 
                DistanceSudo = i / 2,
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
                CourseId = ($"b{i}"), 
                DistanceSudo = (i + 2) / 2, 
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

        if (orderBy == OrderBy.Distance)
        {
            searchResults = searchResults.OrderBy(x => x.DistanceSudo).ToList();
        }
        else if(orderBy == OrderBy.Relevance)
        {
            searchResults = searchResults.OrderBy(x => x.CourseTitle).ToList();
        }

        _searchResultsInner = searchResults;
        return searchResults;
    }
    private List<SearchResult> GetPagedSearchResults(int pageNumber, int pageSize)
    {
        var totalSearchResults = _searchResultsInner.Count;
        var totalPages = (int)Math.Ceiling(totalSearchResults / (double)pageSize);
        
        int currentPage = pageNumber;
        if (currentPage < 1) currentPage = 1;
        if (currentPage > totalPages && totalPages > 0) currentPage = totalPages;

        var searchResults = _searchResultsInner
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        return searchResults;
    }

    
    public async Task<SearchResponse> GetGlobalFacets()
    {
        var endpoint = new Uri(new Uri(_options.Value.ApiBaseUrl), "api/search/global-facets").ToString();
        return await _apiClient.GetAsync<SearchResponse>(ApiClientNames.Feat, endpoint);
    }

    public async Task<SearchResponse> Search(Search search, string sessionId) 
    {
        await Task.Delay(1);
        var request = search.ToSearchRequest();
        request.SessionId = sessionId;
        
        _pageSize = request.PageSize;
        _pageNumber = request.PageNumber;
        _orderBy = request.OrderBy; 
        
        if (!_searchResultsInner.Any())
        {
            GenerateSearchResults(50, _orderBy);
        }
        
        var pagedItems = GetPagedSearchResults(_pageNumber, _pageSize);

        return new SearchResponse()
        {
            SearchResults = pagedItems,
            Facets = new List<Facet>(),
            
            Page = _pageNumber,
            PageSize = _pageSize,
            TotalCount = _searchResultsInner.Count, 
            OrderBy = _orderBy
        };
    }

    public async Task<SearchResponse> GetCourseDetails(Search search, string sessionId)
    {
        await Task.Delay(1);
        var searchRequest = search.ToSearchRequest();
        searchRequest.SessionId = sessionId;

        if (!_searchResultsInner.Any())
        {
            GenerateSearchResults(50, _orderBy);
        }
        // Find the Id in teh data-store, and use that id to build of a specific return type.
        var courseLite = _searchResultsInner.Where(x => x.CourseId == search.CourseId).FirstOrDefault();

        if (courseLite == null)
        {
            return new SearchResponse();
        }

        if (courseLite.CourseType == CourseType.Degree)
        {
            var degree = GetDegreeDetails(courseLite);
            return new SearchResponse() { CourseDetails = degree };
        }
        else if (courseLite.CourseType == CourseType.Apprenticeship)
        {
            var aprenticeship = GetApprenticeshipDetails(courseLite);
            return new SearchResponse() { CourseDetails = aprenticeship };
        }
        else if (courseLite.CourseType == CourseType.Multiply)
        {
            // TODO Multiple locations (?)
        }
        
        var course = GetCourseDetails(courseLite);
        return new SearchResponse() { CourseDetails = course };
    }

    private CourseDetailsUniversity GetDegreeDetails(SearchResult courseLite)
    {
        return new CourseDetailsUniversity
        {
            Type = CourseType.Degree,
            Level = 6,
            EntryRequirements = courseLite.Requirements + " Entry requirements",
            TuitionFee = 1000,
            AwardingOrganisation = "University",
            Description = "A Degree course "+courseLite.Overview,
            University = courseLite.ProviderName + " -University",
            CampusName = "Campus",
            CampusAddress = new Location { Address1 = "10 Street", Town = "Town", Postcode = "W12 6LA" },
            DeliveryMode = DeliveryMode.ClassroomBased,
            StartDate = new StartDate(new DateTime(2025, 10, 1)),
            Duration = TimeSpan.FromDays(28),
            Hours = CourseHours.PartTime,
        };
    }

    private CourseDetailsApprenticeship GetApprenticeshipDetails(SearchResult courseLite)
    {
        return new CourseDetailsApprenticeship
        {
            Type = CourseType.Apprenticeship,
            Level = 3,
            EntryRequirements = "Entry requirements " + courseLite.Requirements,
            Wage = 1000,
            PositionAvailable = "Dogsbody",
            Description = "Description " + courseLite.Overview,
            EmployerName = "Company",
            EmployerAddress = new Location { Address1 = "10 Street", Town = "Town", Postcode = "W12 6LA" },
            EmployerDescription = "A nice company",
            TrainingProvider = "College",
            DeliveryMode = DeliveryMode.WorkBased,
            StartDate = new StartDate(new DateTime(2025, 10, 1)),
            Duration = TimeSpan.FromDays(28),
            Hours = CourseHours.FullTime,
        };
    }

    private CourseDetailsCourse GetCourseDetails(SearchResult courseLite)
    {
        return new CourseDetailsCourse
        {
            Type = CourseType.ALevels,
            Level = 3,
            EntryRequirements = "Entry requirements  " + courseLite.Requirements,
            Cost = 1000,
            Description = "Description " + courseLite.Overview,
            ProviderName = "College",
            ProviderAddresses = [new Location { Address1 = "10 Street", Town = "Town", Postcode = "W12 6LA" }],
            DeliveryMode = DeliveryMode.ClassroomBased,
            StartDates = [new StartDate(new DateTime(2025, 10, 1))],
            Duration = TimeSpan.FromDays(28),
            Hours = CourseHours.FullTime,
        };
    }
}