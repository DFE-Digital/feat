using System.ComponentModel;

namespace feat.common.Models.Staging.FAC.Enums;

public enum AttendancePattern
{
    [Description("Undefined")]
    Undefined = 0,

    [Description("Daytime")]
    Daytime = 1,

    [Description("Evening")]
    Evening = 2,

    [Description("Weekend")]
    Weekend = 3,

    [Description("Day/Block Release")]
    DayOrBlockRelease = 4,
}