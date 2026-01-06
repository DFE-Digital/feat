using feat.common;
using feat.common.Models.Enums;
using feat.web.Extensions;

namespace feat.web.Models.ViewModels;

public class SearchResult
{
    public required Guid Id { get; set; }
    
    public required Guid InstanceId { get; set; }
    
    public required string CourseTitle { get; init; }
    
    public string? ProviderName { get; init; }
    
    public GeoLocation? Location { get; init; }
    
    public string? LocationName { get; init; }
    
    public double? Distance { get; init; }
    
    public string DistanceDisplay =>
        Distance == null
            ? string.Empty
            : $"{Math.Round(Distance.Value)} {((int)Math.Round(Distance.Value) == 1 ? "mile" : "miles")}";
    
    public CourseType? CourseType { private get; init; }
    
    public string CourseTypeDisplay => CourseType?.GetDescription() ?? SharedStrings.NotProvided;
    
    public string? Requirements { get; init; }
    
    public string? Overview { get; init; }
}