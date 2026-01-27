using System.Globalization;

namespace feat.web.Models.ViewModels;

public class CourseDetailsCourse : CourseDetailsBase
{
    public decimal? Cost { private get; set; }
    
    public string CostDisplay => Cost?.ToString("C", new CultureInfo("en-GB")) ?? NotProvidedString;
    
    public string? ProviderName { get; set; }
    
    public List<Location>? ProviderAddresses { get; set; }
    
    public List<Location>? CourseAddresses { get; set; }
    
    public string? ProviderUrl { private get; set; }
    
    public string? ProviderUrlAbsolute => NormalizeUrl(ProviderUrl);
    
    public List<StartDate>? StartDates { get; set; }
}