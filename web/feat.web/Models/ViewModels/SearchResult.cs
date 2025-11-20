using feat.common.Models.Enums;
using feat.web.Extensions;

namespace feat.web.Models.ViewModels;

public class SearchResult
{
    public required Guid Id { get; set; }
    
    public required string CourseTitle { get; init; }
    
    public string? ProviderName { get; init; }
    
    public string? Location { get; init; }
    
    public double? Distance { get; init; }
    
    public string DistanceDisplay => Distance != null ? $"{Math.Round(Distance.Value)} miles" : string.Empty;
    
    public CourseType? CourseType { private get; init; }
    
    public string CourseTypeDisplay => CourseType?.GetDescription() ?? "Not available";
    
    public string? Requirements { get; init; }
    
    public string? Overview { get; init; }
}