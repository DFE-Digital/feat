using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using feat.common.Models.Enums;

namespace feat.api.Models;

[Description("A course and apprenticeship search request. Where possible, search for locations using latitude and longitude, followed by postcode, followed by location name")]
public class McpSearchRequest : SearchRequest
{
    [Description("The type of course to search for, eg: degree, apprenticeship, a-level")]
    public new IEnumerable<CourseType>? CourseType
    {
        get
        {
            var value = base.CourseType;
            return value != null ? value.Select(e => Enum.Parse<CourseType>(e.ToString())) : [];
        }
        set { base.CourseType = (value ?? []).Select(e => e.ToString()); }
    }

    [Description("The qualification level to search for")]
    public new IEnumerable<QualificationLevel>? QualificationLevel
    {
        get
        {
            var value = base.QualificationLevel;
            return value != null ? value.Select(e => Enum.Parse<QualificationLevel>(e.ToString())) : [];
        }
        set { base.QualificationLevel = (value ?? []).Select(e => e.ToString()); }
    }

    [Description("How the course is taught")]
    public new IEnumerable<LearningMethod>? LearningMethod
    {
        get
        {
            var value = base.LearningMethod;
            return value != null ? value.Select(e => Enum.Parse<LearningMethod>(e.ToString())) : [];
        }
        set { base.LearningMethod = (value ?? []).Select(e => e.ToString()); }
    }

    [Description("Whether the course is full-time, part-time, or flexible")]
    public new IEnumerable<CourseHours>? CourseHours
    {
        get
        {
            var value = base.CourseHours;
            return value != null ? value.Select(e => Enum.Parse<CourseHours>(e.ToString())) : [];
        }
        set { base.CourseHours = (value ?? []).Select(e => e.ToString()); }
    }

    [Description("When the course is taught")]
    public new IEnumerable<StudyTime>? StudyTime
    {
        get
        {
            var value = base.StudyTime;
            return value != null ? value.Select(e => Enum.Parse<StudyTime>(e.ToString())) : [];
        }
        set { base.StudyTime = (value ?? []).Select(e => e.ToString()); }
    }
    
    [Description("The postcode to search near. When using postcode, ensure the radius is set")]
    [RegularExpression(@"^[a-zA-Z]{1,2}\d[a-zA-Z\d]?\s*\d[a-zA-Z]{2}$")]
    public new string? Location 
    { 
        get => base.Location;
        set => base.Location = value;
    }

}