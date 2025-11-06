using System.ComponentModel;

namespace feat.ingestion.Models.FAC.Enums;

public enum ApprovedQualificationLevel
{
    [Description("Level 1")]
    Level1,
    [Description("Level 1/2")]
    Level12,
    [Description("Level 2")]
    Level2,
    [Description("Level 3")]
    Level3,
    [Description("Level 4")]
    Level4,
    [Description("Level 5")]
    Level5,
    [Description("Level 6")]
    Level6,
    [Description("Entry Level")]
    EntryLevel,
    Unknown
}