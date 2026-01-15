namespace feat.ingestion.Models.FAA.External;

public class Wage
{
    public required string WageType { get; set; }
    
    public double? WageAmount { get; set; }
    
    public required string WageUnit { get; set; }
    
    public string? WageAdditionalInformation { get; set; }
    
    public string? WorkingWeekDescription { get; set; }
}