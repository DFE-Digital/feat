using feat.web.Models;
using feat.web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace feat.web.Services;

public interface ISearchService
{
    Task<SearchResponse> Search(Search search, string sessionId);

    Task<CourseDetailsBase?> GetCourseDetails(string courseId);
}