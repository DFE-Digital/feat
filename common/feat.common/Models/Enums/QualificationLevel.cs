using System.ComponentModel;
using System.Text.Json.Serialization;

namespace feat.common.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum QualificationLevel
{
    [Description("Entry level (like entry level functional skills)")]
    Entry = 0,
    
    [Description("Level 1 (like first certificate)")]
    Level1 = 1,
    
    [Description("Level 2 (like GCSEs)")]
    Level2 = 2,
    
    [Description("Level 3 (like A levels)")]
    Level3 = 3,
    
    [Description("Level 4 (like higher national certificate)")]
    Level4 = 4,
    
    [Description("Level 5 (like diplomas)")]
    Level5 = 5,
    
    [Description("Level 6 (like degree)")]
    Level6 = 6,
    
    [Description("Level 7 (like masters degree)")]
    Level7 = 7,
    
    [Description("Level 8 (like a PhD)")]
    Level8 = 8
}