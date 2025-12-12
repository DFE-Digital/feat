using feat.common;
using feat.common.Models.Enums;
using feat.web.Configuration;
using feat.web.Extensions;
using feat.web.Models;
using feat.web.Models.ViewModels;
using Microsoft.Extensions.Options;

namespace feat.web.Services;

public class SearchService(
    IApiClient apiClient,
    IOptions<SearchOptions> options) : ISearchService
{
    public async Task<SearchResponse> Search(Search search, string sessionId)
    {
        var request = search.ToSearchRequest();
        request.SessionId = sessionId;
    
        var endpoint = new Uri(new Uri(options.Value.ApiBaseUrl), "api/search").ToString();
    
        return await apiClient.PostAsync<SearchResponse>(ApiClientNames.Feat, endpoint, request);
    }
    
    public async Task<CourseDetailsBase?> GetCourseDetails(string instanceId)
    {
        var endpoint = new Uri(new Uri(options.Value.ApiBaseUrl), $"api/courses/{instanceId}").ToString();
        var response = await apiClient.GetAsync<CourseDetailsResponse>(ApiClientNames.Feat, endpoint);

        return response?.EntryType switch
        {
            EntryType.Course => response.ToCourseViewModel(),
            EntryType.Apprenticeship => response.ToApprenticeshipViewModel(),
            EntryType.UniversityCourse => response.ToUniversityViewModel(),
            _ => null
        };
    }
}