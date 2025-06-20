namespace feat.web.Models;

public class FindAResponse
{
    public int Page { get; set; }
    
    public int PageSize { get; set; }
    
    public long? Total { get; set; }

    public List<Course> Courses { get; set; } = [];

    public void CalculateDistances(Geolocation location)
    {
        foreach (var course in Courses)
        {
            course.CalculateDistance(location);
        }
    }
}