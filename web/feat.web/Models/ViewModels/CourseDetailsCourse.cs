using System.Globalization;

namespace feat.web.Models.ViewModels;

public class CourseDetailsCourse : CourseDetailsBase
{
    public decimal? Cost { private get; set; }
    
    public string CostDisplay => Cost?.ToString("C", new CultureInfo("en-GB")) ?? NotAvailableString;
    
    public string? ProviderName { get; set; }
    
    public List<Location>? ProviderAddresses { get; set; }
    
    public string? ProviderUrl { get; set; }
    
    public List<StartDate>? StartDates { get; set; }
}