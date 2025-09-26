using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace feat.ingestion.Models;

[Table("Employer")]
public class Employer 
{
    public Guid Id { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Updated { get; set; }

    public string Name { get; set; } = null!;

    public ICollection<EmployerLocation> EmployerLocations { get; set; } = new List<EmployerLocation>();

    public ICollection<Vacancy> Vacancies { get; set; } = new List<Vacancy>();
    
}