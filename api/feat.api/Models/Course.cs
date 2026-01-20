using feat.common.Models.Enums;
using GeographicLib;

namespace feat.api.Models;

public class Course
{
    public required Guid Id { get; set; }
    
    public required Guid InstanceId { get; set; }
        
    public required string Title { get; set; }
        
    public string? Provider { get; set; }
    
    public CourseType? CourseType { get; set; }
    
    public LearningMethod? DeliveryMode { get; set; }
    
    public bool? IsNational { get; set; }
    
    public string? Requirements { get; set; }
    
    public string? Overview { get; set; }
    
    public GeoLocation? Location { get; set; }
    
    public string? LocationName { get; set; }
    
    public double? Distance { get; set; }
    
    public double? Score { get; set; }
    
    public void CalculateDistance(GeoLocation? userLocation)
    {
        Distance = Location != null && userLocation != null
            ? CalculateDistanceInMiles(Location, userLocation)
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