
namespace feat.ingestion.Models;

public class Provider : BaseEntity
{
    public string? PUBUKPRN { get; set; } 

    public string? UKPRN { get; set; } 

    public required string Name { get; set; } 

    public string? Legal_Entity_Name { get; set; } 

    public string? Trading_Name { get; set; }

    public string? Other_Names { get; set; }

}