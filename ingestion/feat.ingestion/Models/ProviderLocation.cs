
namespace feat.ingestion.Models;

public class Provider_Location : Base
{
    public required Guid ProviderId { get; set; }

    public required Guid LocationId { get; set; }

    //[ForeignKey("LocationId")]
    //[ForeignKey("ProviderId")]
}