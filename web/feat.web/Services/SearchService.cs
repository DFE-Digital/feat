using feat.web.Configuration;
using feat.web.Models;
using feat.web.Repositories;
using Microsoft.Extensions.Options;

namespace feat.web.Services;

public class SearchService(HttpClientRepository httpClientRepository, IOptions<SearchOptions> options) : ISearchService
{
    public async Task<FindAResponse> Search(Search search, string sessionId)
    {
        var request = search.ToFindARequest();
        request.SessionId = sessionId;

        var url = string.IsNullOrEmpty(options.Value.Url) ? "https://find-a-dev-def2ardzbwgvfzfm.westeurope-01.azurewebsites.net/api/Search" : options.Value.Url;
        return await httpClientRepository.PostAsync<FindAResponse>(url, request) ?? new();
    }
}