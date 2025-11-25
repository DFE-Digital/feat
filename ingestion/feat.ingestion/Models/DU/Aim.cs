using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;
using feat.ingestion.Models.DU.Enums;

namespace feat.ingestion.Models.DU;

[Table("DU_Aims")]
public class Aim
{
    [Key]
    public required int AimCode { get; set; }
    
    [StringLength(20)]
    public required string Label { get; set; }
}

public sealed class AimMap : ClassMap<Aim>
{
    public AimMap()
    {
        // Course Info
        Map(m => m.AimCode).Name("KISAIMCODE");
        Map(m => m.Label).Name("KISAIMLABEL");
    }
}