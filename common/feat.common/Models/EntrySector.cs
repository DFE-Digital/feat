using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using feat.common.Models.Enums;

namespace feat.common.Models;

[Table("EntrySector")]
public class EntrySector
{
    public Guid EntryId { get; set; }

    public Guid SectorId { get; set; }

    public SourceSystem? SourceSystem { get; set; }
    
    [ForeignKey("EntryId")]
    [InverseProperty("EntrySectors")]
    public Entry Entry { get; set; } = null!;

    [ForeignKey("SectorId")]
    [InverseProperty("EntrySectors")]
    public Sector Sector { get; set; } = null!;
}
