using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;
using feat.common.Extensions;
using feat.ingestion.Models.FAC.Enums;

namespace feat.ingestion.Models.FAC;

[Table("FAC_Venues")]
public class Venue
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Key] 
    public Guid VenueId { get; set; }
    public Guid? ProviderId { get; set; }
    
    public VenueStatus? VenueStatus { get; set; }
    
    public DateTime? CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    
    [StringLength(500)]
    public string? VenueName { get; set; }
    
    public int? ProviderUkprn { get; set; }
    
    [StringLength(250)]
    public string? AddressLine1 { get; set; }
    
    [StringLength(250)]
    public string? AddressLine2 { get; set; }
    
    [StringLength(150)]
    public string? Town { get; set; }
    
    [StringLength(150)]
    public string? County { get; set; }
    
    [StringLength(10)]
    public string? Postcode { get; set; }
    
    [StringLength(50)]
    public string? Telephone { get; set; }
    
    [StringLength(500)]
    public string? Email { get; set; }
    
    [StringLength(500)]
    public string? Website { get; set; }
    

    
}

public sealed class VenueMap : ClassMap<Venue>
{
    public VenueMap()
    {
        Map(m => m.VenueId);
        Map(m => m.ProviderId).Default(new Guid?(), useOnConversionFailure: true);
        Map(m => m.VenueStatus).Default(VenueStatus.Archived, useOnConversionFailure: true);
        Map(m => m.CreatedOn).Default(new DateTime?(), useOnConversionFailure: true);
        Map(m => m.UpdatedOn).Default(new DateTime?(), useOnConversionFailure: true);
        Map(m => m.VenueName).TypeConverter<CleanStringExtension>();
        Map(m => m.ProviderUkprn).Default(new int?(), useOnConversionFailure: true);
        Map(m => m.AddressLine1).TypeConverter<CleanStringExtension>();
        Map(m => m.AddressLine2).TypeConverter<CleanStringExtension>();
        Map(m => m.Town).TypeConverter<CleanStringExtension>();
        Map(m => m.County).TypeConverter<CleanStringExtension>();
        Map(m => m.Postcode).TypeConverter<CleanStringExtension>();
        Map(m => m.Telephone).TypeConverter<CleanStringExtension>();
        Map(m => m.Email).TypeConverter<CleanStringExtension>();
        Map(m => m.Website).TypeConverter<CleanStringExtension>();
    }
}
