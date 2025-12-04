using feat.api.Models;

namespace feat.api.Extensions;

public static class ModelExtensions
{
    public static Location ToLocation(this common.Models.Location l)
    {
        return new Location
        {
            Address1 = l.Address1,
            Address2 = l.Address2,
            Address3 = l.Address3,
            Address4 = l.Address4,
            Town = l.Town,
            County = l.County,
            Postcode = l.Postcode,
            GeoLocation = l.GeoLocation != null
                ? new GeoLocation
                {
                    Latitude = l.GeoLocation.Y,
                    Longitude = l.GeoLocation.X
                }
                : null
        };
    }
    
}