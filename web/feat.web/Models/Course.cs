using feat.web.Models.ViewModels;

namespace feat.web.Models;

public class Course
{
    public Guid Id { get; set; }
        
    public required string Title { get; set; }
        
    public required string Provider { get; set; }
    
    public required string CourseType { get; set; }
    
    public required string Requirements { get; set; }
    
    public required string Overview { get; set; }
    
    public GeoLocation? Location { get; set; }
    
    public string? LocationName { get; set; }
    
    public double? Distance { get; set; }
    
    public double? Score { get; set; }
}