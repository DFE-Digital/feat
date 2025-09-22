
namespace feat.ingestion.Models;

public class University_Course : Base
{
    public required Guid EntryId { get; set; }

    public bool? Foundation { get; set; }

    public bool? Honours { get; set; }

    public bool? NHS { get; set; }

    public bool? Sandwich { get; set; }

    public bool? Year_Abroad { get; set; }

    //[ForeignKey("EntryId")]
}