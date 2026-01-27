using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using feat.common.Models.Enums;
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
    
    public Point? GeoLocation { get; set; }

    [StringLength(10)]
    public string? Postcode { get; set; }

    [StringLength(200)]
    public string? Telephone { get; set; }

    [StringLength(100)]
    public string? Town { get; set; }

    [StringLength(2083)]
    public string? Url { get; set; }
    
    public SourceSystem? SourceSystem { get; set; }
    
    [StringLength(200)]
    public string SourceReference { get; set; }

    [InverseProperty("Location")]
    public ICollection<EmployerLocation> EmployerLocations { get; set; } = new List<EmployerLocation>();

    [InverseProperty("Location")]
    public ICollection<EntryInstance> EntryInstances { get; set; } = new List<EntryInstance>();

    [InverseProperty("Location")]
    public ICollection<ProviderLocation> ProviderLocations { get; set; } = new List<ProviderLocation>();
}
