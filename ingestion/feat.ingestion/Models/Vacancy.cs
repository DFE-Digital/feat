
namespace feat.ingestion.Models;

public class Vacancy : Base
{
    public required Guid EntryId { get; set; }

    public required Guid EmployerId { get; set; }

    public short? Positions { get; set; }           // smallint

    public decimal? Wage { get; set; }

    public Wage_Unit_Enum Wage_Unit { get; set; }

    public byte? Hours_Per_Week { get; set; }       // tinyint 

    public DateTime? Closing_Date { get; set; }

    //[ForeignKey("EmployerId")]
    //[ForeignKey("EntryId")]
}