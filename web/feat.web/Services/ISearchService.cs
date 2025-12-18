using feat.web.Models;
using feat.web.Models.ViewModels;

namespace feat.web.Services;

public interface ISearchService
{
    Task<SearchResponse> Search(Search search, string sessionId);

    Task<CourseDetailsBase?> GetCourseDetails(string courseId);

    Task<AutoCompleteLocation[]> GetAutoCompleteLocations(string query, CancellationToken cancellationToken);

    Task<bool> IsLocationValid(string query, CancellationToken cancellationToken);
}