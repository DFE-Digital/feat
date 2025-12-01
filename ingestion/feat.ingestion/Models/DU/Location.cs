using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;
using feat.ingestion.Models.DU.Enums;

namespace feat.ingestion.Models.DU;

[Table("DU_Locations")]
public class Location
{
    public required int UKPRN { get; set; }
    
    [StringLength(200)]
    public required string LocationId { get; set; }

    [StringLength(300)]
    public required string Name { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }
    
    public RegionCode Country { get; set; }
    
    [StringLength(1000)]
    public string? AccommodationUrl { get; set; }
    
    [StringLength(1000)]
    public string? StudentUnionUrl { get; set; }

}

public sealed class LocationMap : ClassMap<Location>
{
    public LocationMap()
    {
        // Course Info
        Map(m => m.UKPRN);
        Map(m => m.LocationId).Name("LOCID");
        Map(m => m.Name).Name("LOCNAME");
        Map(m => m.Latitude).Name("LATITUDE");
        Map(m => m.Longitude).Name("LONGITUDE");
        Map(m => m.Country).Name("LOCCOUNTRY");
        Map(m => m.AccommodationUrl).Name("ACCOMURL");
        Map(m => m.StudentUnionUrl).Name("SUURL");
    }
}