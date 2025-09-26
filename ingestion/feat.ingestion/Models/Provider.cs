using System.ComponentModel.DataAnnotations.Schema;
namespace feat.ingestion.Models;

[Table("Provider")]
public class Provider
{
    public Guid Id { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Updated { get; set; }

    public string? Pubukprn { get; set; }

    public string? Ukprn { get; set; }

    public string Name { get; set; } = null!;

    public string? LegalEntityName { get; set; }

    public string? TradingName { get; set; }

    public string? OtherNames { get; set; }

    public ICollection<Entry> Entries { get; set; } = new List<Entry>();

    public ICollection<ProviderLocation> ProviderLocations { get; set; } = new List<ProviderLocation>();
}