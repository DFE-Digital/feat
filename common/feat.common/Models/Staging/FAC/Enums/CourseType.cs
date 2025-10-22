using System.ComponentModel;

namespace feat.common.Models.Staging.FAC.Enums;

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
    SkillsBootcamp = 6
}