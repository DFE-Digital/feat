namespace feat.web.Models.ViewModels;

public class Location
{
    public string? Name { private get; init; }
    
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
                Name,
                Address1,
                Address2,
                Address3,
                Address4,
                Town,
                County,
                Postcode?.ToUpperInvariant()
            };

            return string.Join(", ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }
    }
    
    private const string PostcodeUrl = "https://google.co.uk/maps?q=";
    private const string LatlongUrl = "https://www.google.com/maps/search/?api=1&query=";
    
    private string GetPostcodeUrl()
    {
        if (string.IsNullOrWhiteSpace(Postcode))
        {
            return string.Empty;
        }
        return PostcodeUrl + Postcode?.ToUpperInvariant().Replace(" ", string.Empty);
    }
    
    private string GeoLocationUrl()
    {
        if (GeoLocation?.Latitude == null || GeoLocation.Longitude == null)
        {
            return string.Empty;
        }
        return $"{LatlongUrl}{GeoLocation.Latitude},{GeoLocation.Longitude}";
    }
    
    public string MapLocationUrl
    {
        get
        {
            var url = GeoLocationUrl();
            return string.IsNullOrEmpty(url) ? GetPostcodeUrl() : url;
        }
    }
}