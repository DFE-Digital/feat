using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace feat.web.Enums;

[Flags]
public enum CourseLevel
{
    [Display(Name = "GCSE level or equivalent")]
    GCSE = 1 << 0,
    [Display(Name = "A Level, T Level, or equivalent")]
    ALevel = 2 << 1,
    [Display(Name = "I'd like to see both")]
    Both = GCSE | ALevel
}