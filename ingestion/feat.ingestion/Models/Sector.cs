using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace feat.ingestion.Models;

[Table("Sector")]
public class Sector
{
    [Key]
    public Guid Id { get; set; } 

    public required DateTime Created { get; set; }

    public DateTime? Updated { get; set; } 

    [StringLength(255)]
    public required string Name { get; set; } = null!;

    public ICollection<EntrySector> EntrySectors { get; set; } = new List<EntrySector>();
}