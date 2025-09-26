using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace feat.ingestion.Models;

[Table("EntryLocation")]
public class EntryLocation
{
    [Key]
    public Guid Id { get; set; }

    public Guid EntryId { get; set; }

    public Guid LocationId { get; set; }

    [ForeignKey("EntryId")]
    [InverseProperty("EntryLocations")]
    public Entry Entry { get; set; } = null!;

    [ForeignKey("LocationId")]
    [InverseProperty("EntryLocations")]
    public Location Location { get; set; } = null!;
}
