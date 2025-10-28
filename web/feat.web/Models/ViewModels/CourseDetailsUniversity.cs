using System.Globalization;

namespace feat.web.Models.ViewModels;

public class CourseDetailsUniversity : CourseDetailsBase
{
    public decimal? TuitionFee { private get; init; }
    
    public string TuitionFeeDisplay => TuitionFee?.ToString("C", new CultureInfo("en-GB")) ?? NotAvailableString;
    
    public string? AwardingOrganisation { get; init; }
    
    public string? University { get; init; }
    
    public string? CampusName { get; init; }
    
    public Location? CampusAddress { get; init; }
    
    public StartDate? StartDate { get; init; }
}