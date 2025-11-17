using GeographicLib;

namespace feat.api.Models;

public class Course
{
    public Guid Id { get; set; }
        
    public required string Title { get; set; }
        
    public required string Provider { get; set; }
    
    public required string CourseType { get; set; }
    
    public required string Requirements { get; set; }
    
    public required string Overview { get; set; }
    
    public GeoLocation? Location { get; set; }
    
    public string? LocationName { get; set; }
    
    public double? Distance { get; set; }
    
    public double? Score { get; set; }
    
    public void SetLocation(GeoLocation? courseLocation, string? locationName, GeoLocation? userLocation)
    {
        Location = courseLocation;
        LocationName = locationName;

        Distance = courseLocation != null
            ? CalculateDistanceInMiles(courseLocation, userLocation)
            : null;
    }

    private static double CalculateDistanceInMiles(GeoLocation point1, GeoLocation point2)
    {
        Geodesic.WGS84.Inverse(point1.Latitude, point1.Longitude, point2.Latitude, point2.Longitude, out var distance);
    
        if (distance > 0)
        {
            return KilometersToMiles(distance / 1000);
        }
    
        return 0;
    }
    
    private static double KilometersToMiles(double value)
    {
        if (value > 0)
        {
            return value / 1.60934;
        }
    
        return 0;
    }
}