namespace feat.ingestion.Models.Geolocation;

public class OpenName
{
    public required string Name1 { get; set; }
    
    public string? Name1Lang { get; set; }
    
    public string? Name2 { get; set; }
    
    public string? Name2Lang { get; set; }

    public required string Type { get; set; }
    
    public required string LocalType { get; set; }
    
    public double X { get; set; }
    
    public double Y { get; set; }
    
    public string? District { get; set; }
    
    public string? County { get; set; }
    
    public required string Country  { get; set; }
}