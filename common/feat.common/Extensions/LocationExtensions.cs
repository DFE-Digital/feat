using feat.common.Models;
using Microsoft.Spatial;
using NetTopologySuite.Geometries;

namespace feat.common.Extensions;

public static class LocationExtensions
{
    public static Point? ToPoint(this PostcodeLatLong postcode)
    {
        return postcode is { Latitude: not null, Longitude: not null } ? 
            new Point(postcode.Longitude.Value, postcode.Latitude.Value) { SRID = 4326 } : null;
    }

    public static Point? ToPoint(this GeographyPoint? geographyPoint)
    {
        return geographyPoint == null ? 
            null : 
            new Point(geographyPoint.Longitude, geographyPoint.Latitude) { SRID = 4326 };
    }

    public static GeographyPoint? ToGeographyPoint(this Point? point)
    {
        return point == null ? null : GeographyPoint.Create(point.Y, point.X);
    }
    
    public static GeographyPoint? ToGeographyPoint(this PostcodeLatLong postcode)
    {
        return postcode is { Latitude: not null, Longitude: not null } ?  
            GeographyPoint.Create(postcode.Longitude.Value, postcode.Latitude.Value) : null;
    }
}