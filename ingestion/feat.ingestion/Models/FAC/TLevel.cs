using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;
using feat.common.Extensions;
using feat.ingestion.Models.FAC.Enums;

namespace feat.ingestion.Models.FAC;

[Table("FAC_TLevels")]
public class TLevel
{
    [Key] public Guid TLevelId { get; set; }
    public Guid TLevelDefinitionId { get; set; }
    public Status TLevelStatus { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public DateTime? DeletedOn { get; set; }
    [StringLength(4000)] public string WhoFor { get; set; }
    [StringLength(4000)] public string? EntryRequirements { get; set; }
    [StringLength(4000)] public string? WhatYoullLearn { get; set; }
    [StringLength(4000)] public string? HowYoullLearn { get; set; }
    [StringLength(4000)] public string? HowYoullBeAssessed { get; set; }
    [StringLength(4000)] public string? WhatYouCanDoNext { get; set; }
    [StringLength(255)] public string? Website { get; set; }
    public DateTime? StartDate { get; set; }
    
    
}

public sealed class TLevelMap : ClassMap<TLevel>
{
    public TLevelMap()
    {
        // Course Info
        Map(m => m.TLevelId);
        Map(m => m.TLevelDefinitionId);
        Map(m => m.TLevelStatus);
        Map(m => m.CreatedOn);
        Map(m => m.UpdatedOn).Default(new DateTime?(), useOnConversionFailure: true);
        Map(m => m.DeletedOn).Default(new DateTime?(), useOnConversionFailure: true);
        Map(m => m.WhoFor).TypeConverter<CleanStringExtension>();
        Map(m => m.EntryRequirements).TypeConverter<CleanStringExtension>();
        Map(m => m.WhatYoullLearn).TypeConverter<CleanStringExtension>();
        Map(m => m.HowYoullLearn).TypeConverter<CleanStringExtension>();
        Map(m => m.HowYoullBeAssessed).TypeConverter<CleanStringExtension>();
        Map(m => m.WhatYouCanDoNext).TypeConverter<CleanStringExtension>();
        Map(m => m.Website).TypeConverter<CleanStringExtension>();
        Map(m => m.StartDate).Default(new DateTime?(), useOnConversionFailure: true);
    }
}