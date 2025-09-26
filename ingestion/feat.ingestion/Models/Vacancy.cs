using System.ComponentModel.DataAnnotations.Schema;
namespace feat.ingestion.Models;

[Table("Vacancy")]
public class Vacancy
{
    public Guid Id { get; set; }

    public Guid EntryId { get; set; }

    public Guid EmployerId { get; set; }

    public short? Positions { get; set; }

    public decimal? Wage { get; set; }

    public string? WageUnit { get; set; }

    public byte? HoursPerWeek { get; set; }

    public DateTime? ClosingDate { get; set; }

    public Employer Employer { get; set; } = null!;

    public Entry Entry { get; set; } = null!;
}

    //[ForeignKey("EmployerId")]
    //[ForeignKey("EntryId")]
