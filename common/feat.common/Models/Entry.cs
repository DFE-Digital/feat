using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using feat.common.Models.Enums;

namespace feat.common.Models;

[Table("Entry")]
public class Entry
{
    [Key]
    public Guid Id { get; set; } 

    public required DateTime Created { get; set; }

    public DateTime? Updated { get; set; } 

    public required Guid ProviderId { get; set; }
    
    [ForeignKey("ProviderId")]
    public Provider Provider { get; set; } = null!;
    
    [StringLength(255)]
    public required string Reference { get; set; } = string.Empty; 
    
    [StringLength(255)]
    public string? SecondaryReference { get; set; }
    
    [StringLength(255)]
    public required string Title { get; set; } = string.Empty;
    
    [StringLength(255)]
    public required string AimOrAltTitle { get; set; } = string.Empty;

    public string? Description { get; set; }

    public required bool FlexibleStart { get; set; }

    public CourseHours? AttendancePattern { get; set; }

    [Column("URL")]
    [StringLength(2083)] 
    public required string Url { get; set; } = string.Empty;
    
    public SourceSystem? SourceSystem { get; set; }

    public DateTime? SourceUpdated { get; set; }

    [StringLength(2083)]
    public string? EntryRequirements { get; set; }

    public EntryType? Type { get; set; }

    public QualificationLevel? Level { get; set; }
    
    public StudyTime? StudyTime { get; set; }

    [InverseProperty("Entry")]
    public ICollection<EntryCost> EntryCosts { get; set; } = new List<EntryCost>();

    [InverseProperty("Entry")]
    public ICollection<EntryInstance> EntryInstances { get; set; } = new List<EntryInstance>();
    
    [InverseProperty("Entry")]
    public ICollection<EntryLocation> EntryLocations { get; set; } = new List<EntryLocation>();

    [InverseProperty("Entry")]
    public ICollection<EntrySector> EntrySectors { get; set; } = new List<EntrySector>();

    [InverseProperty("Entry")]
    public ICollection<UniversityCourse> UniversityCourses { get; set; } = new List<UniversityCourse>();
    
    [InverseProperty("Entry")]
    public ICollection<Vacancy> Vacancies { get; set; } = new List<Vacancy>();
}
