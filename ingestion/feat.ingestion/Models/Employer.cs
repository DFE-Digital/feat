using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace feat.ingestion.Models;

[Table("Employer")]
public class Employer 
{
    [Key]
    public Guid Id { get; set; } 

    [Column(TypeName = "datetime")]
    public required DateTime Created { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Updated { get; set; } 

    [StringLength(255)]
    public required string Name { get; set; } = null!;

    [InverseProperty("Employer")] 
    public ICollection<EmployerLocation> EmployerLocations { get; set; } = new List<EmployerLocation>();

    [InverseProperty("Employer")]
    public ICollection<Vacancy> Vacancies { get; set; } = new List<Vacancy>();
    
}