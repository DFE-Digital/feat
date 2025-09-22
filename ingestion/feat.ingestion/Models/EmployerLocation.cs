
namespace feat.ingestion.Models;

public class Employer_Location : Base
{
    public required Guid EmployerId { get; set; }

    public required Guid LocationId { get; set; }

    //[ForeignKey("EmployerId")]
    //[ForeignKey("LocationId")]
}