using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace feat.ingestion.Models;

[Table("Provider")]
public class Provider
{
    [Key]
    public Guid Id { get; set; } 

    [Column(TypeName = "datetime")]
    public required DateTime Created { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Updated { get; set; } 

    [Column("PUBUKPRN")]
    public string? Pubukprn { get; set; }

    [Column("UKPRN")]
    public string? Ukprn { get; set; }

    [StringLength(255)]
    public required string Name { get; set; } = null!;

    [StringLength(255)]
    public string? LegalEntityName { get; set; }

    [StringLength(255)]
    public string? TradingName { get; set; }

    [StringLength(255)]
    public string? OtherNames { get; set; }

    [InverseProperty("Provider")]
    public ICollection<Entry> Entries { get; set; } = new List<Entry>();

    [InverseProperty("Provider")]
    public ICollection<ProviderLocation> ProviderLocations { get; set; } = new List<ProviderLocation>();
}