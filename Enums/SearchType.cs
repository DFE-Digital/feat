using System.ComponentModel.DataAnnotations;

namespace feat.web.Enums;

public enum SearchType
{
    [Display(Name = $"Explore options after your GCSEs")]
    FE,
    [Display(Name = $"Find university or other higher education options")]
    HE,
    [Display(Name = $"Return to education or training, after a break or whilst working")]
    Return,
}