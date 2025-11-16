using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace feat.common.Models;

[Table("EmployerLocation")]
public class EmployerLocation
{
    public Guid EmployerId { get; set; }

    [ForeignKey("EmployerId")]
    [InverseProperty("EmployerLocations")]
    public Employer Employer { get; set; } = null!;
    
    public Guid LocationId { get; set; }
    
    [ForeignKey("LocationId")]
    [InverseProperty("EmployerLocations")]
    public Location Location { get; set; } = null!;
} 