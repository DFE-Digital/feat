
using System.Text.Json;
using System.Text.Json.Serialization;

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
    
    public required string Id { get; set; }
    
    public object? DebugInfo { get; set; }

    public string ToJSON()
    {
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Converters = { new JsonStringEnumConverter() }
        });

        return json;
    }
}