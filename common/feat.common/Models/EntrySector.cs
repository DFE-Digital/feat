using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace feat.common.Models;

[Table("EntrySector")]
public class EntrySector
{
    [Key]
    public Guid Id { get; set; }

    public Guid EntryId { get; set; }

    public Guid SectorId { get; set; }

    [ForeignKey("EntryId")]
    [InverseProperty("EntrySectors")]
    public Entry Entry { get; set; } = null!;

    [ForeignKey("SectorId")]
    [InverseProperty("EntrySectors")]
    public Sector Sector { get; set; } = null!;
}
