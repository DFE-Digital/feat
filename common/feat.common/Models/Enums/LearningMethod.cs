using System.ComponentModel;
using System.Text.Json.Serialization;

namespace feat.common.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LearningMethod
{
    [Description("Online")]
    Online,
    
    [Description("Classroom based")]
    ClassroomBased,
    
    [Description("Work based")]
    Workbased,
    
    [Description("Hybrid")]
    Hybrid
}