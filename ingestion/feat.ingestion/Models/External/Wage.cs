namespace feat.ingestion.Models.External;

public class Wage
{
    public string WageType { get; set; }
    
    public double? WageAmount { get; set; }
    
    public string WageUnit { get; set; }
    
    public string? WageAdditionalInformation { get; set; }
    
    public string? WorkingWeekDescription { get; set; }
}