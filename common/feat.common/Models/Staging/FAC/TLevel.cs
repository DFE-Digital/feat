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
    public DateTime UpdatedOn { get; set; }
    public DateTime DeletedOn { get; set; }
    [StringLength(2000)] public string WhoFor { get; set; }
    [StringLength(500)] public string? EntryRequirements { get; set; }
    [StringLength(500)] public string? WhatYoullLearn { get; set; }
    [StringLength(500)] public string? HowYoullLearn { get; set; }
    [StringLength(500)] public string? HowYoullBeAssessed { get; set; }
    [StringLength(500)] public string? WhatYouCanDoNext { get; set; }
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
        Map(m => m.UpdatedOn);
        Map(m => m.DeletedOn);
        Map(m => m.WhoFor);
        Map(m => m.EntryRequirements);
        Map(m => m.WhatYoullLearn);
        Map(m => m.HowYoullLearn);
        Map(m => m.HowYoullBeAssessed);
        Map(m => m.WhatYouCanDoNext);
        Map(m => m.Website);
        Map(m => m.StartDate);
    }
}