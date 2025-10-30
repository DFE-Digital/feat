using System.Globalization;

namespace feat.web.Models.ViewModels;

public class CourseDetailsCourse : CourseDetailsBase
{
    public decimal? Cost { private get; init; }
    
    public string CostDisplay => Cost?.ToString("C", new CultureInfo("en-GB")) ?? NotAvailableString;
    
    public string? ProviderName { get; init; }
    
    public List<Location>? ProviderAddresses { get; init; }
    
    public string? ProviderUrl { get; init; }
    
    public List<StartDate>? StartDates { get; init; }
}