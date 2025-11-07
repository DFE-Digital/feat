using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using feat.common.Models.Enums;
using feat.common.Models.Staging.FAA.Enums;

namespace feat.common.Models;

[Table("Vacancy")]
public class Vacancy
{
    [Key] 
    public Guid Id { get; set; }

    public Guid EntryId { get; set; }

    public Guid EmployerId { get; set; }
    
    public ApprenticeshipLevel? Level { get; set; }

    public short? Positions { get; set; }

    [Column(TypeName = "money")] 
    public decimal? Wage { get; set; }

    public WageUnit? WageUnit { get; set; }
    
    public WageType? WageType { get; set; }

    public decimal? HoursPerWeek { get; set; }
    
    [StringLength(1000)]
    public string? WorkingWeekDescription { get; set; }
    
    public DateTime? ClosingDate { get; set; }
    
    public DateTime? PostedDate { get; set; }

    [StringLength(2083)] 
    public string? Url { get; set; }
    
    public bool? NationalVacancy { get; set; }
    
    [StringLength(1000)]
    public string? NationalVacancyDetails { get; set; }

    [ForeignKey("EmployerId")]
    [InverseProperty("Vacancies")]
    public Employer Employer { get; set; } = null!;

    [ForeignKey("EntryId")]
    [InverseProperty("Vacancies")]
    public Entry Entry { get; set; } = null!;
}
