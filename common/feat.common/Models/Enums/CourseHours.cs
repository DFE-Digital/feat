using System.ComponentModel;
using System.Text.Json.Serialization;

namespace feat.common.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CourseHours
{
    [Description("Full time")]
    FullTime,
    
    [Description("Part time")]
    PartTime,
    
    [Description("Flexible")]
    Flexible 
}