using System.ComponentModel;
using System.Text.Json.Serialization;
using feat.common.Extensions.Attributes;

namespace feat.common.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CourseType
{
    [Order(0)]
    [Description("Essential Skills")]
    EssentialSkills = 1,
    
    [Order(1)]
    [Description("T Level")]
    TLevels = 2,
    
    [Order(2)]
    [Description("HTQ")]
    HTQs = 3,
    
    [Order(3)]
    [Description("Free Courses For Jobs")]
    FreeCoursesForJobs = 4,
    
    [Order(4)]
    [Description("Multiply")]
    Multiply = 5,
    
    [Order(5)]
    [Description("Skills Bootcamp")]
    SkillsBootcamp = 6,
    
    [Order(6)]
    [Description("A Level")]
    ALevels = 7,
    
    [Order(7)]
    [Description("Apprenticeship")]
    Apprenticeship = 8,
    
    [Order(8)]
    [Description("Degree")]
    Degree = 9,
    
    [Order(9)]
    [Description("Diploma")]
    Diploma = 10,
    
    [Order(10)]
    [Description("GCSE")]
    GCSE = 101,
    
    [Order(-1)]
    [Description("Unknown")]
    Unknown = -1
}