using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;
using feat.ingestion.Models.FAC.Enums;

namespace feat.ingestion.Models.FAC;

[Table("FAC_TLevelLocations")]
public class TLevelLocation
{
    [Key] public Guid TLevelLocationId { get; set; }
    
    public Guid TLevelId { get; set; }
    
    public Guid VenueId { get; set; }
    public Status? TLevelLocationStatus { get; set; }
    
    
}

public sealed class TLevelLocationMap : ClassMap<TLevelLocation>
{
    public TLevelLocationMap()
    {
        // Course Info
        Map(m => m.TLevelLocationId);
        Map(m => m.TLevelId);
        Map(m => m.VenueId);
        Map(m => m.TLevelLocationStatus).Default(Status.Live, useOnConversionFailure: true);
    }
}