namespace feat.api.Models;

public class CourseDetailsResponse
{
    public Guid Id { get; set; }
    
    public string? Title { get; set; }
    
    public int? Type { get; set; }
    
    public int? Level { get; set; }
    
    public string? EntryRequirements { get; set; }
    
    public string? Description { get; set; }
    
    public int? DeliveryMode { get; set; }
    
    public TimeSpan? Duration { get; set; }
    
    public int? HoursType { get; set; }
    
    public string? EmployerName { get; set; }
    
    public List<Location>? EmployerAddresses { get; set; }
    
    public string? EmployerDescription { get; set; }
    
    public string? ProviderName { get; set; }
    
    public List<Location>? ProviderAddresses { get; set; }
    
    public string? ProviderUrl { get; set; }
    
    public IEnumerable<DateTime?>? StartDates { get; set; }
    
    public IEnumerable<double?>? Costs { get; set; }
    
    public decimal? Wage { get; set; }
    
    public decimal? TuitionFee { get; set; }
    
    public string? PositionAvailable { get; set; }
    
    public string? TrainingProvider { get; set; }
    
    public string? AwardingOrganisation { get; set; }
    
    public string? University { get; set; }
    
    public string? CampusName { get; set; }
    
    public string? CourseUrl { get; set; }
}
