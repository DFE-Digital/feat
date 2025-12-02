using Microsoft.Spatial;

namespace feat.api.Models;

public class AiSearchResponse
{
    public required string Id { get; set; }
    
    public required string InstanceId { get; set; }
    
    public required GeographyPoint? Location { get; set; }
}