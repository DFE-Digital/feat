using System.ComponentModel.DataAnnotations.Schema;
namespace feat.ingestion.Models;


[Table("EmployerLocation")]
public class EmployerLocation
{
    public Guid Id { get; set; }

    public Guid EmployerId { get; set; }

    public Guid LocationId { get; set; }

    public Employer Employer { get; set; } = null!;

    public Location Location { get; set; } = null!;
}
//[ForeignKey("EmployerId")]
//[ForeignKey("LocationId")]