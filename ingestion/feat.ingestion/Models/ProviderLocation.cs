using System.ComponentModel.DataAnnotations.Schema;
namespace feat.ingestion.Models;

[Table("ProviderLocation")]
public class ProviderLocation
{
    public Guid Id { get; set; }

    public Guid ProviderId { get; set; }

    public Guid LocationId { get; set; }

    public Location Location { get; set; } = null!;

    public Provider Provider { get; set; } = null!;
}
    //[ForeignKey("LocationId")]
    //[ForeignKey("ProviderId")]
