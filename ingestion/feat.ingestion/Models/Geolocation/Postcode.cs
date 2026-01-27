namespace feat.ingestion.Models.Geolocation;

public class Postcode
{
    public required string Code { get; set; } 
    
    public double? Latitude { get; set; }
    
    public double? Longitude { get; set; }
    
    public DateTime? Expired { get; set; }
    
    public string? CountryCode { get; set; }
}