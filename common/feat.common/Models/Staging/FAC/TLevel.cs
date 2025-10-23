using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using CsvHelper.Configuration;
using feat.common.Models.Staging.FAC.Enums;

namespace feat.common.Models.Staging.FAC;

[Table("FAC_TLevels")]
public class TLevel
{
    [Key] public Guid TLevelId { get; set; }
    public Guid TLevelDefinitionId { get; set; }
    public CourseStatus TLevelStatus { get; set; }
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
        Map(m => m.WhoFor).TypeConverter<CleanString>();
        Map(m => m.EntryRequirements).TypeConverter<CleanString>();
        Map(m => m.WhatYoullLearn).TypeConverter<CleanString>();
        Map(m => m.HowYoullLearn).TypeConverter<CleanString>();
        Map(m => m.HowYoullBeAssessed).TypeConverter<CleanString>();
        Map(m => m.WhatYouCanDoNext).TypeConverter<CleanString>();
        Map(m => m.Website).TypeConverter<CleanString>();
        Map(m => m.StartDate).Default(new DateTime?(), useOnConversionFailure: true);
    }
}