using GeographicLib;

namespace feat.api.Models;

public class Course
{
    public Course(
        AiSearchCourse course,
        double? score,
        GeoLocation? location)
    {
        Id = course.id;
        ProviderName = course.PROVIDER_NAME;
        Location = CleanString(course.LOCATION);
        StartDate = course.START_DATE;
        Town = CleanString(course.LOCATION_TOWN);
        Postcode = CleanString(course.POSTCODE);
        CourseType = CleanString(course.COURSE_TYPE);
        Sector = CleanString(course.SECTOR);
        EntryRequirements = CleanString(course.ENTRY_REQUIREMENTS);
        DeliveryMode = CleanString(course.DELIVERY_MODE);
        StudyMode = CleanString(course.STUDY_MODE);
        AttendanceMode = CleanString(course.ATTENDANCE_MODE);
        Duration = CleanString(course.DURATION);
        Cost = CleanString(course.COST);
        Source = CleanString(course.DATA_SOURCE);
        EducationLevel = CleanString(course.EDUCATION_LEVEL);
        ApplicationClosingDate = CleanString(course.APPLICATION_CLOSING_DATE);
        HoursPerWeek = CleanString(course.HOURS_PER_WEEK);
        Wage = CleanString(course.WAGE);
        WageUnit = CleanString(course.WAGE_UNIT);
        SkillsRequired = CleanString(course.SKILLS_REQUIRED);
        Name = CleanString(course.STANDARD_NAME);
        Website = CleanString(course.WEBSITE);
        
        if (course.GEOPOINT_LATLONG != null)
        {
            Latitude = course.GEOPOINT_LATLONG.Longitude;
            Longitude = course.GEOPOINT_LATLONG.Latitude;
        }
        else
        {
            Latitude = null;
            Longitude = null;
        }
        
        PostcodeEmpty = ToBoolean(course.empty_postcode_flag);
        LearningAIMTitle = CleanString(course.LEARNING_AIM_TITLE);
        SSAT1 = CleanString(course.SSAT1);
        SSAT2 = CleanString(course.SSAT2);
        SkillsForLifeDescription = CleanString(course.SKILLS_FOR_LIFE_DESC);
        LearningDirectClassification = CleanString(course.LEARNING_DIRECT_CLASSIFICATION);
        TopicModeling = CleanString(course.TOPIC_MODELING);
        AwardingBody = CleanString(course.AWARDING_BODY);
        QualificationType = CleanString(course.QUALIFICATION_TYPE);
        Level = CleanString(course.LEVEL);
        EmployerName = CleanString(course.EMPLOYER_NAME);
        CourseName = CleanString(course.COURSE_NAME);
        WhoThisCourseIsFor = CleanString(course.WHO_THIS_COURSE_IS_FOR);
        
        if (Latitude.HasValue && Longitude.HasValue && location != null)
        {
            Distance = KilometersToMiles(CalculateDistance(
                new GeoLocation { Latitude = Latitude.Value, Longitude = Longitude.Value },
                location)
            );
        }
        else
        {
            Distance = null;
        }
        
        Score = score;
    }
    
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
    
    private double? Latitude { get; set; }

    private double? Longitude { get; set; }
    
    private static bool ToBoolean(string value)
    {
        return value switch
        {
           "1" or "y" or "Y" or "Yes" or "true" or "True" or "TRUE" => true,
            _ => false
        };
    }

    private static string? CleanString(string? value)
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
    
    private static double CalculateDistance(GeoLocation point1, GeoLocation point2)
    {
        Geodesic.WGS84.Inverse(point1.Latitude, point1.Longitude, point2.Latitude, point2.Longitude, out var distance);
        
        if (distance > 0)
            return distance / 1000;
        
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