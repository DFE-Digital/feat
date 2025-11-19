using System.Text.RegularExpressions;
using feat.common.Models;
using feat.common.Models.Enums;
using feat.ingestion.Models.FAC.Enums;
using Microsoft.Spatial;
using NetTopologySuite.Geometries;
using DeliveryMode = feat.common.Models.Enums.DeliveryMode;

namespace feat.ingestion.Handlers.FAC;

public static class FacMappingExtensions
{
    public static CourseHours? ToCourseHours(this StudyMode? source)
    {
        return source switch
        {
            StudyMode.Flexible => CourseHours.Flexible,
            StudyMode.FullTime => CourseHours.FullTime,
            StudyMode.PartTime => CourseHours.PartTime,
            _ => null
        };
    }

    public static StudyTime? ToStudyTime(this AttendancePattern? source)
    {
        return source switch
        {
            AttendancePattern.Daytime => StudyTime.Daytime,
            AttendancePattern.DayOrBlockRelease => StudyTime.Daytime,
            AttendancePattern.Evening => StudyTime.Evening,
            AttendancePattern.Weekend => StudyTime.Weekend,
            _ => null
        };
    }
    
    public static LearningMethod? ToLearningMethod(this DeliveryMode? source)
    {
        return source switch
        {
            DeliveryMode.BlendedLearning => LearningMethod.Hybrid,
            DeliveryMode.ClassroomBased => LearningMethod.ClassroomBased,
            DeliveryMode.Online => LearningMethod.Online,
            DeliveryMode.WorkBased => LearningMethod.Workbased,
            _ => null
        };
    }

    public static QualificationLevel? ToQualificationLevel(this EducationLevel? source)
    {
        return source switch
        {
            EducationLevel.E or EducationLevel.Level0 => QualificationLevel.Entry,
            EducationLevel.Level1 => QualificationLevel.Level1,
            EducationLevel.Level2 => QualificationLevel.Level2,
            EducationLevel.Level3 => QualificationLevel.Level3,
            EducationLevel.Level4 => QualificationLevel.Level4,
            EducationLevel.Level5 => QualificationLevel.Level5,
            EducationLevel.Level6 => QualificationLevel.Level6,
            EducationLevel.Level7 => QualificationLevel.Level7,
            _ => null
        };
    }

    public static string Scrub(this string? source)
    {
        if (source == null)
            return string.Empty;

        var result = source.Trim() switch
        {
            "*" or "***" or "*****" => string.Empty,
            "-" or "?" or "1" or "a" or "n" or "x" or "z" => string.Empty,
            "xx" or "xxx" => string.Empty,
            "n/a" or "TBA" or "TBC" or "PENDING" or "To follow" or "To be added" =>  string.Empty, 
            "See website" or "This course" => string.Empty,
            _ => CheckForWebsite(source.Trim())
        };
        
        return result;
    }

    private static string CheckForWebsite(string source)
    {
        var regex = new Regex(
            @"https\:\/\/|web\s*site.*details|see (course |school |provider )*web\s*site|web\s*site.*information|web\s*site for (more|the|entry requirements|course booket)|please see web|refer to our web|(our|course|provider|school|college|ocr|edexcel|aqa|Eduqas|examination board)+ web\s*site|www\.|of the website",
            RegexOptions.IgnoreCase | RegexOptions.Multiline);

        return regex.IsMatch(source) ? string.Empty : source;
    }
}