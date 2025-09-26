using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace feat.ingestion.Models;

[Table("Location")]
public class Location
{
    public Guid Id { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Updated { get; set; }

    public string? Name { get; set; }

    public string? Address1 { get; set; }

    public string? Address2 { get; set; }

    public string? Address3 { get; set; }

    public string? Address4 { get; set; }

    public string? County { get; set; }

    public string? Email { get; set; }
    
    public Point? LocationPoint { get; set; } // Geography Point (Latitude, Longitude)

    public string? Postcode { get; set; }

    public string? Telephone { get; set; }

    public string? Town { get; set; }

    public string? Url { get; set; }

    public ICollection<EmployerLocation> EmployerLocations { get; set; } = new List<EmployerLocation>();

    public ICollection<EntryLocation> EntryLocations { get; set; } = new List<EntryLocation>();

    public ICollection<ProviderLocation> ProviderLocations { get; set; } = new List<ProviderLocation>();
}
