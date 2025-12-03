using System.Globalization;

namespace feat.web.Models.ViewModels;

public class CourseDetailsUniversity : CourseDetailsBase
{
    public decimal? TuitionFee { private get; set; }
    
    public string TuitionFeeDisplay => TuitionFee?.ToString("C", new CultureInfo("en-GB")) ?? NotProvidedString;
    
    public string? AwardingOrganisation { get; set; }
    
    public string? University { get; set; }
    
    public string? UniversityUrl { get; set; }
    
    public string? CampusName { get; set; }
    
    public List<Location>? CampusAddresses { get; set; }
    
    public List<StartDate>? StartDates { get; set; }
}
