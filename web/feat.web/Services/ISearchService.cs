using feat.web.Models;
using Microsoft.AspNetCore.Mvc;

namespace feat.web.Services;

public interface ISearchService
{
    Task<SearchResponse> Search(Search search, string sessionId);
    
    Task<SearchResponse> GetGlobalFacets();

    Task<SearchResponse> GetFilteredSortedCourses(string sortByFiter); // Temporary awating appropriate implementation
    
    Task<SearchResponse> GetCourseDetails(string courseId); // Temporary awating appropriate implementation
    
}