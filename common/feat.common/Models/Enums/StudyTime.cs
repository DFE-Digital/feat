using System.ComponentModel;

namespace feat.common.Models.Enums;

public enum StudyTime
{
    [Description("Daytime")]
    Daytime,
    
    [Description("Evening")]
    Evening,
    
    [Description("Weekend")]
    Weekend
}