using System.ComponentModel.DataAnnotations.Schema;
namespace feat.ingestion.Models;

[Table("EntryLocation")]
public class EntryLocation
{
    public Guid Id { get; set; }

    public Guid EntryId { get; set; }

    public Guid LocationId { get; set; }

    public Entry Entry { get; set; } = null!;

    public Location Location { get; set; } = null!;
}
// [ForeignKey("EntryId")]
//[ForeignKey("LocationId")]