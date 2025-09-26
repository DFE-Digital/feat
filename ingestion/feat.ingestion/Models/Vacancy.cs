using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace feat.ingestion.Models;

[Table("Vacancy")]
public class Vacancy
{
    [Key] 
    public Guid Id { get; set; }

    public Guid EntryId { get; set; }

    public Guid EmployerId { get; set; }

    public short? Positions { get; set; }

    [Column(TypeName = "money")] public decimal? Wage { get; set; }

    public WageUnit? WageUnit { get; set; }

    public byte? HoursPerWeek { get; set; }

    [Column(TypeName = "datetime")] public DateTime? ClosingDate { get; set; }

    [ForeignKey("EmployerId")]
    [InverseProperty("Vacancies")]
    public Employer Employer { get; set; } = null!;

    [ForeignKey("EntryId")]
    [InverseProperty("Vacancies")]
    public Entry Entry { get; set; } = null!;
}
