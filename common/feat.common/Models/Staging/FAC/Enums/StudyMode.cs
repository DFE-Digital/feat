using System.ComponentModel;

namespace feat.common.Models.Staging.FAC.Enums;

public enum StudyMode
{
    [Description("Full-time")]
    FullTime = 1,

    [Description("Part-time")]
    PartTime = 2,

    [Description("Flexible")]
    Flexible = 3,
}
