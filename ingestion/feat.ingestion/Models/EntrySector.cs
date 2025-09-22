
namespace feat.ingestion.Models;

public class Entry_Sector : Base
{
    public required Guid EntryId { get; set; }

    public required Guid SectorId { get; set; }

    //[ForeignKey("EntryId")]
    //[ForeignKey("SectorId")]
}