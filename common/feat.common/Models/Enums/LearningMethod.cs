using System.ComponentModel;

namespace feat.common.Models.Enums;

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