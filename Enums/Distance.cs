using System.ComponentModel.DataAnnotations;

namespace feat.web.Enums;

public enum Distance
{
    [Display(Name = $"Up to 1 mile")]
    One = 1,
    [Display(Name = $"Up to 2 miles")]
    Two = 2,
    [Display(Name = $"Up to 5 miles")]
    Five = 5,
    [Display(Name = $"Up to 10 miles")]
    Ten = 10,
    [Display(Name = $"Up to 15 miles")]
    Fifteen = 15,
    [Display(Name = $"Up to 25 miles")]
    TwentyFive = 25
}