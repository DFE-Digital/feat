namespace feat.ingestion.Models;

public class Entry_Location : Base
{
    public required Guid EntryId { get; set; }

    public required Guid LocationId { get; set; }

    // [ForeignKey("EntryId")]
    //[ForeignKey("LocationId")]
}