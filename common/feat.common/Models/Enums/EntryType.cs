using System.ComponentModel;
using System.Text.Json.Serialization;

namespace feat.common.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EntryType
{
    [Description("Course")]
    Course,
    
    [Description("Apprenticeship")]
    Apprenticeship,
    
    [Description("Degree")]
    UniversityCourse
}