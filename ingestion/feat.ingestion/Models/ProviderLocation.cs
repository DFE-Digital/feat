using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace feat.ingestion.Models;

[Table("ProviderLocation")]
public class ProviderLocation
{
    [Key]
    public Guid Id { get; set; }

    public Guid ProviderId { get; set; }

    public Guid LocationId { get; set; }

    [ForeignKey("LocationId")]
    [InverseProperty("ProviderLocations")]
    public Location Location { get; set; } = null!;

    [ForeignKey("ProviderId")]
    [InverseProperty("ProviderLocations")]
    public Provider Provider { get; set; } = null!;
}
