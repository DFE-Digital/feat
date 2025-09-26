using System.ComponentModel.DataAnnotations.Schema;
namespace feat.ingestion.Models;

[Table("EntrySector")]
public class EntrySector
{
    public Guid Id { get; set; }

    public Guid EntryId { get; set; }

    public Guid SectorId { get; set; }

    public Entry Entry { get; set; } = null!;

    public Sector Sector { get; set; } = null!;
}
    //[ForeignKey("EntryId")]
    //[ForeignKey("SectorId")]
