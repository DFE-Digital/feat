using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using feat.common.Models.Enums;

namespace feat.common.Models;

[Table("ProviderLocation")]
public class ProviderLocation
{
    public Guid ProviderId { get; set; }

    public Guid LocationId { get; set; }

    public SourceSystem? SourceSystem { get; set; }
    
    [ForeignKey("LocationId")]
    [InverseProperty("ProviderLocations")]
    public Location Location { get; set; } = null!;

    [ForeignKey("ProviderId")]
    [InverseProperty("ProviderLocations")]
    public Provider Provider { get; set; } = null!;
}
