using feat.web.Models;
using Microsoft.AspNetCore.Mvc;

namespace feat.web.Services;

public interface ISearchService
{
    Task<SearchResponse> Search(Search search, string sessionId);
}