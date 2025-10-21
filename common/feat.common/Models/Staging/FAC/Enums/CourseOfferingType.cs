using System.ComponentModel;

namespace feat.common.Models.Staging.FAC.Enums;

public enum CourseOfferingType
{
    [Description("Undefined")]
    Undefined = 0,

    [Description("Course")]
    Course = 1,

    [Description("TLevel")]
    TLevel = 2,
}