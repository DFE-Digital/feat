using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace feat.web.Enums;

public enum AgeGroup
{
    [Display(Name = $"Younger than 18")]
    UnderEighteen,
    [Display(Name = $"Between 18 and 24 years old")]
    EighteenToTwentyFour,
    [Display(Name = $"Older than 24")]
    OverTwentyFour
}