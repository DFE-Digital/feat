using feat.common;
using feat.common.Extensions;
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
    
    public double? Distance { get; init; }
    
    public string DistanceDisplay =>
        Distance == null
            ? string.Empty
            : $"{Math.Round(Distance.Value)} {((int)Math.Round(Distance.Value) == 1 ? "mile" : "miles")}";
    
    public CourseType? CourseType { private get; init; }
    
    public string CourseTypeDisplay => CourseType?.GetDescription() ?? SharedStrings.NotProvided;
    
    public DeliveryMode? DeliveryMode { get; init; }
    
    public bool? IsNational { get; init; }
    
    public string? Requirements { get; init; }
    
    public string? Overview { get; init; }
    
    public string? LocationName { private get; init; }
    
    public string LocationDisplay
    {
        get
        {
            if (CourseType == feat.common.Models.Enums.CourseType.Apprenticeship)
            {
                return IsNational == true
                    ? "National"
                    : AppendDistance(LocationName.ValueOrNotProvided());
            }
            
            if (DeliveryMode == feat.common.Models.Enums.DeliveryMode.Online)
            {
                return string.IsNullOrWhiteSpace(LocationName)
                    ? "Online"
                    : AppendDistance(LocationName);
            }
            
            return AppendDistance(LocationName.ValueOrNotProvided());
        }
    }

    private string AppendDistance(string location)
    {
        return Distance == null
            ? location
            : $"{location}, {DistanceDisplay}";
    }
}