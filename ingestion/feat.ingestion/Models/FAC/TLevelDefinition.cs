using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;
using feat.common.Extensions;
using feat.ingestion.Models.FAC.Enums;

namespace feat.ingestion.Models.FAC;

[Table("FAC_TLevelDefinitions")]
public class TLevelDefinition
{
    [Key]
    public Guid TLevelDefinitionId { get; set; }
    
    [StringLength(500)]
    public string Name { get; set; }
    
    public EducationLevel? QualificationLevel { get; set; }
}

public sealed class TLevelDefinitionMap : ClassMap<TLevelDefinition>
{
    public TLevelDefinitionMap()
    {
        // Course Info
        Map(m => m.TLevelDefinitionId);
        Map(m => m.Name).TypeConverter<CleanStringExtension>();
        Map(m => m.QualificationLevel).Default(EducationLevel.Unknown, useOnConversionFailure: true);
    }
}