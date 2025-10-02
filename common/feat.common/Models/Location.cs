using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace feat.common.Models;

[Table("Location")]
public class Location
{
    [Key]
    public Guid Id { get; set; } 

    public required DateTime Created { get; set; }

    public DateTime? Updated { get; set; } 

    [StringLength(255)]
    public string? Name { get; set; }

    [StringLength(255)]
    public string? Address1 { get; set; }

    [StringLength(255)]
    public string? Address2 { get; set; }

    [StringLength(255)]
    public string? Address3 { get; set; }

    [StringLength(255)]
    public string? Address4 { get; set; }

    [StringLength(60)]
    public string? County { get; set; }

    [StringLength(320)]
    public string? Email { get; set; }
    
    public Point? GeoLocation { get; set; }       // Geography Point (Latitude, Longitude)

    [StringLength(10)]
    public string? Postcode { get; set; }

    [StringLength(20)]
    public string? Telephone { get; set; }

    [StringLength(100)]
    public string? Town { get; set; }

    [StringLength(2083)] // Maximum length for a URL
    public string? Url { get; set; }

    [InverseProperty("Location")]
    public ICollection<EmployerLocation> EmployerLocations { get; set; } = new List<EmployerLocation>();

    [InverseProperty("Location")]
    public ICollection<EntryLocation> EntryLocations { get; set; } = new List<EntryLocation>();

    [InverseProperty("Location")]
    public ICollection<ProviderLocation> ProviderLocations { get; set; } = new List<ProviderLocation>();
}
