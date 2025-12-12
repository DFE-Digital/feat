using CsvHelper.Configuration;
using feat.common.Models;
using feat.ingestion.Models.Geolocation.Converters;

namespace feat.ingestion.Models.Geolocation;

public sealed class PostcodeLatLongMap : ClassMap<PostcodeLatLong>
{
    public PostcodeLatLongMap()
    {
        Map(m => m.Postcode).Name("pcds");
        Map(m => m.Latitude).Name("lat");
        Map(m => m.Longitude).Name("long");
        Map(m => m.Expired).Name("doterm").TypeConverter<YYYYMM>();
    }
}