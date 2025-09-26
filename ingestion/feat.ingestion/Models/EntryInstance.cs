using System.ComponentModel.DataAnnotations.Schema;
namespace feat.ingestion.Models;

[Table("EntryInstance")]
public class EntryInstance
{
    public Guid Id { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Updated { get; set; }

    public Guid EntryId { get; set; }

    public DateTime StartDate { get; set; }

    public TimeOnly Duration { get; set; }

    public StudyMode? StudyMode { get; set; } // e.g., Full-time, Part-time, DistanceLearning

    public Entry Entry { get; set; } = null!;
}
//[ForeignKey("EntryId")]
