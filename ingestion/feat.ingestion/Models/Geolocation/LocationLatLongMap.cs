using CsvHelper.Configuration;
using feat.common.Models;

namespace feat.ingestion.Models.Geolocation;

public sealed class LocationLatLongMap : ClassMap<LocationLatLong>
{
    public LocationLatLongMap()
    {
        Map(m => m.Name).Name("Name");
        Map(m => m.Latitude).Name("Latitude");
        Map(m => m.Longitude).Name("Longitude");
    }
}