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

    public async Task<FindAResponse> Search(Search search, string sessionId)
    {
        var request = search.ToFindARequest();
        request.SessionId = sessionId;
        
        var url = _options.Value.ApiBaseUrl;
        return await _apiClient.PostAsync<FindAResponse>(ApiClientNames.Feat, url, request);
    }
}