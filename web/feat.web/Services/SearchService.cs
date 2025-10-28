using feat.common;
using feat.web.Configuration;
using feat.web.Models;
using Microsoft.Extensions.Options;

namespace feat.web.Services;

public class SearchService : ISearchService
{
    private readonly IApiClient _apiClient;
    private readonly IOptions<SearchOptions> _options;

    public SearchService(
        IApiClient apiClient,
        IOptions<SearchOptions> options)
    {
        _apiClient = apiClient;
        _options = options;
    }

    public async Task<SearchResponse> Search(Search search, string sessionId)
    {
        var request = search.ToSearchRequest();
        request.SessionId = sessionId;
        
        var endpoint = new Uri(new Uri(_options.Value.ApiBaseUrl), "api/search").ToString();
        return await _apiClient.PostAsync<SearchResponse>(ApiClientNames.Feat, endpoint, request);
    }

    public async Task<SearchResponse> GetGlobalFacets()
    {
        var endpoint = new Uri(new Uri(_options.Value.ApiBaseUrl), "api/search/global-facets").ToString();
        return await _apiClient.GetAsync<SearchResponse>(ApiClientNames.Feat, endpoint);
    }
    
    public async Task<SearchResponse> GetFilteredSortedCourses(string sortBy)
    {
        var endpoint = new Uri(new Uri(_options.Value.ApiBaseUrl), "api/search/sort").ToString();
        await Task.Delay(1);
        return await _apiClient.GetAsync<SearchResponse>(ApiClientNames.Feat, endpoint);
    }
    
    public async Task<SearchResponse> GetCourseDetails(string courseId)
    {
        var endpoint = new Uri(new Uri(_options.Value.ApiBaseUrl), "api/search/courseId").ToString();
        await Task.Delay(1);
        return await _apiClient.GetAsync<SearchResponse>(ApiClientNames.Feat, endpoint);
    }
}