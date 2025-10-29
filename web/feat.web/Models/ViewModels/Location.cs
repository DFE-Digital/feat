namespace feat.web.Models.ViewModels;

public class Location
{
    public string? Address1 { private get; init; }
    
    public string? Address2 { private get; init; }
    
    public string? Address3 { private get; init; }
    
    public string? Address4 { private get; init; }
    
    public string? Town { get; init; }
    
    public string? County { private get; init; }
    
    public string? Postcode { private get; init; }
    
    public GeoLocation? GeoLocation { get; init; }

    public string Display => FormatAddress;

    private string FormatAddress
    {
        get
        {
            var parts = new[]
            {
                Address1,
                Address2,
                Address3,
                Address4,
                Town,
                County,
                Postcode
            };

            return string.Join(", ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }
    }
}