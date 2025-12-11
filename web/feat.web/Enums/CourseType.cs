using System.ComponentModel.DataAnnotations;

namespace feat.web.Enums;

[Flags]
public enum CourseType
{
    [Display(Name = "I prefer academic courses (like A Levels)")]
    Academic = 1 << 0,
    [Display(Name = "I prefer practical courses (like T Levels, BTECs, or apprenticeships)")]
    Practical = 2 << 1,
    [Display(Name = "I'd like to see both")]
    Both = Academic | Practical
}