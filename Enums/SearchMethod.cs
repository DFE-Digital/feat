using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace feat.web.Enums;

public enum SearchMethod
{
    [Display(Name = "By a course name, subject, job, or career")]
    ByName,
    [Display(Name = $"See all options in my location")]
    Nearby,
    [Display(Name = "Answer a couple of questions to get more personalised results")]
    Guided
}