using CsvHelper.Configuration;
using feat.common.Models;

namespace feat.ingestion.Models.FAC;

public sealed class PostcodeLatLongMap : ClassMap<PostcodeLatLong>
{
    public PostcodeLatLongMap()
    {
        // Course Info
        Map(m => m.Postcode).Name("postcode");
        Map(m => m.Latitude).Name("latitude");
        Map(m => m.Longitude).Name("longitude");
    }
}