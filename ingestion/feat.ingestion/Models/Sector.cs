using System.ComponentModel.DataAnnotations.Schema;
namespace feat.ingestion.Models;

[Table("Sector")]
public class Sector
{
    public Guid Id { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Updated { get; set; }

    public string Name { get; set; } = null!;

    public ICollection<EntrySector> EntrySectors { get; set; } = new List<EntrySector>();
}