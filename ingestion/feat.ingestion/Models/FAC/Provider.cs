using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;
using feat.common.Extensions;
using feat.ingestion.Models.FAC.Enums;

namespace feat.ingestion.Models.FAC;

[Table("FAC_Providers")]
public class Provider
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Key] public Guid ProviderId { get; set; }
    
    public int? Ukprn { get; set; }
    
    public ProviderStatus? ProviderStatus { get; set; }
    
    public ProviderType? ProviderType { get; set; }
    
    [StringLength(500)]
    public string? ProviderName { get; set; }
    
    [StringLength(500)]
    public string? CourseDirectoryName { get; set; }
    
    [StringLength(500)]
    public string? TradingName { get; set; }
    
    [StringLength(500)]
    public string? Alias { get; set; }
    
    public DateTime? UpdatedOn { get; set; }

    
}

public sealed class ProviderMap : ClassMap<Provider>
{
    public ProviderMap()
    {
        Map(m => m.ProviderId);
        Map(m => m.Ukprn).Default(new int?(), useOnConversionFailure: true);
        Map(m => m.ProviderStatus).Default(ProviderStatus.Unregistered, useOnConversionFailure: true);
        Map(m => m.ProviderType).Default(ProviderType.None, useOnConversionFailure: true);
        Map(m => m.ProviderName).TypeConverter<CleanStringExtension>();
        Map(m => m.CourseDirectoryName).TypeConverter<CleanStringExtension>();
        Map(m => m.TradingName).TypeConverter<CleanStringExtension>();
        Map(m => m.Alias).TypeConverter<CleanStringExtension>();
        Map(m => m.UpdatedOn).Default(new DateTime?(), useOnConversionFailure: true);
    }
}
