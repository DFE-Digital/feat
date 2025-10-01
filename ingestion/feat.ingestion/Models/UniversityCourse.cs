using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace feat.ingestion.Models;

[Table("UniversityCourse")]
public class UniversityCourse
{
    [Key]
    public Guid Id { get; set; }

    public required Guid EntryId { get; set; }

    public bool? Foundation { get; set; }

    public bool? Honours { get; set; }

    public bool? Nhs { get; set; }

    public bool? Sandwich { get; set; }

    public bool? YearAbroad { get; set; }

    [ForeignKey("EntryId")]
    [InverseProperty("UniversityCourses")]
    public Entry Entry { get; set; } = null!;
}
  
