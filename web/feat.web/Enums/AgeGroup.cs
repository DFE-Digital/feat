using System.ComponentModel.DataAnnotations;

namespace feat.web.Enums;

public enum AgeGroup
{
    [Display(Name = $"16 or 17")]
    UnderEighteen,
    
    [Display(Name = $"18")]
    Eighteen,
    
    [Display(Name = $"19")]
    Nineteen,
    
    [Display(Name = $"20 - 24")]
    TwentyToTwentyFour,
    
    [Display(Name = $"25 or older")]
    TwentyFiveOrOver
}