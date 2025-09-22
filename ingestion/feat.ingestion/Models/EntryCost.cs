
namespace feat.ingestion.Models;

public class Entry_Cost : Base
{
    public required Guid EntryId { get; set; }

    public double? Value { get; set; }

    public string? Description { get; set; }

    //[ForeignKey("EntryId")]
}