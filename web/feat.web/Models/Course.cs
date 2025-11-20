using feat.common.Models.Enums;

namespace feat.web.Models;

public class Course
{
    public required Guid Id { get; set; }
        
    public required string Title { get; set; }
        
    public string? Provider { get; set; }
    
    public CourseType? CourseType { get; set; }
    
    public string? Requirements { get; set; }
    
    public string? Overview { get; set; }
    
    public string? LocationName { get; set; }
    
    public double? Distance { get; set; }
}