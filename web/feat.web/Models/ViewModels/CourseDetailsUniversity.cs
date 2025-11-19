using System.Globalization;

namespace feat.web.Models.ViewModels;

public class CourseDetailsUniversity : CourseDetailsBase
{
    public decimal? TuitionFee { private get; set; }
    
    public string TuitionFeeDisplay => TuitionFee?.ToString("C", new CultureInfo("en-GB")) ?? NotAvailableString;
    
    public string? AwardingOrganisation { get; set; }
    
    public string? University { get; set; }
    
    public string? UniversityUrl { get; set; }
    
    public string? CampusName { get; set; }
    
    public Location? CampusAddress { get; set; }
    
    public StartDate? StartDate { get; set; }
}
