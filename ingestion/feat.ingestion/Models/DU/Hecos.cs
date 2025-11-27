using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;
using feat.ingestion.Models.DU.Enums;

namespace feat.ingestion.Models.DU;

[Table("DU_HECOS")]
public class Hecos
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required int Code { get; set; }
    
    [StringLength(100)]
    public required string Label { get; set; }
}

public sealed class HecosMap : ClassMap<Hecos>
{
    public HecosMap()
    {
        // Course Info
        Map(m => m.Code).Name("Code");
        Map(m => m.Label).Name("Label");
    }
}