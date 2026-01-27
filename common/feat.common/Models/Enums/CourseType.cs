using System.ComponentModel;
using System.Text.Json.Serialization;
using feat.common.Extensions.Attributes;

namespace feat.common.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CourseType
{
    [Order(4)]
    [Description("Essential Skills")]
    EssentialSkills = 1,
    
    [Order(10)]
    [Description("T Level")]
    TLevels = 2,
    
    [Order(7)]
    [Description("HTQ")]
    HTQs = 3,
    
    [Order(5)]
    [Description("Free Courses For Jobs")]
    FreeCoursesForJobs = 4,
    
    [Order(8)]
    [Description("Multiply")]
    Multiply = 5,
    
    [Order(9)]
    [Description("Skills Bootcamp")]
    SkillsBootcamp = 6,
    
    [Order(0)]
    [Description("A Level")]
    ALevels = 7,
    
    [Order(1)]
    [Description("Apprenticeship")]
    Apprenticeship = 8,
    
    [Order(2)]
    [Description("Degree")]
    Degree = 9,
    
    [Order(3)]
    [Description("Diploma")]
    Diploma = 10,
    
    [Order(6)]
    [Description("GCSE")]
    GCSE = 101,
    
    [Order(-1)]
    [Description("Unknown")]
    Unknown = -1
}