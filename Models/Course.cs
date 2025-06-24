
namespace feat.web.Models;

public class Course
{
    public double? Score { get; set; }
    
    public double? Distance { get; set; }
    
    public string? CourseName { get; set; }
    
    public string? WhoThisCourseIsFor { get; set; }

    public string ProviderName { get; set; }

    public string? Location { get; set; }

    public string? Town { get; set; }

    public string? Postcode { get; set; }
    
    public DateTimeOffset? StartDate { get; set; }

    public string? CourseType { get; set; }

    public string? Sector { get; set; }

    public string? EntryRequirements { get; set; }

    public string? DeliveryMode { get; set; }

    public string? StudyMode { get; set; }

    public string? AttendanceMode { get; set; }
    
    public string? Duration { get; set; }

    public string? Cost { get; set; }

    public string? Source { get; set; }

    public string? EducationLevel { get; set; }

    public string? ApplicationClosingDate { get; set; }

    public string? HoursPerWeek { get; set; }

    public string? Wage { get; set; }

    public string? WageUnit { get; set; }

    public string? SkillsRequired { get; set; }


    public string? Website { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public bool PostcodeEmpty { get; set; }
    
    public string? Name { get; set; }

    public string? LearningAIMTitle { get; set; }

    public string? SSAT1 { get; set; }

    public string? SSAT2 { get; set; }

    public string? SkillsForLifeDescription { get; set; }

    public string? LearningDirectClassification { get; set; }

    public string? TopicModeling { get; set; }

    public string? AwardingBody { get; set; }

    public string? QualificationType { get; set; }

    public string? Level { get; set; }

    public string? EmployerName { get; set; }
    
    public string Id { get; set; }

    public void CalculateDistance(Geolocation location)
    {
        if (Latitude.HasValue && Longitude.HasValue)
        {
            Distance = KilometersToMiles(CalculateDistance(
                new Geolocation() { Latitude = Latitude.Value, Longitude = Longitude.Value },
                location)
            );
        }
        else
        {
            Distance = 0;
        }
    }
    
    private bool ToBoolean(string value)
    {
        return value switch
        {
           "1" or "y" or "Y" or "Yes" or "true" or "True" or "TRUE" => true,
            _ => false
        };
    }

    private string? CleanString(string? value)
    {
        
        value = value switch
        {
            "" or null or "na" or "nan" or "unknown" or "Unknown"
                or "D_No Aim Title" => null,
            _ => value
        };
        
        value = value?.Trim();
        value = value?.Replace("_x000D_", Environment.NewLine);

        return value;
    }
    
    private double CalculateDistance(Geolocation point1, Geolocation point2)
    {
        double R = 6371;
        var lat = GetRadians(point2.Latitude - point1.Latitude);
        var lng = GetRadians(point2.Longitude - point1.Longitude);
        var h1 = Math.Sin(lat / 2) * Math.Sin(lat / 2) +
                 Math.Cos(GetRadians(point1.Latitude)) * Math.Cos(GetRadians(point2.Latitude)) *
                 Math.Sin(lng / 2) * Math.Sin(lng / 2);
        var h2 = 2 * Math.Asin(Math.Min(1, Math.Sqrt(h1)));
        return R * h2;
    }

    private double KilometersToMiles(double value)
    {
        if (value > 0)
        {
            return value / 1.60934;
        }

        return 0;
    }

    private static double GetRadians(double value)
    {
        return value * Math.PI / 180;
    }
    
}