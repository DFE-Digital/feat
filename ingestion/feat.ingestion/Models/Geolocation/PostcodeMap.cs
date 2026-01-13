using CsvHelper.Configuration;
using feat.common.Models;
using feat.ingestion.Models.Geolocation.Converters;

namespace feat.ingestion.Models.Geolocation;

public sealed class PostcodeMap : ClassMap<Postcode>
{
    public PostcodeMap()
    {
        Map(m => m.Code).Name("pcds");
        Map(m => m.Latitude).Name("lat");
        Map(m => m.Longitude).Name("long");
        Map(m => m.Expired).Name("doterm").TypeConverter<YYYYMM>();
        Map(m => m.CountryCode).Name("ctry25cd");
    }
}