using System.ComponentModel;

namespace feat.ingestion.Models.FAC.Enums;

public enum CourseOfferingType
{
    [Description("Undefined")]
    Undefined = 0,

    [Description("Course")]
    Course = 1,

    [Description("TLevel")]
    TLevel = 2,
}