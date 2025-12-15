using feat.common.Models;

namespace feat.api.Models;

public record AutoCompleteLocation
{
    public required string Name { get; init; }
    
    public required double Latitude { get; init; }
    
    public required double Longitude { get; init; }
}