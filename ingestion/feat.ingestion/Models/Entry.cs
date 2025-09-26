using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace feat.ingestion.Models;

[Table("Entry")]
public class Entry
{
    public Guid Id { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Updated { get; set; }

    public Guid ProviderId { get; set; }
    
    public string Reference { get; set; } = null!;
    
    public string? SecondaryReference { get; set; }
    
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public bool FlexibleStart { get; set; }

    public AttendancePattern? AttendancePattern { get; set; } // Enum: FullTime, PartTime, Flexible, Other

    public string Url { get; set; } = null!;

    public DateTime? SourceUpdated { get; set; }

    public string? EntryRequirements { get; set; }

    public EntryType? Type { get; set; } // Enum: Apprenticeship, Traineeship, T Level

    public EntryLevel? Level { get; set; } // Enum: Intermediate, Advanced, Higher, Degree, Professional

    public ICollection<EntryCost> EntryCosts { get; set; } = new List<EntryCost>();

    public ICollection<EntryInstance> EntryInstances { get; set; } = new List<EntryInstance>();

    public ICollection<EntryLocation> EntryLocations { get; set; } = new List<EntryLocation>();

    public ICollection<EntrySector> EntrySectors { get; set; } = new List<EntrySector>();

    public Provider Provider { get; set; } = null!;

    public ICollection<UniversityCourse> UniversityCourses { get; set; } = new List<UniversityCourse>();

    public ICollection<Vacancy> Vacancies { get; set; } = new List<Vacancy>();
}

// [ForeignKey("ProviderId")]