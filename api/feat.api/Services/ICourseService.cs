using feat.api.Models;

namespace feat.api.Services;

public interface ICourseService
{
    Task<CourseDetailsResponse?> GetCourseByIdAsync(Guid courseId);
}