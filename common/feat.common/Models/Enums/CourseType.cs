using System.ComponentModel;

namespace feat.common.Models.Enums;

public enum CourseType
{
    [Description("Essential Skills")]
    EssentialSkills = 1,
    
    [Description("T Level")]
    TLevels = 2,
    
    [Description("HTQ")]
    HTQs = 3,
    
    [Description("Free Courses For Jobs")]
    FreeCoursesForJobs = 4,
    
    [Description("Multiply")]
    Multiply = 5,
    
    [Description("Skills Bootcamp")]
    SkillsBootcamp = 6,
    
    [Description("A Level")]
    ALevels = 7,
    
    [Description("Apprenticeship")]
    Apprenticeship = 8,
    
    [Description("Degree")]
    Degree = 9,
    
    [Description("Diploma")]
    Diploma = 10,
    
    Unknown = -1
}