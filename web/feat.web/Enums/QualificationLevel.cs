using System.ComponentModel.DataAnnotations;

namespace feat.web.Enums;

[Flags]
public enum QualificationLevel
{
    [Display(Name = "Get skills and experience (without a qualification)")]
    None = 1 << 1,

    [Display(Name = "Level 1 or 2 (like GCSEs)")]
    OneAndTwo = 1 << 2,

    [Display(Name = "Level 3 (like BTECs and A levels)")]
    Three = 1 << 3,
    
    [Display(Name = "Level 4 to 8 (like diplomas and degrees)")] 
    FourToEight = 1 << 4
}