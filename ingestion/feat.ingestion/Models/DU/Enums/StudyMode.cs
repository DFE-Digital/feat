using System.ComponentModel;

namespace feat.ingestion.Models.DU.Enums;

public enum StudyMode
{
    [Description("Not Set")]
    Unknown = 0,
    [Description("Full-time")]
    FullTime = 1,
    [Description("Part-time")]
    PartTime = 2,
    [Description("Both")]
    Both = FullTime | PartTime
}