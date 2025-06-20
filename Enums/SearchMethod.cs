using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace feat.web.Enums;

public enum SearchMethod
{
    [Display(Name = $"Find all courses nearby")]
    AllCoursesNearby,
    [Display(Name = "Answer a few questions for more specific results")]
    AnswerAFewQuestions
}