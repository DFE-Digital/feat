using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace feat.ingestion.Models;

[Table("EntryInstance")]
public class EntryInstance
{
    [Key]
    public Guid Id { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Created { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Updated { get; set; }

    public Guid EntryId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime StartDate { get; set; }

    public TimeOnly Duration { get; set; }

    public StudyMode? StudyMode { get; set; }       // e.g., Full-time, Part-time, DistanceLearning

    [ForeignKey("EntryId")]
    [InverseProperty("EntryInstances")]
    public Entry Entry { get; set; } = null!;
}

