using feat.web.Models;
using feat.web.Models.ViewModels;

namespace feat.web.Utils;

public static class DtoMapper
{
    public static Course ToCourse(this SearchResult searchResult)
    {
        return new Course()
        {
            CourseId = searchResult.CourseId,
            DistanceSudo = searchResult.DistanceSudo,
            CourseTitle = searchResult.CourseTitle,
            ProviderName = searchResult.ProviderName,
            Location = searchResult.Location,
            Overview = searchResult.Overview,
            CourseType = searchResult.CourseType,
            Distance = searchResult.Distance,
            Requirements = searchResult.Requirements
        };
    }

    public static List<Course> ToCourses(this List<SearchResult> searchResults)
    {
        if (searchResults?.Count > 0)
        {
            IList<Course> courses = new List<Course>();
            foreach (var searchResult in searchResults)
            {
                courses.Add(searchResult.ToCourse());
            }

            return courses.ToList();
        }

        return new List<Course>();
    }
}