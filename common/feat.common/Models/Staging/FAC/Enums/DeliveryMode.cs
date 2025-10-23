using System.ComponentModel;

namespace feat.common.Models.Staging.FAC.Enums;

public enum DeliveryMode
{
    [Description("Classroom based")]
    ClassroomBased = 1,

    [Description("Online")]
    Online = 2,

    [Description("Work based")]
    WorkBased = 3,

    [Description("Blended learning")]
    BlendedLearning = 4,
    
    Unknown = -1
}
