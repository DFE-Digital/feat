using System.ComponentModel;

namespace feat.ingestion.Models.FAC.Enums;

public enum DurationUnit
{
    [Description("Days")]
    Days = 1,

    [Description("Weeks")]
    Weeks = 2,

    [Description("Months")]
    Months = 3,

    [Description("Years")]
    Years = 4,

    [Description("Hours")]
    Hours = 5,

    [Description("Minutes")]
    Minutes = 6,
}
