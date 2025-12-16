using DotSpatial.Projections;

namespace feat.ingestion.Models.Geolocation.Converters;

public static class EastingsNorthingsLatLong
{
    // British National Grid (OSGB36) → WGS84 (Lat/Lon)
    public static (double Latitude, double Longitude) ToLatLong(double easting, double northing)
    {
        // Source CRS: EPSG:27700 (OSGB36 / British National Grid)
        var bng = KnownCoordinateSystems.Projected.NationalGrids.BritishNationalGridOSGB36;

        // Target CRS: EPSG:4326 (WGS84)
        var wgs84 = KnownCoordinateSystems.Geographic.World.WGS1984;

        // DotSpatial requires mutable arrays
        double[] xy = { easting, northing };
        double[] z = { 0 };

        Reproject.ReprojectPoints(xy, z, bng, wgs84, 0, 1);

        // Result comes back as lon/lat
        double lon = xy[0];
        double lat = xy[1];

        return (lat, lon);
    }

    // WGS84 (Lat/Lon) → British National Grid (OSGB36)
    public static (double Easting, double Northing) ToNationalGrid(double latitude, double longitude)
    {
        var bng = KnownCoordinateSystems.Projected.NationalGrids.BritishNationalGridOSGB36;
        var wgs84 = KnownCoordinateSystems.Geographic.World.WGS1984;

        double[] xy = { longitude, latitude };  // lon, lat order
        double[] z = { 0 };

        Reproject.ReprojectPoints(xy, z, wgs84, bng, 0, 1);

        return (xy[0], xy[1]);
    }
}