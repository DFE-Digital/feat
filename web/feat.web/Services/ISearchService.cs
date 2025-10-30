using feat.web.Models;
using Microsoft.AspNetCore.Mvc;

namespace feat.web.Services;

public interface ISearchService
{
    Task<SearchResponse> Search(Search search, string sessionId);
    
    Task<SearchResponse> GetGlobalFacets();

    // Temporary awating appropriate implementation
    Task<SearchResponse> GetFilteredSortedPagedCourses(Search search, string sessionId, string sortFilter, int pageNumber, int pageSize); 
    
    Task<SearchResponse> GetCourseDetails(string courseId); 
    
}