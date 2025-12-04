namespace feat.web.Models;

public class Location
{
    public string? Name { get; set; }
    
    public string? Address1 { get; set; }
    
    public string? Address2 { get; set; }
    
    public string? Address3 { get; set; }
    
    public string? Address4 { get; set; }
    
    public string? Town { get; set; }
    
    public string? County { get; set; }
    
    public string? Postcode { get; set; }
    
    public GeoLocation? GeoLocation { get; set; }
}