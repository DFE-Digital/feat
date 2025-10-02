using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace feat.common.Models;

[Table("EntryInstance")]
public class EntryInstance
{
    [Key]
    public Guid Id { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Updated { get; set; }

    public Guid EntryId { get; set; }

    public DateTime? StartDate { get; set; }

    public TimeSpan? Duration { get; set; }

    public StudyMode? StudyMode { get; set; }       // e.g., Full-time, Part-time, DistanceLearning
    
    [StringLength(255)]
    public required string Reference { get; set; } = string.Empty; 

    [ForeignKey("EntryId")]
    [InverseProperty("EntryInstances")]
    public Entry Entry { get; set; } = null!;
}

