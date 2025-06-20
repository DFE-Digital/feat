using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace feat.web.Enums;

public enum AgeGroup
{
    [Display(Name = $"Under 18 years old")]
    UnderEighteen,
    [Display(Name = $"Between 18-21 years old")]
    EighteenToTwentyOne,
    [Display(Name = $"Under 24 years old")]
    UnderTwentyFour,
    [Display(Name = $"Over 24 years old")]
    OverTwentyFour,
}