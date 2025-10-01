using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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
    public required string Title { get; set; } = null!;

    public string? Description { get; set; }

    public required bool FlexibleStart { get; set; }

    public AttendancePattern? AttendancePattern { get; set; }   // Enum: FullTime, PartTime, Flexible, Other

    [Column("URL")]
    [StringLength(2083)] 
    public required string Url { get; set; } = null!;

    public DateTime? SourceUpdated { get; set; }

    [StringLength(2083)]
    public string? EntryRequirements { get; set; }

    public EntryType? Type { get; set; }    // Enum: Apprenticeship, Traineeship, T Level

    public EntryLevel? Level { get; set; }  // Enum: Intermediate, Advanced, Higher, Degree, Professional

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
