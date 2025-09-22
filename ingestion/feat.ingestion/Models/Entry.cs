using System;
using System.Collections.Generic;

namespace feat.ingestion.Models;

public class Entry : BaseEntity
{
    public required Guid ProviderId { get; set; }

    public required string Reference { get; set; }

    public string? Secondary_Reference { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; } 

    public required bool Flexible_Start { get; set; }

    public Attendance_Pattern_Enum Attendance_Pattern { get; set; } 

    public required string URL { get; set; }  

    public DateTime? Source_Updated { get; set; }

    public string? Entry_Requirements { get; set; }

    public Entry_Type_Enum Type { get; set; } 

    public Entry_Level_Enum Level { get; set; } 

    // [ForeignKey("ProviderId")]
}