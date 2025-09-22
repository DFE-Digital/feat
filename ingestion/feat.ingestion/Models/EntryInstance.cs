
namespace feat.ingestion.Models;

public class Entry_Instance : BaseEntity
{
    public Guid EntryId { get; set; }

    public DateTime Start_Date { get; set; }

    public TimeSpan Duration { get; set; }

    public Study_Mode_Enum Study_Mode { get; set; } 

    //[ForeignKey("EntryId")]
}