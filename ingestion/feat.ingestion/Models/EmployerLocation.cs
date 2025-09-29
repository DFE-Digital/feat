using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace feat.ingestion.Models;


[Table("EmployerLocation")]
public class EmployerLocation
{
    [Key]
    public Guid Id { get; set; }

    public Guid EmployerId { get; set; }

    [ForeignKey("EmployerId")]
    [InverseProperty("EmployerLocations")]
    public Employer Employer { get; set; } = null!;
    
    public Guid LocationId { get; set; }
    
    [ForeignKey("LocationId")]
    [InverseProperty("EmployerLocations")]
    public Location Location { get; set; } = null!;
} 