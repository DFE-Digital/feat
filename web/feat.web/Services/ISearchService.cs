using feat.web.Models;
using feat.web.Models.ViewModels;

namespace feat.web.Services;

public interface ISearchService
{
    Task<SearchResponse> Search(Search search, string sessionId);

    Task<CourseDetailsBase?> GetCourseDetails(string courseId);
}